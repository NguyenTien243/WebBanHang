using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHangAPI.ViewModels
{
    public class RequestUpdateStatusOrderModel
    {
        public string HoaDonId { get; set; }
        public string TrangThaiGiaoHangId { get; set; }
    }
}
