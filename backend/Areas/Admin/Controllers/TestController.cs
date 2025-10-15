using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyCms.Api.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public ActionResult<object> Get()
        {
            return Ok(new { message = "Admin Area 测试成功!", area = "Admin", timestamp = DateTime.UtcNow });
        }
    }
}