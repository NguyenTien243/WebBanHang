using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHangAPI.Models
{
    public class HoaDon
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string HoaDonId { get; set; }
        
        public string NguoiDungId { get; set; }
        public NguoiDung NguoiDung { get; set; }
        public double tongHoaDon { get; set; }
        public DateTime ngayXuatDon { get; set; }
        public string diaChiGiaoHang { get; set; }
        public string TrangThaiGiaoHangId { get; set; }
        public TrangThaiGiaoHang TrangThaiGiaoHang { get; set; }
        public bool thanhToanOnline { get; set; }
        public bool daThanhToan { get; set; }
        [ForeignKey("HoaDonId")]
        public ICollection<ChiTietHD> ChiTietHDs { get; set; }
    }
}
