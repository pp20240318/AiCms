using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCms.Api.Data;
using MyCms.Api.DTOs;
using MyCms.Api.Models;
using System.Security.Claims;

namespace MyCms.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly CmsDbContext _context;

        public ContactsController(CmsDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 提交联系我们表单（公开接口）
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<ContactDto>> SubmitContact(CreateContactDto createDto)
        {
            var contact = new Contact
            {
                Name = createDto.Name,
                Email = createDto.Email,
                Phone = createDto.Phone,
                Company = createDto.Company,
                Subject = createDto.Subject,
                Message = createDto.Message,
                Status = ContactStatus.New,
                IpAddress = GetClientIpAddress(),
                UserAgent = Request.Headers.UserAgent.ToString()
            };

            _context.Contacts.Add(contact);
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

            return CreatedAtAction(nameof(GetContact), new { id = contact.Id }, contactDto);
        }

        /// <summary>
        /// 获取联系消息列表（管理员）
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<object>> GetContacts([FromQuery] ContactQueryDto query)
        {
            var queryable = _context.Contacts.AsQueryable();

            // 状态筛选
            if (query.Status.HasValue)
            {
                queryable = queryable.Where(c => c.Status == query.Status.Value);
            }

            // 关键词搜索
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                queryable = queryable.Where(c =>
                    c.Name.Contains(query.SearchTerm) ||
                    c.Email.Contains(query.SearchTerm) ||
                    c.Subject.Contains(query.SearchTerm) ||
                    c.Company!.Contains(query.SearchTerm));
            }

            // 日期范围筛选
            if (query.StartDate.HasValue)
            {
                queryable = queryable.Where(c => c.CreatedAt >= query.StartDate.Value);
            }

            if (query.EndDate.HasValue)
            {
                queryable = queryable.Where(c => c.CreatedAt <= query.EndDate.Value);
            }

            var totalCount = await queryable.CountAsync();

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
                data = contacts,
                totalCount,
                pageCount = (int)Math.Ceiling((double)totalCount / query.PageSize),
                currentPage = query.Page,
                pageSize = query.PageSize
            });
        }

        /// <summary>
        /// 获取指定联系消息
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
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
                return NotFound();
            }

            return Ok(contact);
        }

        /// <summary>
        /// 更新联系消息状态和回复
        /// </summary>
        [HttpPut("{id}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateContactStatus(int id, UpdateContactStatusDto updateDto)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

            contact.Status = updateDto.Status;
            contact.Reply = updateDto.Reply;

            if (updateDto.Status == ContactStatus.Replied && !string.IsNullOrWhiteSpace(updateDto.Reply))
            {
                contact.RepliedAt = DateTime.UtcNow;
                contact.RepliedById = GetCurrentUserId();
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ContactExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        /// <summary>
        /// 删除联系消息
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// 获取联系消息统计
        /// </summary>
        [HttpGet("statistics")]
        [Authorize]
        public async Task<ActionResult<object>> GetContactStatistics()
        {
            var stats = await _context.Contacts
                .GroupBy(c => c.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            var totalCount = await _context.Contacts.CountAsync();
            var todayCount = await _context.Contacts
                .Where(c => c.CreatedAt.Date == DateTime.UtcNow.Date)
                .CountAsync();

            return Ok(new
            {
                totalCount,
                todayCount,
                statusStats = stats
            });
        }

        private async Task<bool> ContactExists(int id)
        {
            return await _context.Contacts.AnyAsync(e => e.Id == id);
        }

        private string? GetClientIpAddress()
        {
            var ipAddress = Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = Request.Headers["X-Real-IP"].FirstOrDefault();
            }
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            }
            return ipAddress;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }
    }
}