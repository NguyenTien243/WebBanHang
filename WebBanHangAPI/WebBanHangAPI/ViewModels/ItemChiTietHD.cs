using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHangAPI.ViewModels
{
    public class ItemChiTietHD
    {
        public string SanPhamId { get; set; }
        public string tenSP { get; set; }
        public string hinhAnh { get; set; }
        public double giaTien { get; set; }
        public int giamGia { get; set; }
        public int soLuongDat { get; set; }
        //public string tenLoaiSanPham { get; set; }
        public double tongTien { get; set; }
    }
}
