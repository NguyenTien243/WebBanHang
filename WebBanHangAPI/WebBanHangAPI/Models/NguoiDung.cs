using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHangAPI.Models
{   [Table("NguoiDungs")]
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
        public bool daXoa { get; set; }
        public VaiTro VaiTro { get; set; }
        public bool conHoatDong { get; set; }
        [ForeignKey("NguoiDungId")]
        public ICollection<GioHang> GioHangs { get; set; }

        [InverseProperty("NguoiDung")]
        public ICollection<HoaDon> HoaDonsKhachHang { get; set; }
        [InverseProperty("NhanVien")]
        public ICollection<HoaDon> HoaDonsNhanVien { get; set; }
        //https://stackoverflow.com/questions/30717484/entity-framework-multiple-references-to-same-table
    }
}
