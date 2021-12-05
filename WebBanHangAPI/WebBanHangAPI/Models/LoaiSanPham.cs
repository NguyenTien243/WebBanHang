using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHangAPI.Models
{
    public class LoaiSanPham
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string LoaiSanPhamId { get; set; }
        public string tenLoaiSP { get; set; }
        public string hinhAnh { get; set; }
        public ICollection<SanPham> SanPhams { get; set; }
    }
}
