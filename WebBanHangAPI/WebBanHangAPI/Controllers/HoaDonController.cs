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
    public class HoaDonController : ControllerBase
    {

        private readonly WebBanHangAPIDBContext _context;
        private readonly IJwtAuthenticationManager _jwtAuthenticationManager;
        public HoaDonController(WebBanHangAPIDBContext context, IJwtAuthenticationManager jwtAuthenticationManager)
        {
            _context = context;
            _jwtAuthenticationManager = jwtAuthenticationManager;

        }
        [Authorize]
        [HttpGet("danhsachhoadonNguoiDung")]
        public async Task<IActionResult> GetAllOfUser()
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
            var findHoaDon = await _context.HoaDons.Where(hd => hd.NguoiDungId == NguoiDungId).Select(sl => new ResponseHoaDon()
            {
                HoaDonId = sl.HoaDonId,
                NguoiDungId = sl.NguoiDungId,
                diaChiGiaoHang = sl.diaChiGiaoHang,
                daThanhToan = sl.daThanhToan,
                ngayXuatDon = sl.ngayXuatDon,
                sdtNguoiNhan = sl.sdtNguoiNhan,
                thanhToanOnline = sl.thanhToanOnline,
                tongHoaDon = sl.tongHoaDon,
                TrangThaiGiaoHangId = sl.TrangThaiGiaoHangId,
                chiTietHD = sl.ChiTietHDs.Select(ct => new ItemChiTietHD()
                {
                    SanPhamId = ct.SanPhamId,
                    tenSP = ct.tenSP,
                    hinhAnh = ct.hinhAnh,
                    giaTien = ct.giaTien,
                    giamGia = ct.giamGia,
                    soLuongDat = ct.soLuongDat,
                    tongTien = ct.tongTien
                }).ToList()
            }).ToListAsync();
            if (findHoaDon.Count == 0)
                return BadRequest(new Response { Status = 400, Message = "Danh sách hóa đơn trống!" });
            return Ok(new Response { Status = 200, Message = Message.Success, Data = findHoaDon });
        }
        [HttpGet("danhsachtatcahoadon")]
        public async Task<IActionResult> GetAll()
        {

            var findHoaDon = await _context.HoaDons.Select(sl => new ResponseHoaDon()
            {
                HoaDonId = sl.HoaDonId,
                NguoiDungId = sl.NguoiDungId,
                diaChiGiaoHang = sl.diaChiGiaoHang,
                daThanhToan = sl.daThanhToan,
                ngayXuatDon = sl.ngayXuatDon,
                sdtNguoiNhan = sl.sdtNguoiNhan,
                thanhToanOnline = sl.thanhToanOnline,
                tongHoaDon = sl.tongHoaDon,
                TrangThaiGiaoHangId = sl.TrangThaiGiaoHangId,
                chiTietHD = sl.ChiTietHDs.Select(ct => new ItemChiTietHD()
                {
                    SanPhamId = ct.SanPhamId,
                    tenSP = ct.tenSP,
                    hinhAnh = ct.hinhAnh,
                    giaTien = ct.giaTien,
                    giamGia = ct.giamGia,
                    soLuongDat = ct.soLuongDat,
                    tongTien = ct.tongTien
                }).ToList()
            }).ToListAsync();
            if (findHoaDon.Count == 0)
                return BadRequest(new Response { Status = 400, Message = "Hóa đơn trống!" });
            return Ok(new Response { Status = 200, Message = Message.Success, Data = findHoaDon });
        }
        [HttpGet("xemchitiethoadon/{hoadonId}")]
        public async Task<IActionResult> Get(string hoadonId)
        {

            var findHoaDon = await _context.HoaDons.Include(hd => hd.ChiTietHDs).Where(hd => hd.HoaDonId == hoadonId).Select(sl => new ResponseHoaDon()
            {
                HoaDonId = sl.HoaDonId,
                NguoiDungId = sl.NguoiDungId,
                diaChiGiaoHang = sl.diaChiGiaoHang,
                daThanhToan = sl.daThanhToan,
                ngayXuatDon = sl.ngayXuatDon,
                sdtNguoiNhan = sl.sdtNguoiNhan,
                thanhToanOnline = sl.thanhToanOnline,
                tongHoaDon = sl.tongHoaDon,
                TrangThaiGiaoHangId = sl.TrangThaiGiaoHangId,
                chiTietHD = sl.ChiTietHDs.Select(ct => new ItemChiTietHD()
                {
                    SanPhamId = ct.SanPhamId,
                    tenSP = ct.tenSP,
                    hinhAnh = ct.hinhAnh,
                    giaTien = ct.giaTien,
                    giamGia = ct.giamGia,
                    soLuongDat = ct.soLuongDat,
                    tongTien = ct.tongTien
                }).ToList()
            }).ToListAsync();
            if (findHoaDon.Count == 0)
                return BadRequest(new Response { Status = 400, Message = "Không tìm thấy hóa đơn!" });
            return Ok(new Response { Status = 200, Message = Message.Success, Data = findHoaDon });
        }
        [Authorize]
        [HttpGet("Xemhoadontheotrangthaiadmin/{TrangThaiGiaoHangId}")]
        public async Task<IActionResult> Getall(string TrangThaiGiaoHangId)
        {
            var NguoiDungId = "";
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
            NguoiDungId = token.Claims.First(claim => claim.Type == "nguoiDungId").Value;
            NguoiDungRole = token.Claims.First(claim => claim.Type == "vaiTro").Value;
            if (NguoiDungRole != "admin" && NguoiDungRole != "staff")
                return BadRequest(new Response { Status = 400, Message = "Không có quyền!, vui lòng đăng nhập với tài khoản admin hoặc nhận viên!" });

            var findHoaDon = await _context.HoaDons.Include(hd => hd.ChiTietHDs).Where(hd => hd.TrangThaiGiaoHangId == TrangThaiGiaoHangId ).Select(sl => new ResponseHoaDon()
            {
                HoaDonId = sl.HoaDonId,
                NguoiDungId = sl.NguoiDungId,
                diaChiGiaoHang = sl.diaChiGiaoHang,
                daThanhToan = sl.daThanhToan,
                ngayXuatDon = sl.ngayXuatDon,
                sdtNguoiNhan = sl.sdtNguoiNhan,
                thanhToanOnline = sl.thanhToanOnline,
                tongHoaDon = sl.tongHoaDon,
                TrangThaiGiaoHangId = sl.TrangThaiGiaoHangId,
                chiTietHD = sl.ChiTietHDs.Select(ct => new ItemChiTietHD()
                {
                    SanPhamId = ct.SanPhamId,
                    tenSP = ct.tenSP,
                    hinhAnh = ct.hinhAnh,
                    giaTien = ct.giaTien,
                    giamGia = ct.giamGia,
                    soLuongDat = ct.soLuongDat,
                    tongTien = ct.tongTien
                }).ToList()
            }).ToListAsync();
            if (findHoaDon.Count == 0)
                return BadRequest(new Response { Status = 400, Message = "Không tìm thấy hóa đơn!" });
            return Ok(new Response { Status = 200, Message = Message.Success, Data = findHoaDon });
        }
        [Authorize]
        [HttpGet("Xemhoadontheotrangthaicuanguoidung/{TrangThaiGiaoHangId}")]
        public async Task<IActionResult> Getallnguoidung(string TrangThaiGiaoHangId)
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
            var findHoaDon = await _context.HoaDons.Include(hd => hd.ChiTietHDs).Where(hd => hd.TrangThaiGiaoHangId == TrangThaiGiaoHangId && hd.NguoiDungId == NguoiDungId).Select(sl => new ResponseHoaDon()
            {
                HoaDonId = sl.HoaDonId,
                NguoiDungId = sl.NguoiDungId,
                diaChiGiaoHang = sl.diaChiGiaoHang,
                daThanhToan = sl.daThanhToan,
                ngayXuatDon = sl.ngayXuatDon,
                sdtNguoiNhan = sl.sdtNguoiNhan,
                thanhToanOnline = sl.thanhToanOnline,
                tongHoaDon = sl.tongHoaDon,
                TrangThaiGiaoHangId = sl.TrangThaiGiaoHangId,
                chiTietHD = sl.ChiTietHDs.Select(ct => new ItemChiTietHD()
                {
                    SanPhamId = ct.SanPhamId,
                    tenSP = ct.tenSP,
                    hinhAnh = ct.hinhAnh,
                    giaTien = ct.giaTien,
                    giamGia = ct.giamGia,
                    soLuongDat = ct.soLuongDat,
                    tongTien = ct.tongTien
                }).ToList()
            }).ToListAsync();
            if (findHoaDon.Count == 0)
                return BadRequest(new Response { Status = 400, Message = "Không tìm thấy hóa đơn!" });
            return Ok(new Response { Status = 200, Message = Message.Success, Data = findHoaDon });
        }


        //tạo hóa đơn
        //1. Add vào hóa đơn
        //2. Thêm sản phẩm vào chi tiết hóa đơn (kiểm tra số lượng đặt, 
        // mã sản phẩm, trừ số lượng còn lại ở sản phẩm)


        private void SetValuesChitietHD(ref ChiTietHD result, string tenSP,
            string hinhAnh, int giamGia, double giaTien, int soLuongDat)
        {
            result.tenSP = tenSP;
            result.hinhAnh = hinhAnh;
            result.giamGia = giamGia;
            result.giaTien = giaTien;
            result.soLuongDat = soLuongDat;
            result.tongTien = (giaTien * (100 - giamGia) / 100) * soLuongDat;
        }
        [Authorize]
        [HttpPost("taohoadon")]
        public async Task<ActionResult<GioHang>> taohoadon([FromBody] RequestOrderModel requestOrder)
        {
            if (string.IsNullOrEmpty(requestOrder.diaChiGiaoHang))
                return BadRequest(new Response { Status = 400, Message = "Thiếu địa chỉ giao hàng!" });
            if (string.IsNullOrEmpty(requestOrder.sdtNguoiNhan))
                return BadRequest(new Response { Status = 400, Message = "Số điện thoại người nhận!" });
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
            double tongHoaDon = 0;

            var findUser = await _context.NguoiDungs.FindAsync(NguoiDungId);
            if (findUser == null)
                return BadRequest(new Response { Status = 400, Message = "Không tìm thấy Người dùng" });
            //1. Add vào hóa đơn
            var hoadon = new HoaDon();
            _context.HoaDons.Add(hoadon);
            hoadon.ngayXuatDon = DateTime.Now;
            hoadon.diaChiGiaoHang = requestOrder.diaChiGiaoHang;
            hoadon.sdtNguoiNhan = requestOrder.sdtNguoiNhan;
            hoadon.NguoiDungId = NguoiDungId;
            hoadon.TrangThaiGiaoHangId = "1";
            if (requestOrder.thanhToanOnline)
            {
                hoadon.thanhToanOnline = true;
                hoadon.daThanhToan = true;
            }

            foreach (var item in requestOrder.danhSachDat)
            {
                var sp = await _context.SanPhams.FindAsync(item.SanPhamId);
                if (sp == null)
                    return NotFound(new Response { Status = 404, Message = $"Không tìm thấy sản phẩm id {item.SanPhamId}" });
                // kiểm tra số lượng đặt có đủ không
                if (sp.soLuongConLai < item.soLuongDat)
                    return BadRequest(new Response { Status = 400, Message = $"Sản phẩm id = {item.SanPhamId} không đủ số lượng để đặt, số lượng tối đa có thể đặt {sp.soLuongConLai}" });
                sp.soLuongConLai -= item.soLuongDat;
                sp.soLuongDaBan += item.soLuongDat;
                ChiTietHD chitietdat = new ChiTietHD();
                chitietdat.HoaDonId = hoadon.HoaDonId;
                chitietdat.SanPhamId = sp.SanPhamId;
                SetValuesChitietHD(ref chitietdat, sp.tenSP, sp.hinhAnh, sp.giamGia, sp.giaTien, item.soLuongDat);
                _context.ChiTietHDs.Add(chitietdat);
                tongHoaDon += chitietdat.tongTien;
            }
            // xoa san pham trong gio hang
            var giohang = await _context.GioHangs.Where(gh => gh.NguoiDungId == NguoiDungId  ).ToListAsync();
            foreach (var item in giohang)
            {
                if (requestOrder.danhSachDat.Any(order => order.SanPhamId == item.SanPhamId))
                    _context.GioHangs.Remove(item);
            }

            hoadon.tongHoaDon = tongHoaDon;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = e.ToString() });
            }
            return Ok(new Response { Status = 200, Message = "Tạo hóa đơn thành công" });
        }

        [Authorize]
        [HttpPut("capnhattrangthaidonAdmin")]
        public async Task<IActionResult> PutTrangThai([FromBody] RequestUpdateStatusOrderModel request)
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
            if (NguoiDungRole != "admin" && NguoiDungRole != "staff")
                return BadRequest(new Response { Status = 400, Message = "Không có quyền!, vui lòng đăng nhập với tài khoản admin hoặc nhận viên!" });
            var findHoaDon = await _context.HoaDons.FindAsync(request.HoaDonId);
            if (findHoaDon == null)
            {
                return NotFound(new Response { Status = 404, Message = "Không tìm thấy hóa đơn" });
            }

            switch (request.TrangThaiGiaoHangId)
            {
                case "3":
                    if (findHoaDon.TrangThaiGiaoHangId != "1")
                        return BadRequest(new Response { Status = 400, Message = "Đơn hàng phải ở trạng thái chờ xác nhận trước!" });
                    break;
                case "4":
                    if (findHoaDon.TrangThaiGiaoHangId != "3")
                        return BadRequest(new Response { Status = 400, Message = "Đơn hàng phải ở trạng thái đang giao trước!" });
                    break;
                case "5":
                    if (findHoaDon.TrangThaiGiaoHangId == "4")
                        return BadRequest(new Response { Status = 400, Message = "Không thể hủy đơn hàng đã giao!" });
                    break;
                default:
                    // code block
                    break;
            }

            try
            {
                findHoaDon.TrangThaiGiaoHangId = request.TrangThaiGiaoHangId;
                await _context.SaveChangesAsync();
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = e.ToString() });
            }
            return Ok(new Response { Status = 200, Message = "Cập nhật trạng thái giao hàng thành công", Data = request });
        }

        [Authorize]
        [HttpPut("nguoidunghuydonhang/{id}")]
        public async Task<IActionResult> PutTrangThaidon(string id)
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
            
            var findHoaDon = await _context.HoaDons.FindAsync(id);
            if (findHoaDon == null)
            {
                return NotFound(new Response { Status = 404, Message = "Không tìm thấy hóa đơn" });
            }
            if (NguoiDungId != findHoaDon.NguoiDungId)
                return BadRequest(new Response { Status = 400, Message = "Hóa đơn yêu cầu hủy không trùng khách hàng Id!" });
            if(findHoaDon.TrangThaiGiaoHangId != "1")
                return BadRequest(new Response { Status = 400, Message = "Chỉ có thể hủy đơn hàng đang chờ xác nhận!" });

            try
            {
                findHoaDon.TrangThaiGiaoHangId = "5";
                await _context.SaveChangesAsync();
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = e.ToString() });
            }
            return Ok(new Response { Status = 200, Message = "Hủy đơn hàng thành công" });
        }
    }
}
