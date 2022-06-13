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
    public class VaiTroController : ControllerBase
    {
        
        private readonly WebBanHangAPIDBContext _context;
        private readonly IJwtAuthenticationManager _jwtAuthenticationManager;

        public VaiTroController(WebBanHangAPIDBContext context, IJwtAuthenticationManager jwtAuthenticationManager)
        {
            _context = context;
            _jwtAuthenticationManager = jwtAuthenticationManager;

        }
        [HttpGet("laydanhsachVaiTro")]
        public async Task<IActionResult> LayDanhSachVaiTro()
        {
            var listvaitro = await _context.VaiTros.ToListAsync();
            return Ok(new Response { Status = 200, Message = Message.Success, Data = listvaitro });
        }
        [Authorize]

        [HttpPost("themVaiTro")]
        public async Task<ActionResult<VaiTro>> ThemVaiTro(VaiTroModel request)
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
            if (request.tenVaiTro == null || request.tenVaiTro.Length == 0)
                return BadRequest(new Response { Status = 400, Message = "tên vai trò bắt buộc" });
            var findvaitro = await _context.VaiTros.Where(u => u.tenVaiTro.Trim() == request.tenVaiTro.Trim()).ToListAsync();
            if (findvaitro.Count != 0)
                return BadRequest(new Response { Status = 400, Message = "Tên vai trò đã tồn tại, vui lòng chọn tên khác" });

            var newvaitro = new VaiTro();
            newvaitro.tenVaiTro = request.tenVaiTro;
            _context.VaiTros.Add(newvaitro);
            try
            {
                await _context.SaveChangesAsync();
                request.VaiTroId = newvaitro.VaiTroId;
            }
            catch(IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = e.ToString() });
            }

            return Ok(new Response { Status = 200, Message = "Inserted", Data = request });
        }
        [Authorize]

        [HttpPut("suatenvaitro")]
        public async Task<IActionResult> SuaVaiTro([FromBody] VaiTroModel request)
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
            if (request.VaiTroId == null || request.VaiTroId.Length == 0)
                return BadRequest(new Response { Status = 400, Message = "Thiếu VaiTroId" });
            if (request.tenVaiTro == null || request.tenVaiTro.Length == 0)
                return BadRequest(new Response { Status = 400, Message = "tên vai trò bắt buộc" });
            var findvaitro = await _context.VaiTros.FindAsync(request.VaiTroId);
            if (findvaitro == null)
            {
                return NotFound(new Response { Status = 404, Message = "Vai trò không tồn tại" });
            }
            
            var checkname = await _context.VaiTros.Where(s => s.tenVaiTro == request.tenVaiTro && s.VaiTroId != request.VaiTroId).ToListAsync();

            if (checkname.Count != 0)
            {
                return BadRequest(new Response { Status = 400, Message = "Tên vai trò đã tồn tại, vui lòng thử tên khác" });
            }
            try
            {
                findvaitro.tenVaiTro = request.tenVaiTro;
                await _context.SaveChangesAsync();
                

            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = e.ToString() });
            }
            return Ok(new Response { Status = 200, Message = "Updated", Data = request });
        }
        [Authorize]
        [HttpDelete("deleteVaiTro/{id}")]
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
            var vaitro = await _context.VaiTros.FindAsync(id);
            if (vaitro != null)
            {
                var users = await _context.NguoiDungs.Where(s => s.VaiTroId == id).ToListAsync();
                foreach( var item in users)
                {
                    item.VaiTroId = null;
                }
                
                try
                {
                    _context.VaiTros.Remove(vaitro);
                    await _context.SaveChangesAsync();
                }
                catch (IndexOutOfRangeException e)
                {
                    return BadRequest(new Response { Status = 400, Message = e.ToString() });
                }

            }
            else
            {
                return BadRequest(new Response { Status = 400, Message = "Không tìm thấy vai trò" });
            }    
            

            return Ok(new Response { Status = 200, Message = "Deleted" });
        }

    }
}
