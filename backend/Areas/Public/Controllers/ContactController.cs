using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCms.Api.Data;
using MyCms.Api.DTOs;
using MyCms.Api.Models;

namespace MyCms.Api.Areas.Public.Controllers
{
    [Area("Public")]
    [Route("api/public/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ContactController : ControllerBase
    {
        private readonly CmsDbContext _context;

        public ContactController(CmsDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 提交联系我们表单（公开接口）
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ContactDto>> SubmitContact(CreateContactDto createDto)
        {
            try
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

                return Ok(contactDto);
            }
            catch (Exception ex)
            {
                return BadRequest($"提交失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取客户端IP地址
        /// </summary>
        private string GetClientIpAddress()
        {
            // 尝试从代理头获取真实IP
            var xForwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(xForwardedFor))
            {
                return xForwardedFor.Split(',')[0].Trim();
            }

            var xRealIp = Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(xRealIp))
            {
                return xRealIp;
            }

            // 使用连接的远程IP
            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }
}