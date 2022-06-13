using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHangAPI.ViewModels
{
    public class DanhGiaSanPhamViewModel
    {
        public string HoaDonId { get; set; }
        public string SanPhamId { get; set; }

        public int SoSao { get; set; }
        public string BinhLuanDanhGia { get; set; }
    }
}
