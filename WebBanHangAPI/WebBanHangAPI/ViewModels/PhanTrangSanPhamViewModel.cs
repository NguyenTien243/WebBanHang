using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHangAPI.ViewModels
{
    public class PhanTrangSanPhamViewModel
    {
        public List<LaySanPhamViewModel> DanhSachSanPham { get; set; }
        public int TongSoTrang { get; set; }
        public int TrangHienTai { get; set; }
        public int KichThuocTrang { get; set; }
    }
}
