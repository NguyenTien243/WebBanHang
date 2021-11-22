using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebBanHangAPI.Common;
using WebBanHangAPI.IServices;
using WebBanHangAPI.Models;
using WebBanHangAPI.ViewModels;

namespace WebBanHangAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly WebBanHangAPIDBContext _context;
        private readonly IConfiguration _configuration;
        private readonly IJwtAuthenticationManager _jwtAuthenticationManager;
        public UsersController(WebBanHangAPIDBContext context, IConfiguration config, IJwtAuthenticationManager jwtAuthenticationManager)
        {
            _context = context;
            _configuration = config;
            _jwtAuthenticationManager = jwtAuthenticationManager;
        }
        [HttpGet("name")]
        public IEnumerable<string> Get()
        {
            return new string[] { "New Jersey", "New Jorl" };
        }
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult authenticate([FromBody] LoginRequest user)
        {
            var token = _jwtAuthenticationManager.Authenticate(user.tenDangNhap, user.matKhau);
            if (token == null)
                return Unauthorized();
            return Ok(token);
        }
        //[HttpPost("login")]
        //[AllowAnonymous]
        //public async Task<IActionResult> Authenticate([FromBody] LoginRequest request)
        //{
        //    if(!ModelState.IsValid)
        //        return BadRequest(new Response { Status = 400, Message = ModelState.ToString() });
        //    var user = await _context.NguoiDungs.Where(u => u.tenDangNhap == request.tenDangNhap && u.matKhau == request.matKhau).ToListAsync();
        //    if(user == null)
        //        return BadRequest(new Response { Status = 400, Message = "Sai tên đăng nhập hoặc mật khẩu" });
        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        //    var claims = new[]
        //    {
        //        new Claim("NguoiDungId",user[0].NguoiDungId)
        //        //new Claim(ClaimTypes.GivenName,user.FirstName),
        //        //new Claim(ClaimTypes.Role, string.Join(";",roles)),
        //        //new Claim("", request.UserName)
        //    };
        //    var token = new JwtSecurityToken(_configuration["Tokens:Issuer"],
        //        _configuration["Tokens:Issuer"],
        //        claims,
        //        expires: DateTime.Now.AddHours(3),
        //        signingCredentials: creds);
        //    var tokena = JwtSecurityTokenHandler().WriteToken(token);
        //    return Ok();
        //}
    }
}
