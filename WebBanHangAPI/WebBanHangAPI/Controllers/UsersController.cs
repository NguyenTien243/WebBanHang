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
using System.Text.RegularExpressions;
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
        [HttpPost("dangnhap")]
        [AllowAnonymous]
        public async Task<IActionResult> login([FromBody] LoginRequest request)
       // public IActionResult authenticate([FromBody] LoginRequest user)
        {
            if(request.tenDangNhap == null || request.tenDangNhap.Length==0)
                return BadRequest(new Response { Status = 400, Message = "Chưa nhập tên đăng nhập" });
            if (request.matKhau == null || request.matKhau.Length == 0)
                return BadRequest(new Response { Status = 400, Message = "Chưa nhập mật khẩu" });
            if (!ModelState.IsValid)
                return BadRequest(new Response { Status = 400, Message = ModelState.ToString() });
            var finduser = await _context.NguoiDungs.Where(u => u.tenDangNhap == request.tenDangNhap && u.matKhau == request.matKhau).ToListAsync();
            if (finduser.Count == 0)
                return BadRequest(new Response { Status = 400, Message = "Sai tên đăng nhập hoặc mật khẩu" });
            var vaitro = await _context.VaiTros.Where(u => u.VaiTroId == finduser[0].VaiTroId).ToListAsync();
            string tenvaitro = "customer";
            if (vaitro.Count != 0)
                tenvaitro = vaitro[0].tenVaiTro;
            var token = _jwtAuthenticationManager.Authenticate(finduser[0], tenvaitro);
            if (token == null)
                return Unauthorized();
            return Ok(token);
        }
        public static bool isEmail(string inputEmail)
        {
            inputEmail = inputEmail ?? string.Empty;
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                  @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                  @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(inputEmail))
                return (true);
            else
                return (false);
        }
        [HttpPost("dangky")]
        [AllowAnonymous]
        public async Task<IActionResult> register([FromBody] RegisterRequest request)
        // public IActionResult authenticate([FromBody] LoginRequest user)
        {
            if (request.email == null || request.email.Length == 0)
                return BadRequest(new Response { Status = 400, Message = "Email bắt buộc" });
            
            if (!isEmail(request.email))
                return BadRequest(new Response { Status = 400, Message = "Sai định dạng Email" });
            if (request.tenDangNhap == null || request.tenDangNhap.Length == 0)
                return BadRequest(new Response { Status = 400, Message = "Tên đăng nhập bắt buộc" });
            if (request.tenNguoiDung == null || request.tenNguoiDung.Length == 0)
                return BadRequest(new Response { Status = 400, Message = "Tên người dùng bắt buộc" });

            if (request.matKhau == null || request.matKhau.Length < 8)
                return BadRequest(new Response { Status = 400, Message = "mật khẩu bắt buộc, tối thiểu 8 ký tự" });
            if (request.xacNhanMatKhau == null || request.xacNhanMatKhau.Length == 0)
                return BadRequest(new Response { Status = 400, Message = "xác nhận mật khẩu bắt buộc" });
            if (request.xacNhanMatKhau.Trim() !=  request.matKhau.Trim())
                return BadRequest(new Response { Status = 400, Message = "mật khẩu xác nhận không trùng" });
            if (request.tenDangNhap == null || request.tenDangNhap.Length == 0)
                return BadRequest(new Response { Status = 400, Message = "Tên đăng nhập bắt buộc" });
            var finduser = await _context.NguoiDungs.Where(u => u.tenDangNhap == request.tenDangNhap ).ToListAsync();
            if (finduser.Count != 0)
                return BadRequest(new Response { Status = 400, Message = "Tên đăng nhập đã tồn tại, vui lòng chọn tài khoản khác" });
            var findemail = await _context.NguoiDungs.Where(u => u.email == request.email).ToListAsync();
            if (findemail.Count != 0)
                return BadRequest(new Response { Status = 400, Message = "Email đã đăng ký, vui lòng chọn email khoản khác" });
            NguoiDung nguoiDung = new NguoiDung();
            nguoiDung.tenNguoiDung = request.tenNguoiDung;
            nguoiDung.email = request.email;
            nguoiDung.sDT = request.sDT;
            nguoiDung.diaChi = request.diaChi;
            nguoiDung.tenDangNhap = request.tenDangNhap;
            nguoiDung.matKhau = request.matKhau;
            nguoiDung.VaiTroId = "3";

            try
            {
                _context.NguoiDungs.Add(nguoiDung);
                await _context.SaveChangesAsync();
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = e.ToString() });
            }
            return Ok(new Response { Status = 200, Message = "Tạo tài khoản thành công" });
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
