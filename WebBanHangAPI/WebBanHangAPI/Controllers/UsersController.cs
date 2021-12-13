﻿using AutoMapper;
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
        [AllowAnonymous]

        [HttpGet("GetHeaderData")]
        public ActionResult<string> GetHeaderData(string headerKey)
        {
            
            Request.Headers.TryGetValue("Authorization", out var tokenheaderValue);
            JwtSecurityToken token = _jwtAuthenticationManager.GetInFo(tokenheaderValue);
            var NguoiDungId = token.Claims.First(claim => claim.Type == "nguoiDungId").Value;
            return Ok();
        }
        [HttpPost("dangnhap")]
        [AllowAnonymous]
        public async Task<IActionResult> login([FromBody] LoginRequest request)
        // public IActionResult authenticate([FromBody] LoginRequest user)
        {
            if (string.IsNullOrEmpty(request.tenDangNhap))
                return BadRequest(new Response { Status = 400, Message = "Chưa nhập tên đăng nhập" });
            if (string.IsNullOrEmpty(request.matKhau))
                return BadRequest(new Response { Status = 400, Message = "Chưa nhập mật khẩu" });
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

        [Authorize]
        [HttpPost("dangkyNhanVien")]
        public async Task<IActionResult> registerStaff([FromBody] RegisterRequest request)
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
            NguoiDung nguoiDung = new NguoiDung();
            nguoiDung.tenNguoiDung = request.tenNguoiDung;
            nguoiDung.email = request.email;
            nguoiDung.sDT = request.sDT;
            nguoiDung.diaChi = request.diaChi;
            nguoiDung.tenDangNhap = request.tenDangNhap;
            nguoiDung.matKhau = request.matKhau;
            nguoiDung.VaiTroId = "2";

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
        //[Authorize]
        //[HttpPut("suathongtinUser")]
        //public async Task<IActionResult> PutSP([FromBody] EditSanPhamModel request)
        //{
        //    var NguoiDungRole = "";
        //    Request.Headers.TryGetValue("Authorization", out var tokenheaderValue);
        //    JwtSecurityToken token = null;
        //    try
        //    {
        //        token = _jwtAuthenticationManager.GetInFo(tokenheaderValue);
        //    }
        //    catch (IndexOutOfRangeException e)
        //    {
        //        return BadRequest(new Response { Status = 400, Message = "Không xác thực được người dùng" });
        //    }
        //    NguoiDungRole = token.Claims.First(claim => claim.Type == "vaiTro").Value;
        //    if (NguoiDungRole != "admin")
        //        return BadRequest(new Response { Status = 400, Message = "Không có quyền!, vui lòng đăng nhập với tài khoản admin" });
        //    if (request.tenSP == null || request.tenSP.Length == 0)
        //        return BadRequest(new Response { Status = 400, Message = "Chưa nhập tên sản phẩm" });
        //    if (request.giaTien == null || request.giaTien < 0)
        //        return BadRequest(new Response { Status = 400, Message = "Nhập giá tiền >= 0" });
        //    if (request.giamGia == null || request.giamGia < 0 || request.giamGia > 100)
        //        return BadRequest(new Response { Status = 400, Message = "Nhập giảm giá >= 0 và <= 100 " });
        //    if (request.moTa == null || request.moTa.Length == 0)
        //        return BadRequest(new Response { Status = 400, Message = "Nhập mô tả!" });
        //    if (request.soLuongConLai == null || request.soLuongConLai < 0)
        //        return BadRequest(new Response { Status = 400, Message = "Nhập Số lượng còn lại >= 0" });
        //    if (request.LoaiSanPhamId == null || request.LoaiSanPhamId.Length == 0)
        //        return BadRequest(new Response { Status = 400, Message = "Chưa Có Loại sản phẩm (thiếu loaiSanPhamId)!" });
        //    var findSP = await _context.SanPhams.FindAsync(request.SanPhamId);
        //    if (findSP == null)
        //        return BadRequest(new Response { Status = 400, Message = "Không tìm thấy sản phẩm" });
        //    var findLoaiSP = await _context.LoaiSanPhams.FindAsync(request.LoaiSanPhamId);
        //    if (findLoaiSP == null)
        //    {
        //        return NotFound(new Response { Status = 404, Message = "LoaiSanPhamId không tồn tại" });
        //    }

        //    var checkname = await _context.SanPhams.Where(s => s.tenSP == request.tenSP && s.SanPhamId != request.SanPhamId).ToListAsync();

        //    if (checkname.Count != 0)
        //    {
        //        return BadRequest(new Response { Status = 400, Message = "Tên Sản Phẩm đã tồn tại, vui lòng thử tên khác" });
        //    }
        //    try
        //    {
        //        findSP.tenSP = request.tenSP;
        //        findSP.hinhAnh = request.hinhAnh;
        //        findSP.giaTien = request.giaTien;
        //        findSP.giamGia = request.giamGia;
        //        findSP.moTa = request.moTa;
        //        findSP.soLuongConLai = request.soLuongConLai;
        //        findSP.LoaiSanPhamId = request.LoaiSanPhamId;
        //        await _context.SaveChangesAsync();


        //    }
        //    catch (IndexOutOfRangeException e)
        //    {
        //        return BadRequest(new Response { Status = 400, Message = e.ToString() });
        //    }
        //    return Ok(new Response { Status = 200, Message = "Updated", Data = request });
        //}
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

        [HttpGet("laydanhsachKhachHang")]
        public async Task<IActionResult> GetNguoiDungs()
        {
          
            var listcustomer = await _context.NguoiDungs.Include(p => p.VaiTro).Where(s => s.VaiTro.tenVaiTro == "customer").Select(nd => new NguoiDungModel{NguoiDungId = nd.NguoiDungId, tenDangNhap = nd.tenDangNhap, email = nd.email, sDT = nd.sDT, conHoatDong = nd.conHoatDong, diaChi = nd.diaChi, gioiTinh = nd.gioiTinh, tenNguoiDung = nd.tenNguoiDung,VaiTroId =nd.VaiTroId }).ToListAsync();
            
            return Ok(new Response { Status = 200, Message = Message.Success, Data = listcustomer });
        }

        [HttpGet("laydanhsachNhanVien")]
        public async Task<IActionResult> GetNhanViens()
        {
           
            var listcustomer = await _context.NguoiDungs.Include(p => p.VaiTro).Where(s => s.VaiTro.tenVaiTro == "staff").Select(nd => new NguoiDungModel { NguoiDungId = nd.NguoiDungId, tenDangNhap = nd.tenDangNhap, email = nd.email, sDT = nd.sDT, conHoatDong = nd.conHoatDong, diaChi = nd.diaChi, gioiTinh = nd.gioiTinh, tenNguoiDung = nd.tenNguoiDung, VaiTroId = nd.VaiTroId }).ToListAsync();

            return Ok(new Response { Status = 200, Message = Message.Success, Data = listcustomer });
        }

        [HttpPost("doimatkhau")]
        public async Task<IActionResult> doimatkhau([FromBody] EditPassword request )
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
            if(findUser.matKhau != request.matKhauHienTai)
                return BadRequest(new Response { Status = 400, Message = "Mật khẩu hiện tại không đúng!" });

            try
            {
                findUser.matKhau = request.matKhauMoi;
                await _context.SaveChangesAsync();
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = e.ToString() });
            }
            return Ok(new Response { Status = 200, Message = "Cập nhật mật khẩu thành công", Data = null });
           
        }

    }
}
