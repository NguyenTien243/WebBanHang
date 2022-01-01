using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHangAPI.ViewModels
{
    public class ThanhToanPaypalModel
    {
        public List<OrderSanPhamModel> danhSachDat { get; set; }
        public string urlRedirect { get; set; }
    }
}
