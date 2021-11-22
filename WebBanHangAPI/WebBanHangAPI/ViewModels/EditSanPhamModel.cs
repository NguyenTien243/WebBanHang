using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHangAPI.ViewModels
{
    public class EditSanPhamModel
    {
        public string SanPhamId { get; set; }
        public string tenSP { get; set; }
        public string hinhAnh { get; set; }
        public double giaTien { get; set; }
        public int giamGia { get; set; }
        public string moTa { get; set; }
        public int soLuongConLai { get; set; }
        public string LoaiSanPhamId { get; set; }
    }
}
