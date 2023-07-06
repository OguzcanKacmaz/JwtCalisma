using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MiniApp2.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetInvoice()
        {
            var userName = HttpContext.User.Identity!.Name;
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            return StatusCode(200, $"Invoice İşlemleri => UserName : {userName} - UserId : {userIdClaim.Value}");
        }
    }
}
