using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBanHangAPI.IServices;
using WebBanHangAPI.Models;

namespace WebBanHangAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NameController : ControllerBase
    {
        private readonly ICustomAuthenticationManager customAuthenticationManager;

        public NameController(WebBanHangAPIDBContext context, ICustomAuthenticationManager customAuthenticationManager)
        {
           
            this.customAuthenticationManager = customAuthenticationManager;

        }
        [Authorize]

        [HttpGet("name")]
        public IEnumerable<string> Get()
        {
            return new string[] { "New Jersey", "New Jorl" };
        }
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] NguoiDung user)
        {
            var token = customAuthenticationManager.Authenticate
                (user.tenDangNhap, user.matKhau);
            if (token == null)
                return Unauthorized();
            return Ok(token);
        }
    }
}
