using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHangAPI.ViewModels
{
    public class ItemGioHang
    {
        public string SanPhamId { get; set; }
        public string tenSP { get; set; }
        public string hinhAnh { get; set; }
        public double giaTien { get; set; }
        public int giamGia { get; set; }
        public int soLuongConLai { get; set; }
        public string LoaiSanPhamId { get; set; }
        public int SoLuongTrongGio { get; set; }
    }
}
