using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHangAPI.Models
{
    public class MaGiamGia
    {
        [Key, Column(Order = 0), Required]
        public string MaGiamGiaId { get; set; }
        public string TenMaGiamGia { get; set; }
        public string NoiDungChiTiet { get; set; }
        public DateTime? NgayHetHang { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public double GiamToiDa { get; set; }
        public double DonToiThieu { get; set; }
        //KieuGiam: Nếu bằng 0 thì giảm trực tiếp vào, nếu bằng một thì giảm theo phần trăm
        public int KieuGiam { get; set; }
        //GiamGia có giá trị từ 1 đến 100(giảm theo %)
        public int GiamGia { get; set; }
        public int SoLuongSuDUng { get; set; }
    }
}
