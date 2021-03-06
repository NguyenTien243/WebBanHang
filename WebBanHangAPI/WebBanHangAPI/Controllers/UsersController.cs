using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
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
        private readonly IMailService _mailService;
        private readonly WebBanHangAPIDBContext _context;
        private readonly IConfiguration _configuration;
        private readonly IJwtAuthenticationManager _jwtAuthenticationManager;
        public UsersController(WebBanHangAPIDBContext context, IConfiguration config, IJwtAuthenticationManager jwtAuthenticationManager, IMailService mailService)
        {
            _context = context;
            _configuration = config;
            _jwtAuthenticationManager = jwtAuthenticationManager;
            _mailService = mailService;
        }
        [HttpGet("name")]
        public IEnumerable<string> Get()
        {
            return new string[] { "New Jersey", "New Jorl" };
        }
        [AllowAnonymous]

        [HttpGet("GetHeaderData")]
        public ActionResult<string> GetHeaderData(string headerKey)
        {

            Request.Headers.TryGetValue("Authorization", out var tokenheaderValue);
            JwtSecurityToken token = _jwtAuthenticationManager.GetInFo(tokenheaderValue);
            var NguoiDungId = token.Claims.First(claim => claim.Type == "nguoiDungId").Value;
            return Ok();
        }
        [HttpPost("sendEmailResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> GuiEmailKhoiPhucMatKhau([FromBody] RequestSendEmaiResetPassword request)
        // public IActionResult authenticate([FromBody] LoginRequest user)
        {
            if(String.IsNullOrEmpty(request.url))
                return BadRequest(new Response { Status = 400, Message = "Thiếu đường dẫn tới trang Khôi phục mật khẩu!" });
            if (!isEmail(request.email))
                return BadRequest(new Response { Status = 400, Message = "Sai định dạng Email" });
            var finduser = await _context.NguoiDungs.Where(u => u.email == request.email).ToListAsync();
            if (finduser.Count == 0)
                return BadRequest(new Response { Status = 400, Message = "Email chưa đăng ký trong hệ thống!" });
            var vaitro = await _context.VaiTros.Where(u => u.VaiTroId == finduser[0].VaiTroId).ToListAsync();
            string tenvaitro = "customer";
            if (vaitro.Count != 0)
                tenvaitro = vaitro[0].tenVaiTro;
            var token = _jwtAuthenticationManager.TokenResetPassword(finduser[0], tenvaitro);
            if (token == null)
                return Unauthorized();
            var message = new MailRequest();
            message.Subject = "Khôi phục mật khẩu";
            message.ToEmail = request.email;
            message.Body = $"Xin chào {finduser[0].tenNguoiDung}." +
                "<br/>" +
                "Chúng tôi nhận được yêu cầu khôi phục mật khẩu từ bạn. Vui lòng nhấp vào link sau để khôi phục mật khẩu:" + $"{request.url}?token={token}" +
                "<br/>" +
                "Yêu cầu khôi phục mật khẩu có hiệu lực trong 5 phút"
            ;
            try
            {
                await _mailService.SendEmailAsync(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Ok(new Response { Status = 200, Message = "Gửi yêu cầu thành công, vui lòng kiểm tra email" });
        }
        [Authorize]
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> KhoiPhucMatKhau([FromBody] ResetPasswordModel request)
        {

            if (request.matKhauMoi == null || request.matKhauMoi.Trim().Length == 0)
                return BadRequest(new Response { Status = 400, Message = "Mật khẩu mới không được bỏ trống!" });
            if (request.xacNhanMatKhauMoi == null || request.xacNhanMatKhauMoi.Trim().Length == 0)
                return BadRequest(new Response { Status = 400, Message = "Xác nhận mật khẩu mới không được bỏ trống!" });
            if (request.matKhauMoi.Length < 8)
                return BadRequest(new Response { Status = 400, Message = "Mật khẩu mới tối thiểu 8 ký tự" });
            if (request.xacNhanMatKhauMoi != request.matKhauMoi)
                return BadRequest(new Response { Status = 400, Message = "Mật khẩu xác nhận không trùng" });
            var NguoiDungId = "";
            Request.Headers.TryGetValue("Authorization", out var tokenheaderValue);
            JwtSecurityToken token = null;
            try
            {
                token = _jwtAuthenticationManager.GetInFo(tokenheaderValue);
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = "Không xác thực được người dùng" });
            }
            NguoiDungId = token.Claims.First(claim => claim.Type == "nguoiDungId").Value;

            var findUser = await _context.NguoiDungs.FindAsync(NguoiDungId);
            if (findUser == null)
            {
                return NotFound(new Response { Status = 404, Message = "Không tìm thấy người dùng" });
            }
            string newPasswordOldHashed = GenerateSha256Hash(request.matKhauMoi, _configuration["AppSettings:Salt"]);

            try
            {
                findUser.matKhau = newPasswordOldHashed;
                await _context.SaveChangesAsync();
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = e.ToString() });
            }
            return Ok(new Response { Status = 200, Message = "Khôi phục mật khẩu thành công", Data = null });

        }

        [HttpPost("dangnhap")]
        [AllowAnonymous]
        public async Task<IActionResult> DangNhap([FromBody] LoginRequest request)
        // public IActionResult authenticate([FromBody] LoginRequest user)
        {
            if (string.IsNullOrEmpty(request.tenDangNhap))
                return BadRequest(new Response { Status = 400, Message = "Chưa nhập tên đăng nhập" });
            if (string.IsNullOrEmpty(request.matKhau))
                return BadRequest(new Response { Status = 400, Message = "Chưa nhập mật khẩu" });
            string passwordHashed = GenerateSha256Hash(request.matKhau, _configuration["AppSettings:Salt"]);

            var finduser = await _context.NguoiDungs.Where(u => u.tenDangNhap == request.tenDangNhap && u.matKhau == passwordHashed).ToListAsync();
            if (finduser.Count == 0)
                return BadRequest(new Response { Status = 400, Message = "Sai tên đăng nhập hoặc mật khẩu" });
            var vaitro = await _context.VaiTros.Where(u => u.VaiTroId == finduser[0].VaiTroId).ToListAsync();
            string tenvaitro = "customer";
            if (vaitro.Count != 0)
                tenvaitro = vaitro[0].tenVaiTro;
            var token = _jwtAuthenticationManager.Authenticate(finduser[0], tenvaitro);
            if (token == null)
                return Unauthorized();
            return Ok(new Response { Status = 200, Message = "Đăng nhập thành công", Data = token });
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
        public async Task<IActionResult> DangKy([FromBody] RegisterRequest request)
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
            if (request.diaChi == null || request.diaChi.Length == 0)
                return BadRequest(new Response { Status = 400, Message = "Địa chỉ bắt buộc" });
            if (request.sDT == null || request.sDT.Length == 0)
                return BadRequest(new Response { Status = 400, Message = "Số điện thoại bắt buộc" });
            if (request.matKhau == null || request.matKhau.Length < 8)
                return BadRequest(new Response { Status = 400, Message = "mật khẩu bắt buộc, tối thiểu 8 ký tự" });
            if (request.xacNhanMatKhau == null || request.xacNhanMatKhau.Length == 0)
                return BadRequest(new Response { Status = 400, Message = "xác nhận mật khẩu bắt buộc" });
            if (request.xacNhanMatKhau.Trim() != request.matKhau.Trim())
                return BadRequest(new Response { Status = 400, Message = "mật khẩu xác nhận không trùng" });
            if (request.tenDangNhap == null || request.tenDangNhap.Length == 0)
                return BadRequest(new Response { Status = 400, Message = "Tên đăng nhập bắt buộc" });
            var finduser = await _context.NguoiDungs.Where(u => u.tenDangNhap == request.tenDangNhap).ToListAsync();
            if (finduser.Count != 0)
                return BadRequest(new Response { Status = 400, Message = "Tên đăng nhập đã tồn tại, vui lòng chọn tài khoản khác" });
            var findemail = await _context.NguoiDungs.Where(u => u.email == request.email).ToListAsync();
            if (findemail.Count != 0)
                return BadRequest(new Response { Status = 400, Message = "Email đã đăng ký, vui lòng chọn email khoản khác" });

            string passwordHashed = GenerateSha256Hash(request.matKhau, _configuration["AppSettings:Salt"]);
            NguoiDung nguoiDung = new NguoiDung();
            nguoiDung.tenNguoiDung = request.tenNguoiDung;
            nguoiDung.email = request.email;
            nguoiDung.sDT = request.sDT;
            nguoiDung.diaChi = request.diaChi;
            nguoiDung.tenDangNhap = request.tenDangNhap;
            nguoiDung.matKhau = passwordHashed;
            nguoiDung.VaiTroId = "3";
            nguoiDung.conHoatDong = true;

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

        [Authorize]
        [HttpPost("dangkyNhanVien")]
        public async Task<IActionResult> DangKyNhanVien([FromBody] RegisterRequest request)
        // public IActionResult authenticate([FromBody] LoginRequest user)
        {
            var NguoiDungRole = "";
            Request.Headers.TryGetValue("Authorization", out var tokenheaderValue);
            JwtSecurityToken token = null;
            try
            {
                token = _jwtAuthenticationManager.GetInFo(tokenheaderValue);
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = "Không xác thực được người dùng" });
            }
            NguoiDungRole = token.Claims.First(claim => claim.Type == "vaiTro").Value;
            if (NguoiDungRole != "admin")
                return BadRequest(new Response { Status = 400, Message = "Không có quyền!, vui lòng đăng nhập với tài khoản admin" });
            if (request.email == null || request.email.Length == 0)
                return BadRequest(new Response { Status = 400, Message = "Email bắt buộc" });

            if (!isEmail(request.email))
                return BadRequest(new Response { Status = 400, Message = "Sai định dạng Email" });
            if (request.tenDangNhap == null || request.tenDangNhap.Length == 0)
                return BadRequest(new Response { Status = 400, Message = "Tên đăng nhập bắt buộc" });
            if (request.tenNguoiDung == null || request.tenNguoiDung.Length == 0)
                return BadRequest(new Response { Status = 400, Message = "Tên người dùng bắt buộc" });
            if (request.diaChi == null || request.diaChi.Length == 0)
                return BadRequest(new Response { Status = 400, Message = "Địa chỉ bắt buộc" });
            if (request.sDT == null || request.sDT.Length == 0)
                return BadRequest(new Response { Status = 400, Message = "Số điện thoại bắt buộc" });
            if (request.matKhau == null || request.matKhau.Length < 8)
                return BadRequest(new Response { Status = 400, Message = "mật khẩu bắt buộc, tối thiểu 8 ký tự" });
            if (request.xacNhanMatKhau == null || request.xacNhanMatKhau.Length == 0)
                return BadRequest(new Response { Status = 400, Message = "xác nhận mật khẩu bắt buộc" });
            if (request.xacNhanMatKhau.Trim() != request.matKhau.Trim())
                return BadRequest(new Response { Status = 400, Message = "mật khẩu xác nhận không trùng" });
            if (request.tenDangNhap == null || request.tenDangNhap.Length == 0)
                return BadRequest(new Response { Status = 400, Message = "Tên đăng nhập bắt buộc" });
            var finduser = await _context.NguoiDungs.Where(u => u.tenDangNhap == request.tenDangNhap).ToListAsync();
            if (finduser.Count != 0)
                return BadRequest(new Response { Status = 400, Message = "Tên đăng nhập đã tồn tại, vui lòng chọn tài khoản khác" });
            var findemail = await _context.NguoiDungs.Where(u => u.email == request.email).ToListAsync();
            if (findemail.Count != 0)
                return BadRequest(new Response { Status = 400, Message = "Email đã đăng ký, vui lòng chọn email khoản khác" });

            string passwordHashed = GenerateSha256Hash(request.matKhau, _configuration["AppSettings:Salt"]);
            NguoiDung nguoiDung = new NguoiDung();
            nguoiDung.tenNguoiDung = request.tenNguoiDung;
            nguoiDung.email = request.email;
            nguoiDung.sDT = request.sDT;
            nguoiDung.diaChi = request.diaChi;
            nguoiDung.tenDangNhap = request.tenDangNhap;
            nguoiDung.matKhau = passwordHashed;
            nguoiDung.VaiTroId = "2";
            nguoiDung.conHoatDong = true;

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
        [Authorize]
        [HttpPut("khachhangcapnhatthongtin")]
        public async Task<IActionResult> KHCapNhatThongTin([FromBody] EditNguoiDungModel request)
        {
            var NguoiDungRole = "";
            var NguoiDungId = "";
            Request.Headers.TryGetValue("Authorization", out var tokenheaderValue);
            JwtSecurityToken token = null;
            try
            {
                token = _jwtAuthenticationManager.GetInFo(tokenheaderValue);
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = "Không xác thực được người dùng" });
            }
            NguoiDungRole = token.Claims.First(claim => claim.Type == "vaiTro").Value;
            NguoiDungId = token.Claims.First(claim => claim.Type == "nguoiDungId").Value;
            if (NguoiDungRole != "customer")
                return BadRequest(new Response { Status = 400, Message = "Không có quyền!, vui lòng đăng nhập với tài khoản Khách hàng!" });
            if (string.IsNullOrEmpty(request.tenNguoiDung))
                return BadRequest(new Response { Status = 400, Message = "Tên người dùng không được bỏ trống!" });
            if (string.IsNullOrEmpty(request.diaChi))
                return BadRequest(new Response { Status = 400, Message = "Địa chỉ không được bỏ trống" });
            if (string.IsNullOrEmpty(request.email))
                return BadRequest(new Response { Status = 400, Message = "Email không được bỏ trống!" });
            if (string.IsNullOrEmpty(request.sDT))
                return BadRequest(new Response { Status = 400, Message = "Số điện thoại không được bỏ trống" });

            if (!isEmail(request.email))
                return BadRequest(new Response { Status = 400, Message = "Sai định dạng Email" });
            var findUser = await _context.NguoiDungs.FindAsync(NguoiDungId);
            if (findUser == null)
                return BadRequest(new Response { Status = 400, Message = "Không tìm thấy người dùng!" });

            try
            {
                findUser.tenNguoiDung = request.tenNguoiDung;
                findUser.diaChi = request.diaChi;
                findUser.email = request.email;
                findUser.sDT = request.sDT;
                await _context.SaveChangesAsync();
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = e.ToString() });
            }
            return Ok(new Response { Status = 200, Message = "Cập nhật thông tin thành công", Data = request });
        }

        [Authorize]
        [HttpPut("AdminsuathongtinNhanVienKhachHang")]
        public async Task<IActionResult> AdminCapNhatNhanVienKhachHang([FromBody] AdminEditNguoiDungModel request)
        {
            var NguoiDungRole = "";
            Request.Headers.TryGetValue("Authorization", out var tokenheaderValue);
            JwtSecurityToken token = null;
            try
            {
                token = _jwtAuthenticationManager.GetInFo(tokenheaderValue);
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = "Không xác thực được người dùng" });
            }
            NguoiDungRole = token.Claims.First(claim => claim.Type == "vaiTro").Value;
            if (NguoiDungRole != "admin")
                return BadRequest(new Response { Status = 400, Message = "Không có quyền!, vui lòng đăng nhập với tài khoản admin" });
            if (string.IsNullOrEmpty(request.NguoiDungId))
                return BadRequest(new Response { Status = 400, Message = "Thiếu người dùng Id!" });
            if (string.IsNullOrEmpty(request.tenNguoiDung))
                return BadRequest(new Response { Status = 400, Message = "Tên người dùng không được bỏ trống!" });
            if (string.IsNullOrEmpty(request.diaChi))
                return BadRequest(new Response { Status = 400, Message = "Địa chỉ không được bỏ trống" });
            if (string.IsNullOrEmpty(request.email))
                return BadRequest(new Response { Status = 400, Message = "Email không được bỏ trống!" });
            if (string.IsNullOrEmpty(request.sDT))
                return BadRequest(new Response { Status = 400, Message = "Số điện thoại không được bỏ trống" });

            if (!isEmail(request.email))
                return BadRequest(new Response { Status = 400, Message = "Sai định dạng Email" });
            var NguoiDungId = request.NguoiDungId;
            var findUser = await _context.NguoiDungs.FindAsync(NguoiDungId);
            if (findUser == null)
                return BadRequest(new Response { Status = 400, Message = "Không tìm thấy người dùng!" });

            try
            {
                findUser.tenNguoiDung = request.tenNguoiDung;
                findUser.diaChi = request.diaChi;
                findUser.email = request.email;
                findUser.sDT = request.sDT;
                await _context.SaveChangesAsync();
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = e.ToString() });
            }
            return Ok(new Response { Status = 200, Message = "Cập nhật thông tin thành công", Data = request });
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
        [Authorize]
        [HttpGet("xemthongtinnguoidung")]
        public async Task<IActionResult> XemThongTinNguoiDung()
        {
            var NguoiDungId = "";
            Request.Headers.TryGetValue("Authorization", out var tokenheaderValue);
            JwtSecurityToken token = null;
            try
            {
                token = _jwtAuthenticationManager.GetInFo(tokenheaderValue);
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = "Không xác thực được người dùng" });
            }
            NguoiDungId = token.Claims.First(claim => claim.Type == "nguoiDungId").Value;

            var user = await _context.NguoiDungs.Include(p => p.VaiTro).Where(s => s.NguoiDungId == NguoiDungId).Select(nd => new ThongTinNguoiDungModel { NguoiDungId = nd.NguoiDungId, VaiTro = nd.VaiTro.tenVaiTro, email = nd.email, sDT = nd.sDT, conHoatDong = nd.conHoatDong, diaChi = nd.diaChi, gioiTinh = nd.gioiTinh, tenNguoiDung = nd.tenNguoiDung, VaiTroId = nd.VaiTroId }).ToListAsync();
            if (user.Count == 0)
                return BadRequest(new Response { Status = 400, Message = "Không tìm thấy thông tin người dùng!" });
            return Ok(new Response { Status = 200, Message = Message.Success, Data = user });
        }
      
        [HttpGet("laydanhsachKhachHang")]
        public async Task<IActionResult> LayDanhSachKhachHang()
        {

            var listcustomer = await _context.NguoiDungs.Include(p => p.VaiTro).Where(s => s.VaiTro.tenVaiTro == "customer" && s.daXoa == false).Select(nd => new NguoiDungModel { NguoiDungId = nd.NguoiDungId, tenDangNhap = nd.tenDangNhap, email = nd.email, sDT = nd.sDT, conHoatDong = nd.conHoatDong, diaChi = nd.diaChi, gioiTinh = nd.gioiTinh, tenNguoiDung = nd.tenNguoiDung, VaiTroId = nd.VaiTroId }).ToListAsync();

            return Ok(new Response { Status = 200, Message = Message.Success, Data = listcustomer });
        }
        
        [HttpGet("laydanhsachNhanVien")]
        public async Task<IActionResult> LayDanhSachNhanVien()
        {

            var listcustomer = await _context.NguoiDungs.Include(p => p.VaiTro).Where(s => s.VaiTro.tenVaiTro == "staff" && s.daXoa == false).Select(nd => new NguoiDungModel { NguoiDungId = nd.NguoiDungId, tenDangNhap = nd.tenDangNhap, email = nd.email, sDT = nd.sDT, conHoatDong = nd.conHoatDong, diaChi = nd.diaChi, gioiTinh = nd.gioiTinh, tenNguoiDung = nd.tenNguoiDung, VaiTroId = nd.VaiTroId }).ToListAsync();

            return Ok(new Response { Status = 200, Message = Message.Success, Data = listcustomer });
        }

        [HttpPost("doimatkhau")]
        public async Task<IActionResult> DoiMatKhau([FromBody] EditPassword request)
        {

            if (request.matKhauHienTai == null || request.matKhauHienTai.Trim().Length == 0)
                return BadRequest(new Response { Status = 400, Message = "Mật khẩu hiện tại không được bỏ trống!" });
            if (request.matKhauMoi == null || request.matKhauMoi.Trim().Length == 0)
                return BadRequest(new Response { Status = 400, Message = "Mật khẩu mới không được bỏ trống!" });
            if (request.xacNhanMatKhauMoi == null || request.xacNhanMatKhauMoi.Trim().Length == 0)
                return BadRequest(new Response { Status = 400, Message = "Xác nhận mật khẩu mới không được bỏ trống!" });
            if (request.matKhauMoi.Length < 8)
                return BadRequest(new Response { Status = 400, Message = "Mật khẩu mới tối thiểu 8 ký tự" });
            if (request.xacNhanMatKhauMoi != request.matKhauMoi)
                return BadRequest(new Response { Status = 400, Message = "Mật khẩu xác nhận không trùng" });
            var NguoiDungId = "";
            Request.Headers.TryGetValue("Authorization", out var tokenheaderValue);
            JwtSecurityToken token = null;
            try
            {
                token = _jwtAuthenticationManager.GetInFo(tokenheaderValue);
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = "Không xác thực được người dùng" });
            }
            NguoiDungId = token.Claims.First(claim => claim.Type == "nguoiDungId").Value;

            var findUser = await _context.NguoiDungs.FindAsync(NguoiDungId);
            if (findUser == null)
            {
                return NotFound(new Response { Status = 404, Message = "Không tìm thấy người dùng" });
            }
            string currentPasswordHashed = GenerateSha256Hash(request.matKhauHienTai, _configuration["AppSettings:Salt"]);
            if (findUser.matKhau != currentPasswordHashed)
                return BadRequest(new Response { Status = 400, Message = "Mật khẩu hiện tại không đúng!" });

            string newPasswordHashed = GenerateSha256Hash(request.matKhauMoi, _configuration["AppSettings:Salt"]);

            try
            {
                findUser.matKhau = newPasswordHashed;
                await _context.SaveChangesAsync();
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = e.ToString() });
            }
            return Ok(new Response { Status = 200, Message = "Cập nhật mật khẩu thành công", Data = null });

        }

        //[Authorize]
        [AllowAnonymous]
        [HttpDelete("xoaNguoiDung/{id}")]
        public async Task<IActionResult> XoaNguoiDung(string id)
        {
            //var NguoiDungRole = "";
            //Request.Headers.TryGetValue("Authorization", out var tokenheaderValue);
            //JwtSecurityToken token = null;
            //try
            //{
            //    token = _jwtAuthenticationManager.GetInFo(tokenheaderValue);
            //}
            //catch (IndexOutOfRangeException e)
            //{
            //    return BadRequest(new Response { Status = 400, Message = "Không xác thực được người dùng" });
            //}
            //NguoiDungRole = token.Claims.First(claim => claim.Type == "vaiTro").Value;
            //if (NguoiDungRole != "admin")
            //    return BadRequest(new Response { Status = 400, Message = "Không có quyền!, vui lòng đăng nhập với tài khoản admin" });
            var user = await _context.NguoiDungs.FindAsync(id);
            if (user != null)
            {
                user.daXoa = true;
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (IndexOutOfRangeException e)
                {
                    return BadRequest(new Response { Status = 400, Message = e.ToString() });
                }
            }
            else
            {
                return BadRequest(new Response { Status = 400, Message = "Không tìm thấy người dùng!" });
            }
            return Ok(new Response { Status = 200, Message = "Xóa thành công" });
        }
        [Authorize]
        [HttpGet("timkiemnguoidung/{name}/{vaitro}/{timkiem}")]
        public async Task<IActionResult> TimKiemNguoiDung(string name, int vaitro, int timkiem)
        {
            List<NguoiDungModel> listcustomer;
            if (vaitro == 2)
            {
                if(timkiem == 1)
                {
                    listcustomer = await _context.NguoiDungs.Include(p => p.VaiTro).Where(s => s.sDT.Contains(name.Trim()) && s.daXoa == false && s.VaiTro.tenVaiTro == "staff").Select(nd => new NguoiDungModel { NguoiDungId = nd.NguoiDungId, tenDangNhap = nd.tenDangNhap, email = nd.email, sDT = nd.sDT, conHoatDong = nd.conHoatDong, diaChi = nd.diaChi, gioiTinh = nd.gioiTinh, tenNguoiDung = nd.tenNguoiDung, VaiTroId = nd.VaiTroId }).ToListAsync();
                    return Ok(new Response { Status = 200, Message = Message.Success, Data = listcustomer });
                }


                if (timkiem == 2)
                {
                    listcustomer = await _context.NguoiDungs.Include(p => p.VaiTro).Where(s => s.tenNguoiDung.Contains(name.Trim()) && s.daXoa == false && s.VaiTro.tenVaiTro == "staff").Select(nd => new NguoiDungModel { NguoiDungId = nd.NguoiDungId, tenDangNhap = nd.tenDangNhap, email = nd.email, sDT = nd.sDT, conHoatDong = nd.conHoatDong, diaChi = nd.diaChi, gioiTinh = nd.gioiTinh, tenNguoiDung = nd.tenNguoiDung, VaiTroId = nd.VaiTroId }).ToListAsync();
                    return Ok(new Response { Status = 200, Message = Message.Success, Data = listcustomer });
                }
                if (timkiem == 3)
                {
                    listcustomer = await _context.NguoiDungs.Include(p => p.VaiTro).Where(s => s.email.Contains(name.Trim()) && s.daXoa == false && s.VaiTro.tenVaiTro == "staff").Select(nd => new NguoiDungModel { NguoiDungId = nd.NguoiDungId, tenDangNhap = nd.tenDangNhap, email = nd.email, sDT = nd.sDT, conHoatDong = nd.conHoatDong, diaChi = nd.diaChi, gioiTinh = nd.gioiTinh, tenNguoiDung = nd.tenNguoiDung, VaiTroId = nd.VaiTroId }).ToListAsync();
                    return Ok(new Response { Status = 200, Message = Message.Success, Data = listcustomer });
                }
            
            }


            if (vaitro == 3)
            {
                if (timkiem == 1)
                {
                    listcustomer = await _context.NguoiDungs.Include(p => p.VaiTro).Where(s => s.sDT.Contains(name.Trim()) && s.daXoa == false && s.VaiTro.tenVaiTro == "customer").Select(nd => new NguoiDungModel { NguoiDungId = nd.NguoiDungId, tenDangNhap = nd.tenDangNhap, email = nd.email, sDT = nd.sDT, conHoatDong = nd.conHoatDong, diaChi = nd.diaChi, gioiTinh = nd.gioiTinh, tenNguoiDung = nd.tenNguoiDung, VaiTroId = nd.VaiTroId }).ToListAsync();
                    return Ok(new Response { Status = 200, Message = Message.Success, Data = listcustomer });
                }


                if (timkiem == 2)
                {
                    listcustomer = await _context.NguoiDungs.Include(p => p.VaiTro).Where(s => s.tenNguoiDung.Contains(name.Trim()) && s.daXoa == false && s.VaiTro.tenVaiTro == "customer").Select(nd => new NguoiDungModel { NguoiDungId = nd.NguoiDungId, tenDangNhap = nd.tenDangNhap, email = nd.email, sDT = nd.sDT, conHoatDong = nd.conHoatDong, diaChi = nd.diaChi, gioiTinh = nd.gioiTinh, tenNguoiDung = nd.tenNguoiDung, VaiTroId = nd.VaiTroId }).ToListAsync();
                    return Ok(new Response { Status = 200, Message = Message.Success, Data = listcustomer });
                }
                if (timkiem == 3)
                {
                    listcustomer = await _context.NguoiDungs.Include(p => p.VaiTro).Where(s => s.email.Contains(name.Trim()) && s.daXoa == false && s.VaiTro.tenVaiTro == "customer").Select(nd => new NguoiDungModel { NguoiDungId = nd.NguoiDungId, tenDangNhap = nd.tenDangNhap, email = nd.email, sDT = nd.sDT, conHoatDong = nd.conHoatDong, diaChi = nd.diaChi, gioiTinh = nd.gioiTinh, tenNguoiDung = nd.tenNguoiDung, VaiTroId = nd.VaiTroId }).ToListAsync();
                    return Ok(new Response { Status = 200, Message = Message.Success, Data = listcustomer });
                }
            }
            return BadRequest(new Response { Status = 400, Message = "Không tìm thấy người dùng!" });
        }
        private String GenerateSha256Hash(string input, string salt)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(input + salt);
            SHA256Managed shaString = new SHA256Managed();
            byte[] hash = shaString.ComputeHash(bytes);
            return BitConverter.ToString(hash);
        }
    }


}
