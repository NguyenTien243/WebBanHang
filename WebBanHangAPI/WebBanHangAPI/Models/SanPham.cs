using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHangAPI.Models
{
    public class SanPham
    {
        public string SanPhamId { get; set; }
        public string tenSP { get; set; }
        public string hinhAnh { get; set; }
        public double giaTien { get; set; }
        public int    giamGia  { get; set; }
        public string moTa { get; set; }
        public int soLuongConLai { get; set; }
        public string LoaiSanPhamId { get; set; }
        public LoaiSanPham LoaiSanPham { get; set; }
        [ForeignKey("SanPhamId")]
        public ICollection<ChiTietHD> ChiTietHDs { get; set; }
        [ForeignKey("SanPhamId")]
        public ICollection<GioHang> GioHangs { get; set; }
    }
}
