using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHangAPI.ViewModels
{
    public class EditLoaiSanPhamModel
    {
        public string LoaiSanPhamId { get; set; }
        public string tenLoaiSP { get; set; }
        public string hinhAnh { get; set; }
    }
}
