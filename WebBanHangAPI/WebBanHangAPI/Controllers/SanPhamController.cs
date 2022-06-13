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
    public class SanPhamController : ControllerBase
    {
        
        private readonly WebBanHangAPIDBContext _context;
        private readonly IJwtAuthenticationManager _jwtAuthenticationManager;
        public SanPhamController(WebBanHangAPIDBContext context, IJwtAuthenticationManager jwtAuthenticationManager)
        {
            _context = context;
            _jwtAuthenticationManager = jwtAuthenticationManager;
        }

        [HttpGet("laySanPhamById/{id}")]
        public async Task<IActionResult> LaySPTheoId(string id)
        {
            if (id.Length == 0 || id == null)
                return BadRequest(new Response { Status = 400, Message = "Thiếu Id sản phẩm!" });
            var sp = await _context.SanPhams.FindAsync(id);
            if (sp == null)
                return NotFound(new Response { Status = 404, Message = "Không tìm thấy sản phẩm" });


            LaySanPhamViewModel ketqua = new LaySanPhamViewModel();
            ketqua.SanPhamId = sp.SanPhamId;
            ketqua.tenSP = sp.tenSP;
            ketqua.hinhAnh = sp.hinhAnh;
            ketqua.giaTien = sp.giaTien;
            ketqua.giamGia = sp.giamGia;
            ketqua.moTa = sp.moTa;
            ketqua.soLuongConLai = sp.soLuongConLai;
            ketqua.soLuongDaBan = sp.soLuongDaBan;
            ketqua.LoaiSanPhamId = sp.LoaiSanPhamId;
            ketqua.isDeleted = sp.isDeleted;

            var findChiTietHoaDon = await _context.ChiTietHDs
               .Where(s => s.SanPhamId == sp.SanPhamId && s.TrangThaiDanhGia == 2)
               .Select(s => new ItemDanhGiaViewModel
               {
                   HoaDonId = s.HoaDonId,
                   SanPhamId = s.SanPhamId,
                   NguoiDungId = s.HoaDon.NguoiDungId,
                   tenNguoiDung = s.HoaDon.NguoiDung.tenNguoiDung,
                   BinhLuanDanhGia = s.BinhLuanDanhGia,
                   SoSao = s.SoSao,
                   TrangThaiDanhGia = s.TrangThaiDanhGia
               })
               .ToListAsync();
            if(findChiTietHoaDon.Count != 0)
                ketqua.DanhGiaTrungBinh = findChiTietHoaDon.Sum(x => x.SoSao) / findChiTietHoaDon.Count;
            return Ok(new Response { Status = 200, Message = Message.Success, Data = ketqua });
        }
        //[Authorize]
        [HttpGet("topsanphambanchay/{soluong}")]
        public async Task<IActionResult> TopSPBanChay(int soluong)
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
            var listsp = await _context.SanPhams.Where(s => s.isDeleted == false).Select(sl => new ThongKeSanPhamModel() { 
            SanPhamId = sl.SanPhamId, tenSP = sl.tenSP, hinhAnh = sl.hinhAnh, soLuongDaBan = sl.soLuongDaBan }).OrderByDescending(t => t.soLuongDaBan).Take(soluong).ToListAsync();
            foreach (var item in listsp)
            {
                var findchitiethoadon = await _context.ChiTietHDs
              .Where(s => s.SanPhamId == item.SanPhamId && s.TrangThaiDanhGia == 2)
              .Select(s => new ItemDanhGiaViewModel
              {
                  SoSao = s.SoSao
              })
              .ToListAsync();
                if (findchitiethoadon.Count != 0)
                    item.DanhGiaTrungBinh = findchitiethoadon.Sum(x => x.SoSao) / findchitiethoadon.Count;
            }
            return Ok(new Response { Status = 200, Message = Message.Success, Data = listsp });
        }
        [HttpGet("laydanhsachSP/{tranghientai:int}/{kichthuoctrang:int}")]
        public async Task<IActionResult> LayDanhSachSP(int tranghientai, int kichthuoctrang = 10)
        {
            var query = _context.SanPhams
                .Where(s => s.isDeleted == false)
                //.Include(x => x.ChiTietHDs)
                .Select(sp => new LaySanPhamViewModel { SanPhamId = sp.SanPhamId,
                    tenSP = sp.tenSP,
                    hinhAnh = sp.hinhAnh,
                    giaTien = sp.giaTien,
                    giamGia = sp.giamGia,
                    moTa = sp.moTa,
                    soLuongConLai = sp.soLuongConLai,
                    soLuongDaBan = sp.soLuongDaBan,
                    LoaiSanPhamId = sp.LoaiSanPhamId,
                    isDeleted = sp.isDeleted,
                });
            var tongRecord = await query.CountAsync();
            var listsp = await query.Skip((tranghientai - 1) * kichthuoctrang)
                .Take(kichthuoctrang)
                .ToListAsync();

            foreach (var item in listsp)
            {
                var findchitiethoadon = await _context.ChiTietHDs
              .Where(s => s.SanPhamId == item.SanPhamId && s.TrangThaiDanhGia == 2)
              .Select(s => new ItemDanhGiaViewModel
              {
                  SoSao = s.SoSao
              })
              .ToListAsync();
                if (findchitiethoadon.Count != 0)
                    item.DanhGiaTrungBinh = findchitiethoadon.Sum(x => x.SoSao) / findchitiethoadon.Count;
            }

            PhanTrangSanPhamViewModel phanTrangSanPham = new PhanTrangSanPhamViewModel();
            phanTrangSanPham.DanhSachSanPham = listsp;
            phanTrangSanPham.TrangHienTai = tranghientai;
            phanTrangSanPham.KichThuocTrang = kichthuoctrang;
            if (tongRecord % kichthuoctrang == 0)
                phanTrangSanPham.TongSoTrang = (int)(tongRecord / kichthuoctrang);
            else
                phanTrangSanPham.TongSoTrang = (int)(tongRecord / kichthuoctrang + 1);


           

            return Ok(new Response { Status = 200, Message = Message.Success, Data = phanTrangSanPham });
        }
        [HttpGet("timkiemsanphamtheoten/{name}/{tranghientai:int}/{kichthuoctrang:int}")]
        public async Task<IActionResult> TimKiemSanPhamTheoTen(string name, int tranghientai, int kichthuoctrang = 10)
        {
            var query = _context.SanPhams.Where(s => s.tenSP.Contains(name.Trim()) && s.isDeleted == false)
                .Select(sp => new LaySanPhamViewModel
                {
                    SanPhamId = sp.SanPhamId,
                    tenSP = sp.tenSP,
                    hinhAnh = sp.hinhAnh,
                    giaTien = sp.giaTien,
                    giamGia = sp.giamGia,
                    moTa = sp.moTa,
                    soLuongConLai = sp.soLuongConLai,
                    soLuongDaBan = sp.soLuongDaBan,
                    LoaiSanPhamId = sp.LoaiSanPhamId,
                    isDeleted = sp.isDeleted,
                });
            var tongRecord = await query.CountAsync();
            var findsanphams = await query.ToListAsync();
            foreach (var item in findsanphams)
            {
                var findchitiethoadon = await _context.ChiTietHDs
              .Where(s => s.SanPhamId == item.SanPhamId && s.TrangThaiDanhGia == 2)
              .Select(s => new ItemDanhGiaViewModel
              {
                  SoSao = s.SoSao
              })
              .ToListAsync();
                if (findchitiethoadon.Count != 0)
                    item.DanhGiaTrungBinh = findchitiethoadon.Sum(x => x.SoSao) / findchitiethoadon.Count;
            }

            PhanTrangSanPhamViewModel phanTrangSanPham = new PhanTrangSanPhamViewModel();
            phanTrangSanPham.DanhSachSanPham = findsanphams;
            phanTrangSanPham.TrangHienTai = tranghientai;
            phanTrangSanPham.KichThuocTrang = kichthuoctrang;
            if (tongRecord % kichthuoctrang == 0)
                phanTrangSanPham.TongSoTrang = (int)(tongRecord / kichthuoctrang);
            else
                phanTrangSanPham.TongSoTrang = (int)(tongRecord / kichthuoctrang + 1);
            return Ok(new Response { Status = 200, Message = Message.Success, Data = phanTrangSanPham });
        }
        [HttpGet("laysptheoLoaisanpham/{id}/{tranghientai:int}/{kichthuoctrang:int}")]
        public async Task<IActionResult> LaySPTheoLoaiSP(string id, int tranghientai, int kichthuoctrang = 10)
        {
            var query =  _context.SanPhams.Where(s => s.LoaiSanPhamId == id && s.isDeleted == false)
                .Select(sp => new LaySanPhamViewModel
                {
                    SanPhamId = sp.SanPhamId,
                    tenSP = sp.tenSP,
                    hinhAnh = sp.hinhAnh,
                    giaTien = sp.giaTien,
                    giamGia = sp.giamGia,
                    moTa = sp.moTa,
                    soLuongConLai = sp.soLuongConLai,
                    soLuongDaBan = sp.soLuongDaBan,
                    LoaiSanPhamId = sp.LoaiSanPhamId,
                    isDeleted = sp.isDeleted,
                });

            var tongRecord = await query.CountAsync();
            var findsanphams = await query.ToListAsync();
            foreach (var item in findsanphams)
            {
                var findchitiethoadon = await _context.ChiTietHDs
              .Where(s => s.SanPhamId == item.SanPhamId && s.TrangThaiDanhGia == 2)
              .Select(s => new ItemDanhGiaViewModel
              {
                  SoSao = s.SoSao
              })
              .ToListAsync();
                if (findchitiethoadon.Count != 0)
                    item.DanhGiaTrungBinh = findchitiethoadon.Sum(x => x.SoSao) / findchitiethoadon.Count;
            }

            PhanTrangSanPhamViewModel phanTrangSanPham = new PhanTrangSanPhamViewModel();
            phanTrangSanPham.DanhSachSanPham = findsanphams;
            phanTrangSanPham.TrangHienTai = tranghientai;
            phanTrangSanPham.KichThuocTrang = kichthuoctrang;
            if (tongRecord % kichthuoctrang == 0)
                phanTrangSanPham.TongSoTrang = (int)(tongRecord / kichthuoctrang);
            else
                phanTrangSanPham.TongSoTrang = (int)(tongRecord / kichthuoctrang + 1);
            return Ok(new Response { Status = 200, Message = Message.Success, Data = phanTrangSanPham });
        }
        [Authorize]

        [HttpPost("themSP")]
        public async Task<ActionResult<SanPham>> ThemSP([FromBody] CreateSanPhamModel request)
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
            if(NguoiDungRole!= "admin")
                return BadRequest(new Response { Status = 400, Message = "Không có quyền!, vui lòng đăng nhập với tài khoản admin" });
            if (request.tenSP == null || request.tenSP.Length == 0)
                return BadRequest(new Response { Status = 400, Message = "Chưa nhập tên sản phẩm" });
            if (request.giaTien == null || request.giaTien < 0)
                return BadRequest(new Response { Status = 400, Message = "Nhập giá tiền >= 0" });
            if (request.giamGia == null || request.giamGia < 0 || request.giamGia > 100)
                return BadRequest(new Response { Status = 400, Message = "Nhập giảm giá >= 0 và <= 100 " });
            if (request.moTa == null || request.moTa.Length == 0)
                return BadRequest(new Response { Status = 400, Message = "Nhập mô tả!" });
            if (request.soLuongConLai == null || request.soLuongConLai < 0)
                return BadRequest(new Response { Status = 400, Message = "Nhập Số lượng còn lại >= 0" });
            if (request.LoaiSanPhamId == null || request.LoaiSanPhamId.Length == 0)
                return BadRequest(new Response { Status = 400, Message = "Chưa Có Loại sản phẩm (thiếu loaiSanPhamId)!" });
            var findloaisanpham = await _context.LoaiSanPhams.Where(s => s.LoaiSanPhamId == request.LoaiSanPhamId ).ToListAsync();
            if (findloaisanpham.Count == 0)
                return BadRequest(new Response { Status = 400, Message = "Không tìm thấy loại sản phẩm" });
            var sanphamnew = new SanPham();
            sanphamnew.tenSP = request.tenSP;
            sanphamnew.hinhAnh = request.hinhAnh;
            sanphamnew.giaTien = request.giaTien;
            sanphamnew.giamGia = request.giamGia;
            sanphamnew.moTa = request.moTa;
            sanphamnew.soLuongConLai = request.soLuongConLai;
            sanphamnew.LoaiSanPhamId = request.LoaiSanPhamId;
            _context.SanPhams.Add(sanphamnew);
            try
            {
                await _context.SaveChangesAsync();
                request.LoaiSanPhamId = sanphamnew.LoaiSanPhamId;
            }
            catch(IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = e.ToString() });
            }

            return Ok(new Response { Status = 200, Message = "Inserted", Data = request });
        }
        [Authorize]
        [HttpPut("suaSP")]
        public async Task<IActionResult> CapNhatSP([FromBody] EditSanPhamModel request)
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
            if (request.tenSP == null || request.tenSP.Length == 0)
                return BadRequest(new Response { Status = 400, Message = "Chưa nhập tên sản phẩm" });
            if (request.giaTien == null || request.giaTien < 0)
                return BadRequest(new Response { Status = 400, Message = "Nhập giá tiền >= 0" });
            if (request.giamGia == null || request.giamGia < 0 || request.giamGia > 100)
                return BadRequest(new Response { Status = 400, Message = "Nhập giảm giá >= 0 và <= 100 " });
            if (request.moTa == null || request.moTa.Length == 0)
                return BadRequest(new Response { Status = 400, Message = "Nhập mô tả!" });
            if (request.soLuongConLai == null || request.soLuongConLai < 0)
                return BadRequest(new Response { Status = 400, Message = "Nhập Số lượng còn lại >= 0" });
            if (request.LoaiSanPhamId == null || request.LoaiSanPhamId.Length == 0)
                return BadRequest(new Response { Status = 400, Message = "Chưa Có Loại sản phẩm (thiếu loaiSanPhamId)!" });
            var findSP = await _context.SanPhams.FindAsync(request.SanPhamId);
            if (findSP == null)
                return BadRequest(new Response { Status = 400, Message = "Không tìm thấy sản phẩm" });
            var findLoaiSP = await _context.LoaiSanPhams.FindAsync(request.LoaiSanPhamId);
            if (findLoaiSP == null)
            {
                return NotFound(new Response { Status = 404, Message = "LoaiSanPhamId không tồn tại" });
            }

            var checkname = await _context.SanPhams.Where(s => s.tenSP == request.tenSP && s.SanPhamId != request.SanPhamId).ToListAsync();

            if (checkname.Count != 0)
            {
                return BadRequest(new Response { Status = 400, Message = "Tên Sản Phẩm đã tồn tại, vui lòng thử tên khác" });
            }
            try
            {
                findSP.tenSP = request.tenSP;
                findSP.hinhAnh = request.hinhAnh;
                findSP.giaTien = request.giaTien;
                findSP.giamGia = request.giamGia;
                findSP.moTa = request.moTa;
                findSP.soLuongConLai = request.soLuongConLai;
                findSP.LoaiSanPhamId = request.LoaiSanPhamId;
                await _context.SaveChangesAsync();
                

            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = e.ToString() });
            }
            return Ok(new Response { Status = 200, Message = "Updated", Data = request });
        }
        [Authorize]
        [HttpDelete("deleteSP/{id}")]
        public async Task<ActionResult<SanPham>> XoaSP(string id)
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
            var SP = await _context.SanPhams.FindAsync(id);
            if (SP != null)
            {
                SP.isDeleted = true;
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
                return BadRequest(new Response { Status = 400, Message ="Không tìm thấy Sản phẩm" });
            }    
            return Ok(new Response { Status = 200, Message = "Deleted" });
        }
        [Authorize]
        [HttpPost("danhgiaSP")]
        public async Task<ActionResult> DanhGiaSP([FromBody] DanhGiaSanPhamViewModel request)
        {

            if (String.IsNullOrEmpty(request.HoaDonId))
                return BadRequest(new Response { Status = 400, Message = "Thiếu HoaDonId" });
            if (String.IsNullOrEmpty(request.SanPhamId))
                return BadRequest(new Response { Status = 400, Message = "Thiếu SanPhamId" });
            if (request.SoSao < 1 || request.SoSao > 5)
                return BadRequest(new Response { Status = 400, Message = "Số sao đánh giá phải từ 1 đến 5" });
            
            var findChiTietHoaDon = await _context.ChiTietHDs
                .Where(s => s.HoaDonId == request.HoaDonId && s.SanPhamId == request.SanPhamId)
                .Include(x => x.HoaDon)
                .FirstOrDefaultAsync();
            if(findChiTietHoaDon == null)
                return BadRequest(new Response { Status = 400, Message = "Không tìm thấy sản phẩm trong hóa đơn tương ứng" });

            if(findChiTietHoaDon.TrangThaiDanhGia != 0)
                return BadRequest(new Response { Status = 400, Message = "Đã đánh giá sản phẩm trước đó ở đơn mua hàng này!" });
            if (findChiTietHoaDon.HoaDon.TrangThaiGiaoHangId != "4")
                return BadRequest(new Response { Status = 400, Message = "Không thể đánh giá vì đơn hàng không ở trạng thái đã giao!" });

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

            if(NguoiDungId != findChiTietHoaDon.HoaDon.NguoiDungId)
                return BadRequest(new Response { Status = 400, Message = "Bạn chưa mua sản phẩm nên không thể đánh giá" });

            if (request.BinhLuanDanhGia != null)
                findChiTietHoaDon.BinhLuanDanhGia = request.BinhLuanDanhGia;

            findChiTietHoaDon.SoSao = request.SoSao;
            findChiTietHoaDon.TrangThaiDanhGia = 1; // 1 tương ứng với đã đánh giá và đang đợi admin duyệt --> trạng thái = 2 là đã được duyệt, trạng thái = 3 là bị hủy đánh giá
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = e.ToString() });
            }

            return Ok(new Response { Status = 200, Message = "Đánh giá sản phẩm thành công", Data = request });
        }

        [Authorize]
        [HttpPost("capnhattrangthaidanhgiaSP")]
        public async Task<ActionResult> CapNhatTrangThaiDanhGiaSP([FromBody] CapNhatDanhGiaViewModel request)
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

            if (String.IsNullOrEmpty(request.HoaDonId))
                return BadRequest(new Response { Status = 400, Message = "Thiếu HoaDonId" });
            if (String.IsNullOrEmpty(request.SanPhamId))
                return BadRequest(new Response { Status = 400, Message = "Thiếu SanPhamId" });
            if (request.TrangThai < 0 || request.TrangThai > 3)
                return BadRequest(new Response { Status = 400, Message = "TrangThai phải từ 0 đến 3" });

            var findChiTietHoaDon = await _context.ChiTietHDs
                .Where(s => s.HoaDonId == request.HoaDonId && s.SanPhamId == request.SanPhamId)
                .Include(x => x.HoaDon)
                .FirstOrDefaultAsync();
            if (findChiTietHoaDon == null)
                return BadRequest(new Response { Status = 400, Message = "Không tìm thấy sản phẩm trong hóa đơn tương ứng" });

            if (findChiTietHoaDon.HoaDon.TrangThaiGiaoHangId != "4")
                return BadRequest(new Response { Status = 400, Message = "Không thể đánh giá vì đơn hàng không ở trạng thái đã giao!" });


            findChiTietHoaDon.TrangThaiDanhGia = request.TrangThai; // 1 tương ứng với đã đánh giá và đang đợi admin duyệt --> trạng thái = 2 là đã được duyệt, trạng thái = 3 là bị hủy đánh giá

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = e.ToString() });
            }

            return Ok(new Response { Status = 200, Message = "Cập nhật trạng thái đánh giá thành công", Data = request });
        }

        [Authorize]
        [HttpGet("laydanhsachdanhgiaSP/{trangthai}")]
        public async Task<ActionResult> DanhSachDanhGiaSP(int trangthai)
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

            if (trangthai < 0 || trangthai > 3)
                return BadRequest(new Response { Status = 400, Message = "TrangThai phải từ 0 đến 3" });

            var findChiTietHoaDon = await _context.ChiTietHDs
                .Where(s => s.TrangThaiDanhGia == trangthai)
                .Select( s => new ItemDanhGiaViewModel { HoaDonId = s.HoaDonId,
                                                         SanPhamId = s.SanPhamId,
                                                         NguoiDungId = s.HoaDon.NguoiDungId,
                                                         tenNguoiDung = s.HoaDon.NguoiDung.tenNguoiDung,
                                                         BinhLuanDanhGia = s.BinhLuanDanhGia,
                                                         SoSao = s.SoSao,
                                                         TrangThaiDanhGia = s.TrangThaiDanhGia})
                .ToListAsync();
           
            

            return Ok(new Response { Status = 200, Message = "Thành công", Data = findChiTietHoaDon });
        }

        [HttpGet("laydanhsachdanhgiatheoSP/{SanPhamId}")]
        public async Task<ActionResult> LayDanhSachDanhGiaSP(string SanPhamId)
        {

            if (String.IsNullOrEmpty(SanPhamId))
                return BadRequest(new Response { Status = 400, Message = "Thiếu SanPhamId" });

            var findChiTietHoaDon = await _context.ChiTietHDs
                .Where(s => s.SanPhamId == SanPhamId && s.TrangThaiDanhGia == 2)
                .Select(s => new ItemDanhGiaViewModel
                {
                    HoaDonId = s.HoaDonId,
                    SanPhamId = s.SanPhamId,
                    NguoiDungId = s.HoaDon.NguoiDungId,
                    tenNguoiDung = s.HoaDon.NguoiDung.tenNguoiDung,
                    BinhLuanDanhGia = s.BinhLuanDanhGia,
                    SoSao = s.SoSao,
                    TrangThaiDanhGia = s.TrangThaiDanhGia
                })
                .ToListAsync();

            return Ok(new Response { Status = 200, Message = "Thành công", Data = findChiTietHoaDon });
        }
    }
}
