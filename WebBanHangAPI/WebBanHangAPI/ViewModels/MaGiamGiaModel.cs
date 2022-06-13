using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebBanHangAPI.ValidationAttributes;

namespace WebBanHangAPI.ViewModels
{
    public class MaGiamGiaModel
    {
        [Required(ErrorMessage ="Code là bắt buộc")]
        public string MaGiamGiaId { get; set; }
        [Required(ErrorMessage = "Tên mã giám giá là bắt buộc")]
        public string TenMaGiamGia { get; set; }
        public string NoiDungChiTiet { get; set; }
        [MaGiamGia_NgayHetHanPhaiSauNgayBatDau]
        public DateTime? NgayHetHang { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public double GiamToiDa { get; set; }
        public double DonToiThieu { get; set; }
        //KieuGiam: Nếu bằng 0 thì giảm trực tiếp vào, nếu bằng một thì giảm theo phần trăm
        public int KieuGiam { get; set; }
        //GiamGia có giá trị từ 1 đến 100(giảm theo %)
        public int GiamGia { get; set; }
        public int SoLuongSuDUng { get; set; }

        public bool KTNgayHetHanPhaiSauNgayBatDau()
        {
            if (!NgayBatDau.HasValue || !NgayHetHang.HasValue) return true;
            return NgayBatDau.Value.Date <= NgayHetHang.Value.Date;
        }
    }
}
