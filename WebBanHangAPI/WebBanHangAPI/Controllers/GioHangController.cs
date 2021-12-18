﻿using Microsoft.AspNetCore.Authorization;
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
    public class GioHangController : ControllerBase
    {
        
        private readonly WebBanHangAPIDBContext _context;
        private readonly IJwtAuthenticationManager _jwtAuthenticationManager;
        public GioHangController(WebBanHangAPIDBContext context, IJwtAuthenticationManager jwtAuthenticationManager)
        {
            _context = context;
            _jwtAuthenticationManager = jwtAuthenticationManager;

        }
        [Authorize]
        [HttpGet("xemgiohang")]
        public async Task<IActionResult> Get()
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
            var cart = await _context.NguoiDungs.Include(p => p.GioHangs).ThenInclude(t => t.SanPham).Where(s => s.NguoiDungId == NguoiDungId).Select(nd => nd.GioHangs.Select(sl => new ItemGioHang() { SanPhamId = sl.SanPhamId, giamGia = sl.SanPham.giamGia,giaTien = sl.SanPham.giaTien,hinhAnh = sl.SanPham.hinhAnh,LoaiSanPhamId = sl.SanPham.LoaiSanPhamId,soLuongConLai = sl.SanPham.soLuongConLai,SoLuongTrongGio = sl.soLuong,tenSP = sl.SanPham.tenSP })).ToListAsync();
            if(cart.Count == 0)
                return Ok(new Response { Status = 200, Message = "Giỏ hàng trống", Data = cart });
            return Ok(new Response { Status = 200, Message = Message.Success, Data = cart });
        }
        [Authorize]
        [HttpPost("themspvaogiohang")]
        public async Task<ActionResult<GioHang>> themSanPhamVaoGio(GioHangModel request)
        {
            var NguoiDungId = "";
            Request.Headers.TryGetValue("Authorization", out var tokenheaderValue);
            JwtSecurityToken token = null;
            try
            {
                token = _jwtAuthenticationManager.GetInFo(tokenheaderValue);
            }
            catch(IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = "Không xác thực được người dùng" });
            }
            NguoiDungId = token.Claims.First(claim => claim.Type == "nguoiDungId").Value;
            
            if (request.SanPhamId == null || request.SanPhamId.Length == 0)
                return BadRequest(new Response { Status = 400, Message = "Thiếu SanPhamId" });
            if (request.soLuong == null || request.soLuong < 0)
                return BadRequest(new Response { Status = 400, Message = "Số lượng phải lớn hơn hoặc bằng 0!" });
            var findSP = await _context.SanPhams.FindAsync(request.SanPhamId);
            if (findSP == null)
                return BadRequest(new Response { Status = 400, Message = "Không tìm thấy sản phẩm" });
            var findUser = await _context.NguoiDungs.FindAsync(NguoiDungId);
            if (findUser == null)
                return BadRequest(new Response { Status = 400, Message = "Không tìm thấy Người dùng" });
            var fingiohang = await _context.GioHangs.Where(gh => gh.SanPhamId == request.SanPhamId && gh.NguoiDungId == NguoiDungId).ToListAsync();
            // nếu sản phẩm có trong giỏ hàng trước rồi thì cập nhật số lượng
            if(fingiohang.Count != 0)
            {
                if(fingiohang[0].soLuong + request.soLuong > findSP.soLuongConLai)
                {
                    return BadRequest(new Response { Status = 400, Message = $"Bạn đã có {fingiohang[0].soLuong} trong giỏ hàng. Không thể thêm vì sẽ vượt số lượng có sẵn của sản phẩm." });
                }    
                fingiohang[0].soLuong += request.soLuong;
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
            else
            {
                if (request.soLuong > findSP.soLuongConLai)
                {
                    return BadRequest(new Response { Status = 400, Message = $"Không đủ số lượng có sẵn, có thể thêm tối đa {findSP.soLuongConLai} vào giỏ hàng." });
                }
                var gioHang = new GioHang();
                gioHang.NguoiDungId = NguoiDungId;
                gioHang.SanPhamId = request.SanPhamId;
                gioHang.soLuong = request.soLuong;
                _context.GioHangs.Add(gioHang);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (IndexOutOfRangeException e)
                {
                    return BadRequest(new Response { Status = 400, Message = e.ToString() });
                }
                return Ok(new Response { Status = 200, Message = "Đã thêm vào giỏ hàng" });
            }
            

            
        }
        [Authorize]
        [HttpPost("giamsoluongtronggiohang")]
        public async Task<ActionResult<GioHang>> giamSanPhamTrongGio(GioHangModel request)
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

            if (request.SanPhamId == null || request.SanPhamId.Length == 0)
                return BadRequest(new Response { Status = 400, Message = "Thiếu SanPhamId" });
            if (request.soLuong == null )
                return BadRequest(new Response { Status = 400, Message = "Số lượng phải lớn hơn hoặc bằng 0!" });
            var findSP = await _context.SanPhams.FindAsync(request.SanPhamId);
            if (findSP == null)
                return BadRequest(new Response { Status = 400, Message = "Không tìm thấy sản phẩm" });
            var findUser = await _context.NguoiDungs.FindAsync(NguoiDungId);
            if (findUser == null)
                return BadRequest(new Response { Status = 400, Message = "Không tìm thấy Người dùng" });
            var fingiohang = await _context.GioHangs.Where(gh => gh.SanPhamId == request.SanPhamId && gh.NguoiDungId == NguoiDungId).ToListAsync();
            // nếu sản phẩm có trong giỏ hàng trước rồi thì cập nhật số lượng
            if (fingiohang.Count != 0)
            {
                if(fingiohang[0].soLuong + request.soLuong < 1)
                    return BadRequest(new Response { Status = 400, Message = "Số lượng không thể nhỏ hơn 1!" });
                if (fingiohang[0].soLuong + request.soLuong > findSP.soLuongConLai )
                {
                    return BadRequest(new Response { Status = 400, Message = $"Bạn đã có {fingiohang[0].soLuong} trong giỏ hàng. Không thể thêm vì sẽ vượt số lượng có sẵn của sản phẩm." });
                }
                fingiohang[0].soLuong += request.soLuong;
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
            
            return BadRequest(new Response { Status = 400, Message = "Không tìm thấy sản phẩm trong giỏ" });

        }


        [Authorize]
        [HttpDelete("xoasanphamtronggiohang/{id}")]
        public async Task<ActionResult<GioHang>> DeleteLoaiSP(string id)
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

            var sptronggiohang = await _context.GioHangs.Where(gh => gh.NguoiDungId == NguoiDungId && gh.SanPhamId == id).FirstOrDefaultAsync();
            if (sptronggiohang != null)
            {
                               
                try
                {
                    _context.GioHangs.Remove(sptronggiohang);
                    await _context.SaveChangesAsync();
                }
                catch (IndexOutOfRangeException e)
                {
                    return BadRequest(new Response { Status = 400, Message = e.ToString() });
                }

            }
            else
            {
                return BadRequest(new Response { Status = 400, Message = "Không tìm thấy sản phẩm trong giỏ!" });
            }


            return Ok(new Response { Status = 200, Message = "Xóa sản phẩm khỏi giỏ hàng thành công" });
        }

    }
}
