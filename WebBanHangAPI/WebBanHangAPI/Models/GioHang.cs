using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHangAPI.Models
{
    public class GioHang
    {
        [Key, Column(Order = 0)]
        public string NguoiDungId { get; set; }
        public NguoiDung NguoiDung { get; set; }

        [Key, Column(Order = 1)]
        public string SanPhamId { get; set; }
        public SanPham SanPham { get; set; }
        public int soLuong { get; set; }
        
    }
}
