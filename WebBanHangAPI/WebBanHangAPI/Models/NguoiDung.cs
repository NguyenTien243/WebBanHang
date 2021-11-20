using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHangAPI.Models
{
    public class NguoiDung
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string NguoiDungId { get; set; }
        public string tenNguoiDung { get; set; }
        public string email { get; set; }
        public string sDT { get; set; }
        public string diaChi { get; set; }
        public bool gioiTinh { get; set; }
        public string tenDangNhap { get; set; }
        public string matKhau { get; set; }
        public string VaiTroId { get; set; }
        public VaiTro VaiTro { get; set; }
        public bool conHoatDong { get; set; }
        [ForeignKey("NguoiDungId")]
        public ICollection<GioHang> GioHangs { get; set; }


        public ICollection<HoaDon> HoaDons { get; set; }
    }
}
