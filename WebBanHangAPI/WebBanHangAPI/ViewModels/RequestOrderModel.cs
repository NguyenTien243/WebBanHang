using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHangAPI.ViewModels
{
    public class RequestOrderModel
    {
        public List<OrderSanPhamModel> danhSachDat { get; set; }
        public string diaChiGiaoHang { get; set; }
        public string sdtNguoiNhan { get; set; }
        public bool thanhToanOnline { get; set; }
    }
}
