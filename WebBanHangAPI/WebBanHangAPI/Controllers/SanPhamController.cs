using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBanHangAPI.Common;
using WebBanHangAPI.Models;
using WebBanHangAPI.ViewModels;

namespace WebBanHangAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SanPhamController : ControllerBase
    {
        
        private readonly WebBanHangAPIDBContext _context;
        public SanPhamController(WebBanHangAPIDBContext context)
        {
            _context = context;
        }
        [HttpGet("laydanhsachSP")]
        public async Task<IActionResult> Get()
        {
            var listsp = await _context.SanPhams.Where(s => s.isDeleted == false).ToListAsync();
            return Ok(new Response { Status = 200, Message = Message.Success, Data = listsp });
        }
        [HttpGet("laysptheoLoaisanpham/{id}")]
        public async Task<IActionResult> GetSP(string id)
        {
            var findsanphams = await _context.SanPhams.Where(s => s.LoaiSanPhamId == id && s.isDeleted == false).ToListAsync();
            return Ok(new Response { Status = 200, Message = Message.Success, Data = findsanphams });
        }

        [HttpPost("themSP")]
        public async Task<ActionResult<SanPham>> themSanPham([FromBody] CreateSanPhamModel request)
        {
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

        [HttpPut("suaSP")]
        public async Task<IActionResult> PutSP([FromBody] EditSanPhamModel request)
        {
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

        [HttpDelete("deleteSP/{id}")]
        public async Task<ActionResult<SanPham>> DeleteSP(string id)
        {
          
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

    }
}
