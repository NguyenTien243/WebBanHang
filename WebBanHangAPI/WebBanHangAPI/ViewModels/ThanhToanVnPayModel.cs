using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHangAPI.ViewModels
{
    public class ThanhToanVnPayModel
    {
        public List<OrderSanPhamModel> danhSachDat { get; set; }
        public string diaChiGiaoHang { get; set; }
        public string sdtNguoiNhan { get; set; }
        public string bankCode { get; set; }
        public string vnpLocale { get; set; }
        public string vnp_Returnurl { get; set; }
    }
}
