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
        public int soLuong { get; set; }
        public int giamGia { get; set; }
        public int giaTien { get; set; }
        public int tongTien { get; set; }
    }
}
