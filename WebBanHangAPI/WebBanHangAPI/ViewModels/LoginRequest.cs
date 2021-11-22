using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHangAPI.ViewModels
{
    public class LoginRequest
    {
        //[Required(ErrorMessage = "Chưa nhập tài khoản")]
        public string tenDangNhap { get; set; }
        //[Required(ErrorMessage = "Chưa nhập mật khẩu")]
        public string matKhau { get; set; }
        
    }
}
