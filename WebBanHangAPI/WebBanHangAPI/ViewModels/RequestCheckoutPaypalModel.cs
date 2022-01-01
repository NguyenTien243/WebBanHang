using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHangAPI.ViewModels
{
    public class RequestCheckoutPaypalModel
    {
        public List<OrderSanPhamModel> danhSachDat { get; set; }
        public string diaChiGiaoHang { get; set; }
        public string sdtNguoiNhan { get; set; }
        public string paymentId { get; set; }
        public string PayerID { get; set; }
    }
}
