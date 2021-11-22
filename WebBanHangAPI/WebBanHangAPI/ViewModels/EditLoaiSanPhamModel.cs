using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHangAPI.ViewModels
{
    public class EditLoaiSanPhamModel
    {
        [Required(ErrorMessage = "Thiếu LoaiSanPhamId")]
        public string LoaiSanPhamId { get; set; }
        [Required(ErrorMessage = "Thiếu tenLoaiSP")]
        public string tenLoaiSP { get; set; }
    }
}
