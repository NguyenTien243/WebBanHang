using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHangAPI.ViewModels
{
    public class OrderSanPhamModel
    {
        public string SanPhamId { get; set; }
        public int soLuongDat { get; set; }
    }
}
