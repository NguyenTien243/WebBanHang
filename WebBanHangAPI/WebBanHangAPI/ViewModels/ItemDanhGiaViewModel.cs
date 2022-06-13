using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHangAPI.ViewModels
{
    public class ItemDanhGiaViewModel
    {
        public string HoaDonId { get; set; }
        public string SanPhamId { get; set; }
        public string NguoiDungId { get; set; }
        public string tenNguoiDung { get; set; }

        public int SoSao { get; set; }
        public string BinhLuanDanhGia { get; set; }
        public int TrangThaiDanhGia { get; set; }
    }
}
