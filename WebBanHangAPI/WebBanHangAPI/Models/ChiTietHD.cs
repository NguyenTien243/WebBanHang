using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHangAPI.Models
{
    public class ChiTietHD
    {
     
        [Key, Column(Order = 0)]
        public string HoaDonId { get; set; }
        public HoaDon HoaDon { get; set; }
        
        [Key, Column(Order = 1)]
        public string SanPhamId { get; set; }
        public SanPham SanPham { get; set; }
        public string tenSP { get; set; }
        public string hinhAnh { get; set; }
        public double giaTien { get; set; }
        public int giamGia { get; set; }
        public int soLuongDat { get; set; }
        //public string tenLoaiSanPham { get; set; }
        public double tongTien { get; set; }
    }
}
