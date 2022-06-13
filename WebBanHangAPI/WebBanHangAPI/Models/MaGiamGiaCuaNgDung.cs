using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanHangAPI.Models
{
    public class MaGiamGiaCuaNgDung
    {
        [Key, Column(Order = 0)]
        public string? NguoiDungId { get; set; }
        public NguoiDung NguoiDung { get; set; }

        [Key, Column(Order = 1)]
        public string? MaGiamGiaId { get; set; }
        public MaGiamGia MaGiamGia { get; set; }
        public bool DaSuDung { get; set; }
    }
}
