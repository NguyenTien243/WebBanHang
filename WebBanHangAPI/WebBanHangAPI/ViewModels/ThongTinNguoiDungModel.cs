using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHangAPI.ViewModels
{
    public class ThongTinNguoiDungModel
    {
        public string NguoiDungId { get; set; }
        public string tenNguoiDung { get; set; }
        public string email { get; set; }
        public string sDT { get; set; }
        public string diaChi { get; set; }
        public bool gioiTinh { get; set; }
        public string VaiTroId { get; set; }
        public string VaiTro { get; set; }
        public bool conHoatDong { get; set; }
    }
}
