using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHangAPI.Models
{
    public class HoaDon
    {
        public string HoaDonId { get; set; }
        public string KhachHangId { get; set; }
        public string NhanVienId { get; set; }
        public double tongHoaDon { get; set; }
        public DateTime ngayXuatDon { get; set; }
        public string diaChiGiaoHang { get; set; }
        public string TrangThaiGiaoHangId { get; set; }
        public TrangThaiGiaoHang TrangThaiGiaoHang { get; set; }
        public Boolean thanhToanOnline { get; set; }
        public Boolean daThanhToan { get; set; }
        [ForeignKey("HoaDonId")]
        public ICollection<ChiTietHD> ChiTietHDs { get; set; }
    }
}
