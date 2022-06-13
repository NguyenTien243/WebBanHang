using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBanHangAPI.Models;

namespace WebBanHangAPI.ViewModels
{
    public class LaySanPhamViewModel
    {
        public string SanPhamId { get; set; }
        public string tenSP { get; set; }
        public string hinhAnh { get; set; }
        public double giaTien { get; set; }
        public int giamGia { get; set; }
        public string moTa { get; set; }
        public int soLuongConLai { get; set; }
        public int soLuongDaBan { get; set; }
        public string LoaiSanPhamId { get; set; }
        public bool isDeleted { get; set; }
        public float DanhGiaTrungBinh { get; set; }
    }
}
