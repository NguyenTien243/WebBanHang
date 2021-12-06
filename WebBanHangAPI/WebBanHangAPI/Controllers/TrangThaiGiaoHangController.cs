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
    public class TrangThaiGiaoHangController : ControllerBase
    {
        
        private readonly IJwtAuthenticationManager _jwtAuthenticationManager;
        private readonly WebBanHangAPIDBContext _context;
        public TrangThaiGiaoHangController(WebBanHangAPIDBContext context, IJwtAuthenticationManager jwtAuthenticationManager)
        {
            _context = context;
            _jwtAuthenticationManager = jwtAuthenticationManager;

        }
        [HttpGet("laydanhsachTrangThaiGiao")]
        public async Task<IActionResult> Get()
        {
            var listTrangThaiGiao = await _context.TrangThaiGiaoHangs.ToListAsync();
            return Ok(new Response { Status = 200, Message = Message.Success, Data = listTrangThaiGiao });
        }
        [Authorize]

        [HttpPost("themTrangThai")]
        public async Task<ActionResult<TrangThaiGiaoHang>> themTrangThau(TrangThaiModel request)
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
            if (request.tenTrangThai == null || request.tenTrangThai.Length == 0)
                return BadRequest(new Response { Status = 400, Message = "tên trạng th bắt buộc" });
            var findTrangThai = await _context.TrangThaiGiaoHangs.Where(u => u.tenTrangThai.Trim() == request.tenTrangThai.Trim()).ToListAsync();
            if (findTrangThai.Count != 0)
                return BadRequest(new Response { Status = 400, Message = "Tên trạng thái giao hàng đã tồn tại, vui lòng chọn tên khác khác" });

            var newTrangThai = new TrangThaiGiaoHang();
            newTrangThai.tenTrangThai = request.tenTrangThai;
            _context.TrangThaiGiaoHangs.Add(newTrangThai);
            try
            {
                await _context.SaveChangesAsync();
                request.TrangThaiGiaoHangId = newTrangThai.TrangThaiGiaoHangId;
            }
            catch(IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = e.ToString() });
            }

            return Ok(new Response { Status = 200, Message = "Inserted", Data = request });
        }
        [Authorize]
        [HttpPut("suatentrangthai")]
        public async Task<IActionResult> editTrangThai([FromBody] TrangThaiModel request)
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
            if (request.TrangThaiGiaoHangId == null || request.TrangThaiGiaoHangId.Length == 0)
                return BadRequest(new Response { Status = 400, Message = "Thiếu TrangThaiGiaoHangId" });
            if (request.tenTrangThai == null || request.tenTrangThai.Length == 0)
                return BadRequest(new Response { Status = 400, Message = "Thiếu tên trạng thái" });
            var findTrangThai = await _context.TrangThaiGiaoHangs.FindAsync(request.TrangThaiGiaoHangId);
            if (findTrangThai == null)
            {
                return NotFound(new Response { Status = 404, Message = "Trạng thái giao hàng không tồn tại" });
            }
            
            var checkname = await _context.TrangThaiGiaoHangs.Where(s => s.tenTrangThai == request.tenTrangThai && s.TrangThaiGiaoHangId != request.TrangThaiGiaoHangId).ToListAsync();

            if (checkname.Count != 0)
            {
                return BadRequest(new Response { Status = 400, Message = "Tên trạng thái giao hàng đã tồn tại, vui lòng thử tên khác" });
            }
            try
            {
                findTrangThai.tenTrangThai = request.tenTrangThai;
                await _context.SaveChangesAsync();
                

            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = e.ToString() });
            }
            return Ok(new Response { Status = 200, Message = "Updated", Data = request });
        }

        //[HttpDelete("deleteVaiTro/{id}")]
        //public async Task<ActionResult<VaiTro>> DeleteLoaiSP(string id)
        //{
          
        //    var vaitro = await _context.VaiTros.FindAsync(id);
        //    if (vaitro != null)
        //    {
        //        var users = await _context.NguoiDungs.Where(s => s.VaiTroId == id).ToListAsync();
        //        foreach( var item in users)
        //        {
        //            item.VaiTroId = null;
        //        }
                
        //        try
        //        {
        //            _context.VaiTros.Remove(vaitro);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (IndexOutOfRangeException e)
        //        {
        //            return BadRequest(new Response { Status = 400, Message = e.ToString() });
        //        }

        //    }
        //    else
        //    {
        //        return BadRequest(new Response { Status = 400, Message = "Không tìm thấy vai trò" });
        //    }    
            

        //    return Ok(new Response { Status = 200, Message = "Deleted" });
        //}

    }
}
