using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHangAPI.ViewModels
{
    public class ThanhToanVNPayChoHoaDonModel
    {
        public string hoaDonId { get; set; }
        public string bankCode { get; set; }
        public string vnpLocale { get; set; }
        public string vnp_Returnurl { get; set; }
    }
}
