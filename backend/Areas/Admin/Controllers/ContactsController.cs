using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCms.Api.Data;
using MyCms.Api.DTOs;
using MyCms.Api.Models;
using System.Security.Claims;

namespace MyCms.Api.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize]
    public class ContactsController : ControllerBase
    {
        private readonly CmsDbContext _context;

        public ContactsController(CmsDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有联系信息（管理员）
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<object>> GetContacts([FromQuery] ContactQueryDto query)
        {
            var queryable = _context.Contacts.AsQueryable();

            // 状态过滤
            if (query.Status.HasValue)
            {
                queryable = queryable.Where(c => c.Status == query.Status.Value);
            }

            // 搜索
            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                queryable = queryable.Where(c => c.Name.Contains(query.SearchTerm) ||
                                               c.Email.Contains(query.SearchTerm) ||
                                               c.Subject.Contains(query.SearchTerm) ||
                                               c.Message.Contains(query.SearchTerm));
            }

            // 总数
            var totalCount = await queryable.CountAsync();

            // 分页
            var contacts = await queryable
                .OrderByDescending(c => c.CreatedAt)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(c => new ContactDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Email = c.Email,
                    Phone = c.Phone,
                    Company = c.Company,
                    Subject = c.Subject,
                    Message = c.Message,
                    Status = c.Status,
                    Reply = c.Reply,
                    RepliedAt = c.RepliedAt,
                    RepliedById = c.RepliedById,
                    IpAddress = c.IpAddress,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return Ok(new
            {
                items = contacts,
                totalCount,
                page = query.Page,
                pageSize = query.PageSize,
                totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize)
            });
        }

        /// <summary>
        /// 根据ID获取联系信息（管理员）
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ContactDto>> GetContact(int id)
        {
            var contact = await _context.Contacts
                .Where(c => c.Id == id)
                .Select(c => new ContactDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Email = c.Email,
                    Phone = c.Phone,
                    Company = c.Company,
                    Subject = c.Subject,
                    Message = c.Message,
                    Status = c.Status,
                    Reply = c.Reply,
                    RepliedAt = c.RepliedAt,
                    RepliedById = c.RepliedById,
                    IpAddress = c.IpAddress,
                    CreatedAt = c.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (contact == null)
            {
                return NotFound("联系信息不存在");
            }

            return Ok(contact);
        }

        /// <summary>
        /// 回复联系信息（管理员）
        /// </summary>
        [HttpPut("{id}/reply")]
        public async Task<ActionResult<ContactDto>> ReplyContact(int id, UpdateContactStatusDto replyDto)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound("联系信息不存在");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("无效的用户信息");
            }

            contact.Reply = replyDto.Reply;
            contact.Status = ContactStatus.Replied;
            contact.RepliedAt = DateTime.UtcNow;
            contact.RepliedById = userId;

            await _context.SaveChangesAsync();

            var contactDto = new ContactDto
            {
                Id = contact.Id,
                Name = contact.Name,
                Email = contact.Email,
                Phone = contact.Phone,
                Company = contact.Company,
                Subject = contact.Subject,
                Message = contact.Message,
                Status = contact.Status,
                Reply = contact.Reply,
                RepliedAt = contact.RepliedAt,
                RepliedById = contact.RepliedById,
                IpAddress = contact.IpAddress,
                CreatedAt = contact.CreatedAt
            };

            return Ok(contactDto);
        }

        /// <summary>
        /// 更新联系信息状态（管理员）
        /// </summary>
        [HttpPut("{id}/status")]
        public async Task<ActionResult<ContactDto>> UpdateContactStatus(int id, UpdateContactStatusDto statusDto)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound("联系信息不存在");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("无效的用户信息");
            }

            contact.Status = statusDto.Status;

            if (!string.IsNullOrEmpty(statusDto.Reply))
            {
                contact.Reply = statusDto.Reply;
                contact.RepliedAt = DateTime.UtcNow;
                contact.RepliedById = userId;
            }

            await _context.SaveChangesAsync();

            var contactDto = new ContactDto
            {
                Id = contact.Id,
                Name = contact.Name,
                Email = contact.Email,
                Phone = contact.Phone,
                Company = contact.Company,
                Subject = contact.Subject,
                Message = contact.Message,
                Status = contact.Status,
                Reply = contact.Reply,
                RepliedAt = contact.RepliedAt,
                RepliedById = contact.RepliedById,
                IpAddress = contact.IpAddress,
                CreatedAt = contact.CreatedAt
            };

            return Ok(contactDto);
        }

        /// <summary>
        /// 删除联系信息（管理员）
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteContact(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound("联系信息不存在");
            }

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

            return Ok("联系信息删除成功");
        }

        /// <summary>
        /// 获取联系消息统计（管理员）
        /// </summary>
        [HttpGet("statistics")]
        public async Task<ActionResult<object>> GetContactStatistics()
        {
            var totalCount = await _context.Contacts.CountAsync();
            var todayCount = await _context.Contacts
                .Where(c => c.CreatedAt.Date == DateTime.UtcNow.Date)
                .CountAsync();

            var statusStats = await _context.Contacts
                .GroupBy(c => c.Status)
                .Select(g => new
                {
                    status = g.Key,
                    count = g.Count()
                })
                .ToListAsync();

            return Ok(new
            {
                totalCount,
                todayCount,
                statusStats
            });
        }
    }
}