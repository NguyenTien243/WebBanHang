using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHangAPI.ViewModels
{
    public class RegisterRequest
    {
        public string email { get; set; }
        public string tenDangNhap { get; set; }
        public string tenNguoiDung { get; set; }
        public string diaChi { get; set; }
        public string sDT { get; set; }
        public string matKhau { get; set; }
        public string xacNhanMatKhau { get; set; }
    }
}
