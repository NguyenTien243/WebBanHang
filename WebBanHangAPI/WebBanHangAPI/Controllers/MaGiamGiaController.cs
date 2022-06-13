using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using WebBanHangAPI.Common;
using WebBanHangAPI.IServices;
using WebBanHangAPI.Models;
using WebBanHangAPI.ViewModels;

namespace WebBanHangAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaGiamGiaController : ControllerBase
    {
        private readonly WebBanHangAPIDBContext _context;
        private readonly IJwtAuthenticationManager _jwtAuthenticationManager;
        public MaGiamGiaController(WebBanHangAPIDBContext context, IJwtAuthenticationManager jwtAuthenticationManager)
        {
            _context = context;
            _jwtAuthenticationManager = jwtAuthenticationManager;

        }
        

        [HttpGet("laydanhsachMaGiamGia")]
        public async Task<IActionResult> LayDanhSachMaGiamGia()
        {
            var listMaGiamGia= await _context.MaGiamGias.ToListAsync();
            return Ok(new Response { Status = 200, Message = Message.Success, Data = listMaGiamGia });
        }
        [Authorize]
        [HttpPost("themMaGiamGia")]
        public async Task<ActionResult<MaGiamGia>> ThemMaGiamGia(MaGiamGiaModel request)
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
            
            var findIDMaGiamGia = await _context.MaGiamGias.Where(m => m.MaGiamGiaId.Trim() == request.MaGiamGiaId.Trim()).ToListAsync();
            if (findIDMaGiamGia.Count != 0)
                return BadRequest(new Response { Status = 400, Message = "Code đã tồn tại, vui lòng chọn code khác" });

            var newMaGiamGia = new MaGiamGia();
            newMaGiamGia.MaGiamGiaId = request.MaGiamGiaId;
            newMaGiamGia.TenMaGiamGia = request.TenMaGiamGia;
            newMaGiamGia.NoiDungChiTiet = request.NoiDungChiTiet;
            newMaGiamGia.NgayHetHang = request.NgayHetHang;
            newMaGiamGia.NgayBatDau = request.NgayBatDau;
            newMaGiamGia.GiamToiDa = request.GiamToiDa;
            newMaGiamGia.DonToiThieu = request.DonToiThieu;
            newMaGiamGia.KieuGiam = request.KieuGiam;
            newMaGiamGia.GiamGia = request.GiamGia;
            newMaGiamGia.SoLuongSuDUng = request.SoLuongSuDUng;
            _context.MaGiamGias.Add(newMaGiamGia);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = e.ToString() });
            }

            return Ok(new Response { Status = 200, Message = "Inserted", Data = request });
        }

        [Authorize]
        [HttpPost("themMaGiamNgDung")]
        public async Task<ActionResult<MaGiamGiaCuaNgDung>> ThemSPVaoGioHang(ThemMaGiamNgDung request)
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

            var findMaGiam = await _context.MaGiamGias.FindAsync(request.MaGiamGiaId);
            if (findMaGiam == null)
                return BadRequest(new Response { Status = 400, Message = "Không tìm thấy mã giảm" });
            var findUser = await _context.NguoiDungs.FindAsync(NguoiDungId);
            if (findUser == null)
                return BadRequest(new Response { Status = 400, Message = "Không tìm thấy Người dùng" });
            var findMaGiamDaThemChua = await _context.MaGiamGiaCuaNgDungs.Where(mgnd => mgnd.MaGiamGiaId == request.MaGiamGiaId && mgnd.NguoiDungId == NguoiDungId).ToListAsync();
            // nếu ng dùng đã thêm mã rồi thì thông báo lỗi
            if (findMaGiamDaThemChua.Count != 0)
            {
                return BadRequest(new Response { Status = 400, Message = $"Bạn đã có mã trong danh sách mã giảm giá của bạn rồi. Không thể thêm được nữa." });
            }
            else
            {         
                var maGiam = new MaGiamGiaCuaNgDung();
                maGiam.NguoiDungId = NguoiDungId;
                maGiam.MaGiamGiaId = request.MaGiamGiaId;
                maGiam.DaSuDung =false;
                _context.MaGiamGiaCuaNgDungs.Add(maGiam);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (IndexOutOfRangeException e)
                {
                    return BadRequest(new Response { Status = 400, Message = e.ToString() });
                }
                return Ok(new Response { Status = 200, Message = "Đã thêm vào mã vào danh sách mã của bạn." });
            }
        }
        [Authorize]
        [HttpGet("xemdanhsachMaGiamGiaCuaNgDung")]
        public async Task<IActionResult> XemMaGiamGiaCuaNgDUng()
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
            var listMaGiamNgDung = await _context.NguoiDungs
                .Include(p => p.MaGiamGiaCuaNgDungs)
                .ThenInclude(t => t.MaGiamGia)
                .Where(s => s.NguoiDungId == NguoiDungId)
                .Select(nd => nd.MaGiamGiaCuaNgDungs
                    .Select(sl => new DanhSachMaGiamGiaNgDungModel() 
                        { 
                            MaGiamGiaId = sl.MaGiamGia.MaGiamGiaId,
                            TenMaGiamGia = sl.MaGiamGia.TenMaGiamGia,
                            DonToiThieu = sl.MaGiamGia.DonToiThieu,
                            GiamGia = sl.MaGiamGia.GiamGia,
                            GiamToiDa = sl.MaGiamGia.GiamToiDa,
                            KieuGiam = sl.MaGiamGia.KieuGiam,
                            NgayBatDau = sl.MaGiamGia.NgayBatDau,
                            NgayHetHang = sl.MaGiamGia.NgayHetHang,
                            NoiDungChiTiet = sl.MaGiamGia.NoiDungChiTiet,
                            SoLuongSuDUng = sl.MaGiamGia.SoLuongSuDUng,
                            DaSuDung = sl.DaSuDung
                        }))
                    .ToListAsync();
            if (listMaGiamNgDung.Count == 0)
                return Ok(new Response { Status = 200, Message = "Không có mã nào trong danh sách", Data = listMaGiamNgDung });
            return Ok(new Response { Status = 200, Message = Message.Success, Data = listMaGiamNgDung });
        }

        [Authorize]
        [HttpPut("capnhatMaGiamGia")]
        public async Task<ActionResult<MaGiamGia>> CapNhapMaGiamGia(MaGiamGiaModel request)
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

            var findIDMaGiamGia = await _context.MaGiamGias.Where(m => m.MaGiamGiaId.Trim() == request.MaGiamGiaId.Trim()).ToListAsync();
            if (findIDMaGiamGia.Count != 0)
            {
                //findIDMaGiamGia[0].MaGiamGiaId = request.MaGiamGiaId;
                findIDMaGiamGia[0].TenMaGiamGia = request.TenMaGiamGia;
                findIDMaGiamGia[0].NoiDungChiTiet = request.NoiDungChiTiet;
                findIDMaGiamGia[0].NgayBatDau = request.NgayBatDau;
                findIDMaGiamGia[0].NgayHetHang = request.NgayHetHang;
                findIDMaGiamGia[0].KieuGiam = request.KieuGiam;
                findIDMaGiamGia[0].TenMaGiamGia = request.TenMaGiamGia;
                findIDMaGiamGia[0].SoLuongSuDUng = request.SoLuongSuDUng;
                findIDMaGiamGia[0].GiamToiDa = request.GiamToiDa;
                findIDMaGiamGia[0].GiamGia = request.GiamGia;
                findIDMaGiamGia[0].DonToiThieu = request.DonToiThieu;
            }    
            else
                return BadRequest(new Response { Status = 400, Message = "Mã giảm giá không tìm thấy để sửa" });
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = e.ToString() });
            }
            return Ok(new Response { Status = 200, Message = "Đã cập nhật mã giảm giá thành công",  Data= request });           

        }

        [Authorize]
        [HttpDelete("deleteMaGiamGia/{id}")]
        public async Task<ActionResult<VaiTro>> XoaVaiTro(string id)
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
            var findMaGiam = await _context.MaGiamGias.FindAsync(id);
            if (findMaGiam != null)
            {
                var lishMaGiamNgDung = await _context.MaGiamGiaCuaNgDungs.Where(s => s.MaGiamGiaId == id).ToListAsync();
                foreach (var item in lishMaGiamNgDung)
                {
                    _context.MaGiamGiaCuaNgDungs.Remove(item);
                }
                try
                {
                    _context.MaGiamGias.Remove(findMaGiam);
                    await _context.SaveChangesAsync();
                }
                catch (IndexOutOfRangeException e)
                {
                    return BadRequest(new Response { Status = 400, Message = e.ToString() });
                }

            }
            else
            {
                return BadRequest(new Response { Status = 400, Message = "Không tìm thấy mã giảm giá" });
            }


            return Ok(new Response { Status = 200, Message = "Deleted" });
        }
    }
}
