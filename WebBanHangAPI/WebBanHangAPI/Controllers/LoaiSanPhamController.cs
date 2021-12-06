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
    public class LoaiSanPhamController : ControllerBase
    {
        
        private readonly WebBanHangAPIDBContext _context;
        public LoaiSanPhamController(WebBanHangAPIDBContext context)
        {
            _context = context;
        }
        [HttpGet("laydanhsachLoaiSP")]
        public async Task<IActionResult> Get()
        {
            var listloaisp = await _context.LoaiSanPhams.ToListAsync();
            return Ok(new Response { Status = 200, Message = Message.Success, Data = listloaisp });
        }
        [HttpGet("layLoaiSPById/{id}")]
        public async Task<IActionResult> Getbyid(string id)
        {
            if(id.Length==0 || id == null)
                return BadRequest(new Response { Status = 400, Message = "Thiếu Id loại sản phẩm!" });
            var loaisp = await _context.LoaiSanPhams.FindAsync(id);
            if(loaisp == null)
                return NotFound(new Response { Status = 404, Message = "Không tìm thấy loại sản phẩm" });
            return Ok(new Response { Status = 200, Message = Message.Success, Data = loaisp });
        }

        [HttpPost("themloaiSP")]
        public async Task<ActionResult<LoaiSanPham>> themLoaiSanPham(LoaiSanPhamModel loaiSanPham)
        {
            if (loaiSanPham.tenLoaiSP == null || loaiSanPham.tenLoaiSP.Length ==0)
                return BadRequest(new Response { Status = 400, Message = "Thiếu tenLoaiSP" });
            var checkname = await _context.LoaiSanPhams.Where(s => s.tenLoaiSP == loaiSanPham.tenLoaiSP).ToListAsync();
            if(checkname.Count != 0)
                return BadRequest(new Response { Status = 400, Message = "Tên Loại Sản Phẩm đã tồn tại, vui lòng thử tên khác" });


            var newLoaiSP = new LoaiSanPham();
            newLoaiSP.tenLoaiSP = loaiSanPham.tenLoaiSP;
            newLoaiSP.hinhAnh = loaiSanPham.hinhAnh;
            _context.LoaiSanPhams.Add(newLoaiSP);
            try
            {
                await _context.SaveChangesAsync();
                loaiSanPham.hinhAnh = newLoaiSP.hinhAnh;
            }
            catch(IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = e.ToString() });
            }

            return Ok(new Response { Status = 200, Message = "Inserted", Data = new EditLoaiSanPhamModel() { LoaiSanPhamId = newLoaiSP.LoaiSanPhamId, hinhAnh = newLoaiSP.hinhAnh, tenLoaiSP = newLoaiSP.tenLoaiSP } });
        }

        [HttpPut("suaLoaiSP")]
        public async Task<IActionResult> editLoaiSP([FromBody] EditLoaiSanPhamModel loaisp)
        {
            if(loaisp.LoaiSanPhamId == null || loaisp.LoaiSanPhamId.Length == 0)
                return BadRequest(new Response { Status = 400, Message = "Thiếu tenLoaiSP" });
            if (loaisp.tenLoaiSP == null || loaisp.tenLoaiSP.Length == 0)
                return BadRequest(new Response { Status = 400, Message = "Thiếu tenLoaiSP" });
            
            
            var findLoaiSP = await _context.LoaiSanPhams.FindAsync(loaisp.LoaiSanPhamId);
            if (findLoaiSP == null)
            {
                return NotFound(new Response { Status = 404, Message = "LoaiSanPhamId không tồn tại" });
            }
            
            var checkname = await _context.LoaiSanPhams.Where(s => s.tenLoaiSP == loaisp.tenLoaiSP && s.LoaiSanPhamId != loaisp.LoaiSanPhamId).ToListAsync();

            if (checkname.Count != 0)
            {
                return BadRequest(new Response { Status = 400, Message = "Tên Loại Sản Phẩm đã tồn tại, vui lòng thử tên khác" });
            }
            try
            {
                findLoaiSP.tenLoaiSP = loaisp.tenLoaiSP;
                findLoaiSP.hinhAnh = loaisp.hinhAnh;
                await _context.SaveChangesAsync();
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = e.ToString() });
            }
            return Ok(new Response { Status = 200, Message = "Updated", Data = loaisp });
        }

        [HttpDelete("deleteLoaiSP/{id}")]
        public async Task<ActionResult<LoaiSanPham>> DeleteLoaiSP(string id)
        {
          
            var loaiSP = await _context.LoaiSanPhams.FindAsync(id);
            if (loaiSP != null)
            {
                var sanphams = await _context.SanPhams.Where(s => s.LoaiSanPhamId == id).ToListAsync();
                foreach( var item in sanphams)
                {
                    item.LoaiSanPhamId = null;
                }
                
                try
                {
                    _context.LoaiSanPhams.Remove(loaiSP);
                    await _context.SaveChangesAsync();
                }
                catch (IndexOutOfRangeException e)
                {
                    return BadRequest(new Response { Status = 400, Message = e.ToString() });
                }

            }
            

            return Ok(new Response { Status = 200, Message = "Deleted" });
        }

    }
}
