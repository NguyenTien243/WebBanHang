using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHangAPI.ViewModels
{
    public class ResponseHoaDon
    {
        public string HoaDonId { get; set; }
        public string NguoiDungId { get; set; }
        public string tenNguoiDung { get; set; }
        public double tongHoaDon { get; set; }
        public DateTime ngayXuatDon { get; set; }
        public string diaChiGiaoHang { get; set; }
        public string sdtNguoiNhan { get; set; }
        public string TrangThaiGiaoHangId { get; set; }
        public bool thanhToanOnline { get; set; }
        public bool daThanhToan { get; set; }
        public List<ItemChiTietHD> chiTietHD { get; set; }
    }
}
