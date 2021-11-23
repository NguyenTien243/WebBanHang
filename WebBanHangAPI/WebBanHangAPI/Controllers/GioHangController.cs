﻿using Microsoft.AspNetCore.Http;
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
    public class GioHangController : ControllerBase
    {
        
        private readonly WebBanHangAPIDBContext _context;
        public GioHangController(WebBanHangAPIDBContext context)
        {
            _context = context;
        }
        [HttpGet("xemgiohang")]
        public async Task<IActionResult> Get()
        {
            var listloaisp = await _context.LoaiSanPhams.ToListAsync();
            return Ok(new Response { Status = 200, Message = Message.Success, Data = listloaisp });
        }

        [HttpPost("themspvaogiohang")]
        public async Task<ActionResult<GioHang>> themSanPhamVaoGio(GioHangModel request)
        {
            if (request.NguoiDungId == null || request.NguoiDungId.Length == 0)
                return BadRequest(new Response { Status = 400, Message = "Thiếu NguoiDungId" });
            if (request.SanPhamId == null || request.SanPhamId.Length == 0)
                return BadRequest(new Response { Status = 400, Message = "Thiếu SanPhamId" });
            if (request.soLuong == null || request.soLuong < 0)
                return BadRequest(new Response { Status = 400, Message = "Số lượng phải lớn hơn hoặc bằng 0!" });
            var findSP = await _context.SanPhams.FindAsync(request.SanPhamId);
            if (findSP == null)
                return BadRequest(new Response { Status = 400, Message = "Không tìm thấy sản phẩm" });
            var findUser = await _context.NguoiDungs.FindAsync(request.NguoiDungId);
            if (findUser == null)
                return BadRequest(new Response { Status = 400, Message = "Không tìm thấy Người dùng" });
            var fingiohang = await _context.GioHangs.Where(gh => gh.SanPhamId == request.SanPhamId && gh.NguoiDungId == request.NguoiDungId).ToListAsync();
            // nếu sản phẩm có trong giỏ hàng trước rồi thì cập nhật số lượng
            if(fingiohang.Count != 0)
            {
                fingiohang[0].soLuong = request.soLuong;
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (IndexOutOfRangeException e)
                {
                    return BadRequest(new Response { Status = 400, Message = e.ToString() });
                }
                return Ok(new Response { Status = 200, Message = "Đã cập nhật số lượng trong giỏ hàng" });
            }    
            var gioHang = new GioHang();
            gioHang.NguoiDungId = request.NguoiDungId;
            gioHang.SanPhamId = request.SanPhamId;
            gioHang.soLuong = request.soLuong;
            _context.GioHangs.Add(gioHang);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch(IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = e.ToString() });
            }

            return Ok(new Response { Status = 200, Message = "Inserted" });
        }

        [HttpPut("suaLoaiSP")]
        public async Task<IActionResult> editLoaiSP([FromBody] EditLoaiSanPhamModel loaisp)
        {
            var findLoaiSP = await _context.LoaiSanPhams.FindAsync(loaisp.LoaiSanPhamId);
            if (findLoaiSP == null)
            {
                return NotFound(new Response { Status = 404, Message = "LoaiSanPhamId không tồn tại" });
            }
            
            var checkname = await _context.LoaiSanPhams.Where(s => s.tenLoaiSP == loaisp.tenLoaiSP).ToListAsync();

            if (checkname.Count != 0)
            {
                return BadRequest(new Response { Status = 400, Message = "Tên Loại Sản Phẩm đã tồn tại, vui lòng thử tên khác" });
            }
            try
            {
                findLoaiSP.tenLoaiSP = loaisp.tenLoaiSP;
                await _context.SaveChangesAsync();
                loaisp.LoaiSanPhamId = findLoaiSP.LoaiSanPhamId;

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
