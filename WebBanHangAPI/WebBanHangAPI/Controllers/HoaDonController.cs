using BraintreeHttp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PayPal.Core;
using PayPal.v1.Payments;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using WebBanHangAPI.Common;
using WebBanHangAPI.IServices;
using WebBanHangAPI.Models;
using WebBanHangAPI.ViewModels;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Http;
using WebBanHangAPI.VNPayHelper;
using System.Web;

namespace WebBanHangAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoaDonController : ControllerBase
    {

        private readonly WebBanHangAPIDBContext _context;
        private readonly IJwtAuthenticationManager _jwtAuthenticationManager;
        private readonly string _clientId;
        private readonly string _secretKey;
        private readonly IConfiguration _configuration;
        public double TyGiaUSD = 23300;
        public HoaDonController(WebBanHangAPIDBContext context, IJwtAuthenticationManager jwtAuthenticationManager,
            IConfiguration config)
        {
            _context = context;
            _jwtAuthenticationManager = jwtAuthenticationManager;
            _clientId = config["PaypalSettings:ClientId"];
            _secretKey = config["PaypalSettings:SecretKey"];
            _configuration = config;
        }

        [HttpPost("ThanhToanPaypal")]
        public async Task<IActionResult> ThanhToanPaypal([FromBody] ThanhToanPaypalModel requestOrder)
        {

            //return Redirect("https://www.google.com/");
            List<ItemGioHang> myCart = new List<ItemGioHang>();
            foreach (var item in requestOrder.danhSachDat)
            {
                var sp = await _context.SanPhams.FindAsync(item.SanPhamId);
                if (sp == null)
                    return NotFound(new Response { Status = 404, Message = $"Không tìm thấy sản phẩm id {item.SanPhamId}" });
                // kiểm tra số lượng đặt có đủ không
                if (sp.soLuongConLai < item.soLuongDat)
                    return BadRequest(new Response { Status = 400, Message = $"Sản phẩm id = {item.SanPhamId} không đủ số lượng để đặt, số lượng tối đa có thể đặt {sp.soLuongConLai}" });
                ItemGioHang spdatmua = new ItemGioHang();
                spdatmua.tenSP = sp.tenSP;
                spdatmua.SoLuongTrongGio = item.soLuongDat;
                spdatmua.giaTien = sp.giaTien * (100 - sp.giamGia) / 100;
                myCart.Add(spdatmua);
            }
            var environment = new SandboxEnvironment(_clientId, _secretKey);
            var client = new PayPalHttpClient(environment);

            //ItemGioHang a = new ItemGioHang();
            //ItemGioHang b = new ItemGioHang();
            //a.tenSP = "SP1";
            //b.tenSP = "SP2";
            //a.giaTien = 100000;
            //b.giaTien = 200000;
            //myCart.Add(a);
            //myCart.Add(b);
            //a.SoLuongTrongGio = 1;
            //b.SoLuongTrongGio = 1;
            #region Create Paypal Order
            var itemList = new ItemList()
            {
                Items = new List<Item>()
            };
            //var total = Math.Round(myCart.Sum(p => p.giaTien) / TyGiaUSD,2);
            double total = 0;
            foreach (var item in myCart)
            {
                itemList.Items.Add(new Item()
                {
                    Name = item.tenSP,
                    Currency = "USD",
                    Price = Math.Round(item.giaTien / TyGiaUSD, 2).ToString(),
                    Quantity = item.SoLuongTrongGio.ToString(),
                    Sku = "sku",
                    Tax = "0"
                });
                double c = Math.Round(item.giaTien * item.SoLuongTrongGio / TyGiaUSD, 2);
                total += c;
            }
            total = Math.Round(total, 2);
            #endregion
            var paypalOrderId = DateTime.Now.Ticks;
            var hostname = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            var payment = new Payment()
            {
                Intent = "sale",
                Transactions = new List<Transaction>()
                {
                    new Transaction()
                    {
                        Amount = new Amount()
                        {
                            Total = total.ToString(),
                            Currency = "USD",
                            Details = new AmountDetails
                            {
                                Tax = "0",
                                Shipping = "0",
                                Subtotal = total.ToString()
                            }
                        },
                        ItemList = itemList,
                        Description = $"Invoice #{paypalOrderId}",
                        InvoiceNumber = paypalOrderId.ToString()
                    }
                },
                RedirectUrls = new RedirectUrls()
                {
                    CancelUrl = $"{hostname}",
                    ReturnUrl = requestOrder.urlRedirect
                },
                Payer = new Payer()
                {
                    PaymentMethod = "paypal"
                }
            };

            PaymentCreateRequest request = new PaymentCreateRequest();
            request.RequestBody(payment);
            string paypalRedirectUrl = null;
            try
            {

                var response = await client.Execute(request);
                var statusCode = response.StatusCode;
                Payment result = response.Result<Payment>();

                var links = result.Links.GetEnumerator();

                while (links.MoveNext())
                {
                    LinkDescriptionObject lnk = links.Current;
                    if (lnk.Rel.ToLower().Trim().Equals("approval_url"))
                    {
                        //saving the payapalredirect URL to which user will be redirected for payment  
                        paypalRedirectUrl = lnk.Href;
                    }
                }

                // return Redirect(paypalRedirectUrl);
            }
            catch (HttpException httpException)
            {
                var statusCode = httpException.StatusCode;
                var debugId = httpException.Headers.GetValues("PayPal-Debug-Id").FirstOrDefault();

                //Process when Checkout with Paypal fails
                return BadRequest(new Response { Status = 400, Message = httpException.Message.ToString() });

            }
            return Ok(new Response { Status = 200, Message = Message.Success, Data = paypalRedirectUrl });
        }

        //[HttpGet("ThanhToanPaypal")]
        //public async Task<IActionResult> ThanhToanPaypal()
        //{

        //    //   
        //    var environment = new SandboxEnvironment(_clientId, _secretKey);
        //    var client = new PayPalHttpClient(environment);
        //    List<ItemGioHang> myCart = new List<ItemGioHang>();
        //    ItemGioHang a = new ItemGioHang();
        //    ItemGioHang b = new ItemGioHang();
        //    a.tenSP = "SP1";
        //    b.tenSP = "SP2";
        //    a.giaTien = 100000;
        //    b.giaTien = 200000;
        //    myCart.Add(a);
        //    myCart.Add(b);
        //    a.SoLuongTrongGio = 1;
        //    b.SoLuongTrongGio = 1;
        //    #region Create Paypal Order
        //    var itemList = new ItemList()
        //    {
        //        Items = new List<Item>()
        //    };
        //    //var total = Math.Round(myCart.Sum(p => p.giaTien) / TyGiaUSD,2);
        //    var total = 12.87;
        //    foreach (var item in myCart)
        //    {
        //        itemList.Items.Add(new Item()
        //        {
        //            Name = item.tenSP,
        //            Currency = "USD",
        //            Price = Math.Round(item.giaTien / TyGiaUSD, 2).ToString(),
        //            Quantity = item.SoLuongTrongGio.ToString(),
        //            Sku = "sku",
        //            Tax = "0"
        //        });
        //    }
        //    #endregion
        //    var paypalOrderId = DateTime.Now.Ticks;
        //    var hostname = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
        //    var payment = new Payment()
        //    {
        //        Intent = "sale",
        //        Transactions = new List<Transaction>()
        //        {
        //            new Transaction()
        //            {
        //                Amount = new Amount()
        //                {
        //                    Total = total.ToString(),
        //                    Currency = "USD",
        //                    Details = new AmountDetails
        //                    {
        //                        Tax = "0",
        //                        Shipping = "0",
        //                        Subtotal = total.ToString()
        //                    }
        //                },
        //                ItemList = itemList,
        //                Description = $"Invoice #{paypalOrderId}",
        //                InvoiceNumber = paypalOrderId.ToString()
        //            }
        //        },
        //        RedirectUrls = new RedirectUrls()
        //        {
        //            CancelUrl = $"{hostname}",
        //            ReturnUrl = $"https://www.google.com/"
        //        },
        //        Payer = new Payer()
        //        {
        //            PaymentMethod = "paypal"
        //        }
        //    };

        //    PaymentCreateRequest request = new PaymentCreateRequest();
        //    request.RequestBody(payment);

        //    try
        //    {

        //        var response = await client.Execute(request);
        //        var statusCode = response.StatusCode;
        //        Payment result = response.Result<Payment>();

        //        var links = result.Links.GetEnumerator();
        //        string paypalRedirectUrl = null;
        //        while (links.MoveNext())
        //        {
        //            LinkDescriptionObject lnk = links.Current;
        //            if (lnk.Rel.ToLower().Trim().Equals("approval_url"))
        //            {
        //                //saving the payapalredirect URL to which user will be redirected for payment  
        //                paypalRedirectUrl = lnk.Href;
        //            }
        //        }

        //        return Redirect(paypalRedirectUrl);
        //    }
        //    catch (HttpException httpException)
        //    {
        //        var statusCode = httpException.StatusCode;
        //        var debugId = httpException.Headers.GetValues("PayPal-Debug-Id").FirstOrDefault();

        //        //Process when Checkout with Paypal fails
        //        return BadRequest(new Response { Status = 400, Message = httpException.Message.ToString() });

        //    }
        //    return Ok(new Response { Status = 200, Message = Message.Success });
        //}
        [Authorize]
        [HttpPost("ThanhToanVNPay")]
        public async Task<IActionResult> ThanhToanVNPay([FromBody] ThanhToanVnPayModel requestOrder)
        {

            //Get Config Info
            //string vnp_Returnurl = _configuration["VnPaySettings:vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = _configuration["VnPaySettings:vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = _configuration["VnPaySettings:vnp_TmnCode"]; //Ma website
            string vnp_HashSecret = _configuration["VnPaySettings:vnp_HashSecret"]; //Chuoi bi mat

            if (string.IsNullOrEmpty(vnp_TmnCode) || string.IsNullOrEmpty(vnp_HashSecret))
            {
                return BadRequest((new Response { Status = 400, Message = "Vui lòng cấu hình các tham số: vnp_TmnCode,vnp_HashSecret trong file web.config" }));
            }


            if (requestOrder.danhSachDat.Count == 0)
                return BadRequest(new Response { Status = 400, Message = "Danh sách đặt trống!" });
            if (string.IsNullOrEmpty(requestOrder.diaChiGiaoHang))
                return BadRequest(new Response { Status = 400, Message = "Thiếu địa chỉ giao hàng!" });
            if (string.IsNullOrEmpty(requestOrder.sdtNguoiNhan))
                return BadRequest(new Response { Status = 400, Message = "Thiếu số điện thoại người nhận!" });
            if (string.IsNullOrEmpty(requestOrder.vnp_Returnurl))
                return BadRequest(new Response { Status = 400, Message = "Thiếu vnp_Returnurl!" });

            // check out thành công tiến hành lưu lại hóa đơn
            var NguoiDungId = "";
            Request.Headers.TryGetValue("Authorization", out var tokenheaderValue);
            JwtSecurityToken token = null;
            try
            {
                token = _jwtAuthenticationManager.GetInFo(tokenheaderValue);
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = "Không xác thực được người dùng" });
            }
            NguoiDungId = token.Claims.First(claim => claim.Type == "nguoiDungId").Value;
            double tongHoaDon = 0;

            var findUser = await _context.NguoiDungs.FindAsync(NguoiDungId);
            if (findUser == null)
                return BadRequest(new Response { Status = 400, Message = "Không tìm thấy Người dùng" });

            //1. Add vào hóa đơn
            var hoadon = new HoaDon();
            _context.HoaDons.Add(hoadon);
            hoadon.ngayXuatDon = DateTime.Now;
            hoadon.diaChiGiaoHang = requestOrder.diaChiGiaoHang;
            hoadon.sdtNguoiNhan = requestOrder.sdtNguoiNhan;
            hoadon.NguoiDungId = NguoiDungId;
            hoadon.TrangThaiGiaoHangId = "1";
            //if (request.thanhToanOnline)
            //{
            //    hoadon.thanhToanOnline = true;
            //    hoadon.daThanhToan = true;
            //}
            hoadon.thanhToanOnline = true;
            hoadon.daThanhToan = false;
            foreach (var item in requestOrder.danhSachDat)
            {
                var sp = await _context.SanPhams.FindAsync(item.SanPhamId);
                if (sp == null)
                    return NotFound(new Response { Status = 404, Message = $"Không tìm thấy sản phẩm id {item.SanPhamId}" });
                // kiểm tra số lượng đặt có đủ không
                if (sp.soLuongConLai < item.soLuongDat)
                    return BadRequest(new Response { Status = 400, Message = $"Sản phẩm id = {item.SanPhamId} không đủ số lượng để đặt, số lượng tối đa có thể đặt {sp.soLuongConLai}" });
                sp.soLuongConLai -= item.soLuongDat;
                //sp.soLuongDaBan += item.soLuongDat;
                ChiTietHD chitietdat = new ChiTietHD();
                chitietdat.HoaDonId = hoadon.HoaDonId;
                chitietdat.SanPhamId = sp.SanPhamId;
                SetValuesChitietHD(ref chitietdat, sp.tenSP, sp.hinhAnh, sp.giamGia, sp.giaTien, item.soLuongDat);
                _context.ChiTietHDs.Add(chitietdat);
                tongHoaDon += chitietdat.tongTien;
            }
            // xoa san pham trong gio hang
            var giohang = await _context.GioHangs.Where(gh => gh.NguoiDungId == NguoiDungId).ToListAsync();
            foreach (var item in giohang)
            {
                if (requestOrder.danhSachDat.Any(order => order.SanPhamId == item.SanPhamId))
                    _context.GioHangs.Remove(item);
            }
            //Tính tổng hóa đơn có thêm mã giảm giá
            var findMaGiamGia = await _context.MaGiamGias.Where(m => m.MaGiamGiaId == requestOrder.maGiamGiaId).ToListAsync();
            foreach (var maGiam in findMaGiamGia)
            {
                //Kiểm tra thời gian sử dụng
                if (maGiam.NgayBatDau.Value.Date <= hoadon.ngayXuatDon.Date && hoadon.ngayXuatDon.Date <= maGiam.NgayHetHang.Value.Date)
                {
                    //Kiểm tra số lượng mã giảm giá còn lại
                    if (maGiam.SoLuongSuDUng > 0)
                    {
                        //Kiểm tra đơn tối thiểu có đủ để sử dụng không
                        if (maGiam.DonToiThieu <= tongHoaDon)
                        {
                            //Tìm mã trong bảng MaCuaNgDung
                            var Ma = await _context.MaGiamGiaCuaNgDungs.Where(m => m.MaGiamGiaId == maGiam.MaGiamGiaId && m.NguoiDungId == NguoiDungId).ToListAsync();
                            //Kiểm tra mã của người dùng đã sử dụng không
                            foreach (var m in Ma)
                            {
                                if (m.DaSuDung == false)
                                {
                                    //Chuyển mã thành đã sử dụng
                                    m.DaSuDung = true;

                                    //Số lượng mã giảm giá giảm đi 1
                                    maGiam.SoLuongSuDUng--;

                                    //Nếu bằng 0 thì giảm trực tiếp vào đơn
                                    if (maGiam.KieuGiam == 0)
                                    {
                                        tongHoaDon = tongHoaDon - maGiam.GiamToiDa;
                                        if (tongHoaDon < 0) tongHoaDon = 0;
                                    }

                                    // Nếu bằng 1 thì giảm theo % 
                                    if (maGiam.KieuGiam == 1)
                                    {
                                        //Nếu phần số tiền giảm theo phần trăm lớn hơn giảm tối đa thì giảm trực tiếp
                                        if(tongHoaDon * maGiam.GiamGia > maGiam.GiamToiDa) tongHoaDon = tongHoaDon - maGiam.GiamGia;
                                        else //Nếu k lớn hơn thì giảm theo phần trăm
                                        tongHoaDon = tongHoaDon - (tongHoaDon * (maGiam.GiamGia/100));
                                    }
                                }
                                else
                                {
                                    //Trả về thông báo không thành công
                                    return BadRequest(new Response { Status = 400, Message = "Bạn đã sử dụng mã này rồi" });
                                }
                            }
                        }
                        else
                        {
                            //Trả về thông báo không thành công
                            return BadRequest(new Response { Status = 400, Message = $"Không sử dụng được mã giảm giá vì không đủ đơn hàng tối thiểu! Đơn tối tối thiểu để sử dụng là {maGiam.DonToiThieu} VNĐ" });
                        }
                    }
                    else
                    {
                        //Trả về thông báo không thành công
                        return BadRequest(new Response { Status = 400, Message = "Mã giảm giá có thể sử dụng đã hết" });
                    }
                }
                else
                {
                    //trả về thông báo không thành công
                    if (maGiam.NgayBatDau.Value.Date > hoadon.ngayXuatDon.Date)
                    {
                        //Chưa tới ngày sử dụng
                        return BadRequest(new Response { Status = 400, Message = "Chưa tới ngày sử dụng" });
                    }
                    if (hoadon.ngayXuatDon.Date > maGiam.NgayHetHang.Value.Date)
                    {
                        //Mã đã hết hạn
                        return BadRequest(new Response { Status = 400, Message = "Mã đã hết hạn" });
                    }

                }
            }
            hoadon.tongHoaDon = tongHoaDon;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = e.ToString() });
            }

            // tạo request VNPAY
            VnPayLibrary pay = new VnPayLibrary();
            pay.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
            pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_TmnCode", vnp_TmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            pay.AddRequestData("vnp_Amount", (hoadon.tongHoaDon * 100).ToString()); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
            if (!String.IsNullOrEmpty(requestOrder.bankCode))
            {
                pay.AddRequestData("vnp_BankCode", requestOrder.bankCode);
            }
            //pay.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            pay.AddRequestData("vnp_CreateDate", hoadon.ngayXuatDon.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            pay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(HttpContext)); //Địa chỉ IP của khách hàng thực hiện giao dịch
            if (!string.IsNullOrEmpty(requestOrder.vnpLocale))
            {
                pay.AddRequestData("vnp_Locale", requestOrder.vnpLocale);
            }
            else
            {
                pay.AddRequestData("vnp_Locale", "vn");
            }
            //pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            pay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + hoadon.HoaDonId); //Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            pay.AddRequestData("vnp_ReturnUrl", requestOrder.vnp_Returnurl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            pay.AddRequestData("vnp_TxnRef", hoadon.HoaDonId); //mã hóa đơn
            pay.AddRequestData("vnp_ExpireDate", DateTime.Now.AddDays(3).ToString("yyyyMMddHHmmss"));
            string paymentUrl = pay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return Ok((new Response { Status = 200, Message = "Ok", Data = paymentUrl }));
        }

        [Authorize]
        [HttpPost("ThanhToanVNPayTheoHoaDon")]
        public async Task<IActionResult> ThanhToanVNPayTheoHoaDon([FromBody] ThanhToanVNPayChoHoaDonModel requestOrder)
        {
            //Get Config Info
            //string vnp_Returnurl = _configuration["VnPaySettings:vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = _configuration["VnPaySettings:vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = _configuration["VnPaySettings:vnp_TmnCode"]; //Ma website
            string vnp_HashSecret = _configuration["VnPaySettings:vnp_HashSecret"]; //Chuoi bi mat

            if (string.IsNullOrEmpty(vnp_TmnCode) || string.IsNullOrEmpty(vnp_HashSecret))
            {
                return BadRequest((new Response { Status = 400, Message = "Vui lòng cấu hình các tham số: vnp_TmnCode,vnp_HashSecret trong file web.config" }));
            }

            if (string.IsNullOrEmpty(requestOrder.hoaDonId))
                return BadRequest(new Response { Status = 400, Message = "Thiếu hoaDonId!" });
            if (string.IsNullOrEmpty(requestOrder.vnp_Returnurl))
                return BadRequest(new Response { Status = 400, Message = "Thiếu vnp_Returnurl!" });

            // check out thành công tiến hành lưu lại hóa đơn
            var NguoiDungId = "";
            Request.Headers.TryGetValue("Authorization", out var tokenheaderValue);
            JwtSecurityToken token = null;
            try
            {
                token = _jwtAuthenticationManager.GetInFo(tokenheaderValue);
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = "Không xác thực được người dùng" });
            }
            NguoiDungId = token.Claims.First(claim => claim.Type == "nguoiDungId").Value;

            var findUser = await _context.NguoiDungs.FindAsync(NguoiDungId);
            if (findUser == null)
                return BadRequest(new Response { Status = 400, Message = "Không tìm thấy Người dùng" });

            // lấy hóa đơn
            var hoadon = await _context.HoaDons.Where(x => x.HoaDonId == requestOrder.hoaDonId).FirstOrDefaultAsync();
            if(hoadon == null)
                return BadRequest(new Response { Status = 400, Message = "Không tìm thấy hóa đơn" });
            if (hoadon.daThanhToan == true)
                return BadRequest(new Response { Status = 400, Message = "Hóa đơn đã thanh toán" });
            // tạo request VNPAY
            VnPayLibrary pay = new VnPayLibrary();
            pay.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
            pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_TmnCode", vnp_TmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            pay.AddRequestData("vnp_Amount", (hoadon.tongHoaDon * 100).ToString()); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
            if (!String.IsNullOrEmpty(requestOrder.bankCode))
            {
                pay.AddRequestData("vnp_BankCode", requestOrder.bankCode);
            }
            //pay.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            pay.AddRequestData("vnp_CreateDate", hoadon.ngayXuatDon.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            pay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(HttpContext)); //Địa chỉ IP của khách hàng thực hiện giao dịch
            if (!string.IsNullOrEmpty(requestOrder.vnpLocale))
            {
                pay.AddRequestData("vnp_Locale", requestOrder.vnpLocale);
            }
            else
            {
                pay.AddRequestData("vnp_Locale", "vn");
            }
            //pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            pay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + hoadon.HoaDonId); //Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            pay.AddRequestData("vnp_ReturnUrl", requestOrder.vnp_Returnurl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            pay.AddRequestData("vnp_TxnRef", hoadon.HoaDonId); //mã hóa đơn
            pay.AddRequestData("vnp_ExpireDate", DateTime.Now.AddDays(3).ToString("yyyyMMddHHmmss"));
            string paymentUrl = pay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return Ok((new Response { Status = 200, Message = "Ok", Data = paymentUrl }));
        }

        [HttpGet("CheckoutVNPay")]
        public async Task<IActionResult> CheckoutVNPay([FromQuery]RequestCheckoutVNPayModel request)
        {
            string vnp_HashSecret = _configuration["VnPaySettings:vnp_HashSecret"]; //Chuoi bi mat

            VnPayLibrary vnpay = new VnPayLibrary();
            var vnpayData = HttpUtility.ParseQueryString(Request.QueryString.ToString());
            foreach (string s in vnpayData)
            {
                //get all querystring data
                if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(s, vnpayData[s]);
                }
            }
            bool checkSignature = vnpay.ValidateSignature(request.vnp_SecureHash, vnp_HashSecret);
            if (checkSignature)
            {
                if (request.vnp_ResponseCode == "00" && request.vnp_TransactionStatus == "00")
                {
                    //Thanh toan thanh cong
                    var hoadon = await _context.HoaDons.Where(x => x.HoaDonId == request.vnp_TxnRef).FirstOrDefaultAsync();
                    if (hoadon == null)
                        return BadRequest(new Response { Status = 400, Message = "Không tìm thấy hóa đơn!" });
                    hoadon.daThanhToan = true;
                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        return BadRequest(new Response { Status = 400, Message = e.ToString() });
                    }
                    return Ok((new Response { Status = 200, Message = "Thanh toán đơn hàng thành công", Data = request }));
                }
                else
                {
                    //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                    return BadRequest(new Response { Status = 400, Message = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + request.vnp_ResponseCode, Data = request});
                }
            }
            else
            {
                return BadRequest(new Response { Status = 400, Message = "Có lỗi xảy ra trong quá trình xử lý, vnp_HashSecret không hợp lệ", Data = request });
            }
        }

        // tham khảo XuanThulap https://www.youtube.com/watch?v=iJlAMzFy4yQ
        [HttpPost("CheckoutPaypal")]
        public async Task<IActionResult> CheckoutPaypal(RequestCheckoutPaypalModel request)
        {
            if (request.danhSachDat.Count == 0)
                return BadRequest(new Response { Status = 400, Message = "Danh sách đặt trống!" });
            if (string.IsNullOrEmpty(request.diaChiGiaoHang))
                return BadRequest(new Response { Status = 400, Message = "Thiếu địa chỉ giao hàng!" });
            if (string.IsNullOrEmpty(request.sdtNguoiNhan))
                return BadRequest(new Response { Status = 400, Message = "Thiếu số điện thoại người nhận!" });
            if (String.IsNullOrEmpty(request.paymentId))
                return BadRequest(new Response { Status = 400, Message = "Thiếu paymentId" });
            if (String.IsNullOrEmpty(request.PayerID))
                return BadRequest(new Response { Status = 400, Message = "Thiếu PayerID" });

            using var httpClient = new System.Net.Http.HttpClient();
            httpClient.BaseAddress = new Uri("http://example.com/");
            httpClient.DefaultRequestHeaders
      .Accept
      .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header

            var authenticationString = $"AV4JKwmAtMXAawVtRhe-WIC7tg6W3uOWvwok7QPnIdt8HBUWAdC6rnTkR3Kdz__HUFi33TXRBhITkwTT:EKdnQsgfLVcF06apKsJKnXvy01Uj_6hhvQ5UOmd-O5tvPcKw0x9fjUJqQCIeZ-AeJtM_J-oVCRXiF0Vt";
            var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));
            var httpMessageRequest = new HttpRequestMessage();
            httpMessageRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
            httpMessageRequest.Method = HttpMethod.Post;
            httpMessageRequest.RequestUri = new Uri("https://api.sandbox.paypal.com/v1/payments/payment/" + request.paymentId + "/execute");



            //string data = @"{
            //    ""payer_id"" : ""${}"",
            //    ""matKhau"" : ""123456""
            //}";

            string data = "{\r\n                \"" + "payer_id" + "\" : \"" + request.PayerID + "\"   }";
            var content = new StringContent(data, Encoding.UTF8, "application/json");

            //httpMessageRequest.Headers.Add("User-Agent","Mozilla/5.0");
            //httpMessageRequest.Headers.Add("Content-Type", "application/json");
            //var parameters = new List<KeyValuePair<string, string>>();
            //parameters.Add(new KeyValuePair<string, string>("tenDangNhap", "admin"));
            //parameters.Add(new KeyValuePair<string, string>("matKhau", "123456"));


            //var content = new FormUrlEncodedContent(parameters);
            httpMessageRequest.Content = content;
            var httpResponseMessage = await httpClient.SendAsync(httpMessageRequest);

            if (httpResponseMessage.StatusCode != System.Net.HttpStatusCode.OK)
                return BadRequest(new Response { Status = 400, Message = "Thanh toán lỗi, vui lòng thử lại!" });

            // check out thành công tiến hành lưu lại hóa đơn
            var NguoiDungId = "";
            Request.Headers.TryGetValue("Authorization", out var tokenheaderValue);
            JwtSecurityToken token = null;
            try
            {
                token = _jwtAuthenticationManager.GetInFo(tokenheaderValue);
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = "Không xác thực được người dùng" });
            }
            NguoiDungId = token.Claims.First(claim => claim.Type == "nguoiDungId").Value;
            double tongHoaDon = 0;

            var findUser = await _context.NguoiDungs.FindAsync(NguoiDungId);
            if (findUser == null)
                return BadRequest(new Response { Status = 400, Message = "Không tìm thấy Người dùng" });
            //1. Add vào hóa đơn
            var hoadon = new HoaDon();
            _context.HoaDons.Add(hoadon);
            hoadon.ngayXuatDon = DateTime.Now;
            hoadon.diaChiGiaoHang = request.diaChiGiaoHang;
            hoadon.sdtNguoiNhan = request.sdtNguoiNhan;
            hoadon.NguoiDungId = NguoiDungId;
            hoadon.TrangThaiGiaoHangId = "1";
            //if (request.thanhToanOnline)
            //{
            //    hoadon.thanhToanOnline = true;
            //    hoadon.daThanhToan = true;
            //}
            hoadon.thanhToanOnline = true;
            hoadon.daThanhToan = true;
            foreach (var item in request.danhSachDat)
            {
                var sp = await _context.SanPhams.FindAsync(item.SanPhamId);
                if (sp == null)
                    return NotFound(new Response { Status = 404, Message = $"Không tìm thấy sản phẩm id {item.SanPhamId}" });
                // kiểm tra số lượng đặt có đủ không
                if (sp.soLuongConLai < item.soLuongDat)
                    return BadRequest(new Response { Status = 400, Message = $"Sản phẩm id = {item.SanPhamId} không đủ số lượng để đặt, số lượng tối đa có thể đặt {sp.soLuongConLai}" });
                sp.soLuongConLai -= item.soLuongDat;
                //sp.soLuongDaBan += item.soLuongDat;
                ChiTietHD chitietdat = new ChiTietHD();
                chitietdat.HoaDonId = hoadon.HoaDonId;
                chitietdat.SanPhamId = sp.SanPhamId;
                SetValuesChitietHD(ref chitietdat, sp.tenSP, sp.hinhAnh, sp.giamGia, sp.giaTien, item.soLuongDat);
                _context.ChiTietHDs.Add(chitietdat);
                //Tính toán tổng hóa đơn khi áp dụng mã giảm giá
                tongHoaDon += chitietdat.tongTien;
            }
            // xoa san pham trong gio hang
            var giohang = await _context.GioHangs.Where(gh => gh.NguoiDungId == NguoiDungId).ToListAsync();
            foreach (var item in giohang)
            {
                if (request.danhSachDat.Any(order => order.SanPhamId == item.SanPhamId))
                    _context.GioHangs.Remove(item);
            }

            hoadon.tongHoaDon = tongHoaDon;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = e.ToString() });
            }
            return Ok(new Response { Status = 200, Message = "Tạo hóa đơn thành công" });

            //var html = await httpResponseMessage.Content.ReadAsStringAsync();

        }
        //[Authorize]
        [HttpGet("thongketrangthaidonhang")]
        public async Task<IActionResult> ThongKeTrangThaiDonHang()
        {
            //var NguoiDungRole = "";
            //Request.Headers.TryGetValue("Authorization", out var tokenheaderValue);
            //JwtSecurityToken token = null;
            //try
            //{
            //    token = _jwtAuthenticationManager.GetInFo(tokenheaderValue);
            //}
            //catch (IndexOutOfRangeException e)
            //{
            //    return BadRequest(new Response { Status = 400, Message = "Không xác thực được người dùng" });
            //}
            //NguoiDungRole = token.Claims.First(claim => claim.Type == "vaiTro").Value;
            //if (NguoiDungRole != "admin")
            //    return BadRequest(new Response { Status = 400, Message = "Không có quyền!, vui lòng đăng nhập với tài khoản admin" });
            var trangthai = await _context.HoaDons.GroupBy(gr => gr.TrangThaiGiaoHangId).OrderBy(o => o.Key).Select(m => new { TrangThaiGiaoHangId = m.Key, soLuongHoaDon = m.Count() }).ToListAsync();//.grop(o => o.ngayXuatDon.Month);
                                                                                                                                                                                                       //if (trangthai.Count == 4)
                                                                                                                                                                                                       //    return Ok(new Response { Status = 200, Message = Message.Success, Data = trangthai });


            return Ok(new Response { Status = 200, Message = Message.Success, Data = trangthai });
        }

        //[Authorize]
        [HttpGet("thongkedoanhthutheothang/{nam}")]
        public async Task<IActionResult> ThongKeDoanhThuTheoThang(int nam)
        {

            //var NguoiDungRole = "";
            //Request.Headers.TryGetValue("Authorization", out var tokenheaderValue);
            //JwtSecurityToken token = null;
            //try
            //{
            //    token = _jwtAuthenticationManager.GetInFo(tokenheaderValue);
            //}
            //catch (IndexOutOfRangeException e)
            //{
            //    return BadRequest(new Response { Status = 400, Message = "Không xác thực được người dùng" });
            //}
            //NguoiDungRole = token.Claims.First(claim => claim.Type == "vaiTro").Value;
            //if (NguoiDungRole != "admin")
            //    return BadRequest(new Response { Status = 400, Message = "Không có quyền!, vui lòng đăng nhập với tài khoản admin" });
            var doanhthu = await _context.HoaDons.Where(s => s.TrangThaiGiaoHangId == "4" && s.ngayXuatDon.Year == nam).GroupBy(gr => gr.ngayXuatDon.Month).OrderBy(o => o.Key).Select(m => new { thang = m.Key, tongDoanhThu = m.Sum(s => s.tongHoaDon) }).ToListAsync();//.grop(o => o.ngayXuatDon.Month);//Select(sl => new ThongKeSanPhamModel()
            //{
            //    SanPhamId = sl.SanPhamId,
            //    tenSP = sl.tenSP,
            //    hinhAnh = sl.hinhAnh,
            //    soLuongDaBan = sl.soLuongDaBan
            //}).OrderByDescending(t => t.soLuongDaBan).Take(soluong).ToListAsync();
            if (doanhthu.Count == 12)
                return Ok(new Response { Status = 200, Message = Message.Success, Data = doanhthu });

            List<ThongKeDoanhThuModel> response = new List<ThongKeDoanhThuModel>();
            // kiểm tra nếu tháng không có doanh thu thì thêm danh thu = 0 vào tháng đó


            for (int i = 0; i < 12; i++)
            {
                ThongKeDoanhThuModel tk = new ThongKeDoanhThuModel();
                if (doanhthu.Any(x => x.thang == i + 1))
                {
                    var d = doanhthu.Where(x => x.thang == i + 1).FirstOrDefault();
                    tk.thang = d.thang;
                    tk.tongDoanhThu = d.tongDoanhThu;
                }
                else
                {
                    tk.thang = i + 1;
                    tk.tongDoanhThu = 0;
                }
                response.Add(tk);
            }
            return Ok(new Response { Status = 200, Message = Message.Success, Data = response });
        }

        [Authorize]
        [HttpGet("danhsachhoadonNguoiDung")]
        public async Task<IActionResult> DanhSachHoaDonNguoiDung()
        {
            var NguoiDungId = "";
            Request.Headers.TryGetValue("Authorization", out var tokenheaderValue);
            JwtSecurityToken token = null;
            try
            {
                token = _jwtAuthenticationManager.GetInFo(tokenheaderValue);
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = "Không xác thực được người dùng" });
            }
            NguoiDungId = token.Claims.First(claim => claim.Type == "nguoiDungId").Value;
            var findHoaDon = await _context.HoaDons.Where(hd => hd.NguoiDungId == NguoiDungId).Select(sl => new ResponseHoaDon()
            {
                HoaDonId = sl.HoaDonId,
                NguoiDungId = sl.NguoiDungId,
                tenNguoiDung = sl.NguoiDung.tenNguoiDung,
                diaChiGiaoHang = sl.diaChiGiaoHang,
                daThanhToan = sl.daThanhToan,
                ngayXuatDon = sl.ngayXuatDon,
                sdtNguoiNhan = sl.sdtNguoiNhan,
                thanhToanOnline = sl.thanhToanOnline,
                tongHoaDon = sl.tongHoaDon,
                TrangThaiGiaoHangId = sl.TrangThaiGiaoHangId,
                chiTietHD = sl.ChiTietHDs.Select(ct => new ItemChiTietHD()
                {
                    SanPhamId = ct.SanPhamId,
                    tenSP = ct.tenSP,
                    hinhAnh = ct.hinhAnh,
                    giaTien = ct.giaTien,
                    giamGia = ct.giamGia,
                    soLuongDat = ct.soLuongDat,
                    tongTien = ct.tongTien
                }).ToList()
            }).ToListAsync();
            if (findHoaDon.Count == 0)
                return BadRequest(new Response { Status = 400, Message = "Danh sách hóa đơn trống!" });
            return Ok(new Response { Status = 200, Message = Message.Success, Data = findHoaDon });
        }
        [HttpGet("danhsachtatcahoadon")]
        public async Task<IActionResult> DanhSachTatCaHoaDon()
        {

            var findHoaDon = await _context.HoaDons.Select(sl => new ResponseHoaDon()
            {
                HoaDonId = sl.HoaDonId,
                NguoiDungId = sl.NguoiDungId,
                diaChiGiaoHang = sl.diaChiGiaoHang,
                tenNguoiDung = sl.NguoiDung.tenNguoiDung,
                daThanhToan = sl.daThanhToan,
                ngayXuatDon = sl.ngayXuatDon,
                sdtNguoiNhan = sl.sdtNguoiNhan,
                thanhToanOnline = sl.thanhToanOnline,
                tongHoaDon = sl.tongHoaDon,
                TrangThaiGiaoHangId = sl.TrangThaiGiaoHangId,
                chiTietHD = sl.ChiTietHDs.Select(ct => new ItemChiTietHD()
                {
                    SanPhamId = ct.SanPhamId,
                    tenSP = ct.tenSP,
                    hinhAnh = ct.hinhAnh,
                    giaTien = ct.giaTien,
                    giamGia = ct.giamGia,
                    soLuongDat = ct.soLuongDat,
                    tongTien = ct.tongTien
                }).ToList()
            }).ToListAsync();
            if (findHoaDon.Count == 0)
                return BadRequest(new Response { Status = 400, Message = "Hóa đơn trống!" });
            return Ok(new Response { Status = 200, Message = Message.Success, Data = findHoaDon });
        }
        [HttpGet("xemchitiethoadon/{hoadonId}")]
        public async Task<IActionResult> XemChiTietHoaDon(string hoadonId)
        {

            var findHoaDon = await _context.HoaDons.Include(hd => hd.ChiTietHDs).Where(hd => hd.HoaDonId == hoadonId).Select(sl => new ResponseHoaDon()
            {
                HoaDonId = sl.HoaDonId,
                NguoiDungId = sl.NguoiDungId,
                tenNguoiDung = sl.NguoiDung.tenNguoiDung,
                diaChiGiaoHang = sl.diaChiGiaoHang,
                daThanhToan = sl.daThanhToan,
                ngayXuatDon = sl.ngayXuatDon,
                sdtNguoiNhan = sl.sdtNguoiNhan,
                thanhToanOnline = sl.thanhToanOnline,
                tongHoaDon = sl.tongHoaDon,
                TrangThaiGiaoHangId = sl.TrangThaiGiaoHangId,
                chiTietHD = sl.ChiTietHDs.Select(ct => new ItemChiTietHD()
                {
                    SanPhamId = ct.SanPhamId,
                    tenSP = ct.tenSP,
                    hinhAnh = ct.hinhAnh,
                    giaTien = ct.giaTien,
                    giamGia = ct.giamGia,
                    soLuongDat = ct.soLuongDat,
                    tongTien = ct.tongTien
                }).ToList()
            }).ToListAsync();
            if (findHoaDon.Count == 0)
                return BadRequest(new Response { Status = 400, Message = "Không tìm thấy hóa đơn!" });
            return Ok(new Response { Status = 200, Message = Message.Success, Data = findHoaDon });
        }
        [Authorize]
        [HttpGet("Xemhoadontheotrangthaiadmin/{TrangThaiGiaoHangId}")]
        public async Task<IActionResult> XemHoaDonTheoTrangThaiAdmin(string TrangThaiGiaoHangId)
        {
            var NguoiDungId = "";
            var NguoiDungRole = "";
            Request.Headers.TryGetValue("Authorization", out var tokenheaderValue);
            JwtSecurityToken token = null;
            try
            {
                token = _jwtAuthenticationManager.GetInFo(tokenheaderValue);
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = "Không xác thực được người dùng" });
            }
            NguoiDungId = token.Claims.First(claim => claim.Type == "nguoiDungId").Value;
            NguoiDungRole = token.Claims.First(claim => claim.Type == "vaiTro").Value;
            if (NguoiDungRole != "admin" && NguoiDungRole != "staff")
                return BadRequest(new Response { Status = 400, Message = "Không có quyền!, vui lòng đăng nhập với tài khoản admin hoặc nhận viên!" });

            var findHoaDon = await _context.HoaDons.Include(hd => hd.ChiTietHDs).Where(hd => hd.TrangThaiGiaoHangId == TrangThaiGiaoHangId).Select(sl => new ResponseHoaDon()
            {
                HoaDonId = sl.HoaDonId,
                NguoiDungId = sl.NguoiDungId,
                tenNguoiDung = sl.NguoiDung.tenNguoiDung,
                diaChiGiaoHang = sl.diaChiGiaoHang,
                daThanhToan = sl.daThanhToan,
                ngayXuatDon = sl.ngayXuatDon,
                sdtNguoiNhan = sl.sdtNguoiNhan,
                thanhToanOnline = sl.thanhToanOnline,
                tongHoaDon = sl.tongHoaDon,
                TrangThaiGiaoHangId = sl.TrangThaiGiaoHangId,
                chiTietHD = sl.ChiTietHDs.Select(ct => new ItemChiTietHD()
                {
                    SanPhamId = ct.SanPhamId,
                    tenSP = ct.tenSP,
                    hinhAnh = ct.hinhAnh,
                    giaTien = ct.giaTien,
                    giamGia = ct.giamGia,
                    soLuongDat = ct.soLuongDat,
                    tongTien = ct.tongTien
                }).ToList()
            }).ToListAsync();
            if (findHoaDon.Count == 0)
                return BadRequest(new Response { Status = 400, Message = "Chưa có đơn hàng!" });
            return Ok(new Response { Status = 200, Message = Message.Success, Data = findHoaDon });
        }
        [Authorize]
        [HttpGet("Xemhoadontheotrangthaicuanguoidung/{TrangThaiGiaoHangId}")]
        public async Task<IActionResult> XemHoaDonTheoTrangThaiNguoiDung(string TrangThaiGiaoHangId)
        {
            var NguoiDungId = "";
            Request.Headers.TryGetValue("Authorization", out var tokenheaderValue);
            JwtSecurityToken token = null;
            try
            {
                token = _jwtAuthenticationManager.GetInFo(tokenheaderValue);
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = "Không xác thực được người dùng" });
            }
            NguoiDungId = token.Claims.First(claim => claim.Type == "nguoiDungId").Value;
            var findHoaDon = await _context.HoaDons.Include(hd => hd.ChiTietHDs).Where(hd => hd.TrangThaiGiaoHangId == TrangThaiGiaoHangId && hd.NguoiDungId == NguoiDungId).Select(sl => new ResponseHoaDon()
            {
                HoaDonId = sl.HoaDonId,
                NguoiDungId = sl.NguoiDungId,
                tenNguoiDung = sl.NguoiDung.tenNguoiDung,
                diaChiGiaoHang = sl.diaChiGiaoHang,
                daThanhToan = sl.daThanhToan,
                ngayXuatDon = sl.ngayXuatDon,
                sdtNguoiNhan = sl.sdtNguoiNhan,
                thanhToanOnline = sl.thanhToanOnline,
                tongHoaDon = sl.tongHoaDon,
                TrangThaiGiaoHangId = sl.TrangThaiGiaoHangId,
                chiTietHD = sl.ChiTietHDs.Select(ct => new ItemChiTietHD()
                {
                    SanPhamId = ct.SanPhamId,
                    tenSP = ct.tenSP,
                    hinhAnh = ct.hinhAnh,
                    giaTien = ct.giaTien,
                    giamGia = ct.giamGia,
                    soLuongDat = ct.soLuongDat,
                    tongTien = ct.tongTien
                }).ToList()
            }).ToListAsync();
            if (findHoaDon.Count == 0)
                return BadRequest(new Response { Status = 400, Message = "Chưa có đơn hàng!" });
            return Ok(new Response { Status = 200, Message = Message.Success, Data = findHoaDon });
        }




        //tạo hóa đơn
        //1. Add vào hóa đơn
        //2. Thêm sản phẩm vào chi tiết hóa đơn (kiểm tra số lượng đặt, 
        // mã sản phẩm, trừ số lượng còn lại ở sản phẩm)


        private void SetValuesChitietHD(ref ChiTietHD result, string tenSP,
            string hinhAnh, int giamGia, double giaTien, int soLuongDat)
        {
            result.tenSP = tenSP;
            result.hinhAnh = hinhAnh;
            result.giamGia = giamGia;
            result.giaTien = giaTien;
            result.soLuongDat = soLuongDat;
            result.tongTien = (giaTien * (100 - giamGia) / 100) * soLuongDat; 
        }
        [Authorize]
        [HttpPost("taohoadon")]
        public async Task<ActionResult<GioHang>> TaoHoaDon([FromBody] RequestOrderModel requestOrder)
        {
            if (requestOrder.danhSachDat.Count == 0)
                return BadRequest(new Response { Status = 400, Message = "Danh sách đặt trống!" });
            if (string.IsNullOrEmpty(requestOrder.diaChiGiaoHang))
                return BadRequest(new Response { Status = 400, Message = "Thiếu địa chỉ giao hàng!" });
            if (string.IsNullOrEmpty(requestOrder.sdtNguoiNhan))
                return BadRequest(new Response { Status = 400, Message = "Số điện thoại người nhận!" });
            var NguoiDungId = "";
            Request.Headers.TryGetValue("Authorization", out var tokenheaderValue);
            JwtSecurityToken token = null;
            try
            {
                token = _jwtAuthenticationManager.GetInFo(tokenheaderValue);
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = "Không xác thực được người dùng" });
            }
            NguoiDungId = token.Claims.First(claim => claim.Type == "nguoiDungId").Value;
            double tongHoaDon = 0;

            var findUser = await _context.NguoiDungs.FindAsync(NguoiDungId);
            if (findUser == null)
                return BadRequest(new Response { Status = 400, Message = "Không tìm thấy Người dùng" });
            //1. Add vào hóa đơn
            var hoadon = new HoaDon();
            _context.HoaDons.Add(hoadon);
            hoadon.ngayXuatDon = DateTime.Now;
            hoadon.diaChiGiaoHang = requestOrder.diaChiGiaoHang;
            hoadon.sdtNguoiNhan = requestOrder.sdtNguoiNhan;
            hoadon.NguoiDungId = NguoiDungId;
            hoadon.TrangThaiGiaoHangId = "1";
            if (requestOrder.thanhToanOnline)
            {
                hoadon.thanhToanOnline = true;
                hoadon.daThanhToan = true;
            }

            foreach (var item in requestOrder.danhSachDat)
            {
                var sp = await _context.SanPhams.FindAsync(item.SanPhamId);
                if (sp == null)
                    return NotFound(new Response { Status = 404, Message = $"Không tìm thấy sản phẩm id {item.SanPhamId}" });
                // kiểm tra số lượng đặt có đủ không
                if (sp.soLuongConLai < item.soLuongDat)
                    return BadRequest(new Response { Status = 400, Message = $"Sản phẩm id = {item.SanPhamId} không đủ số lượng để đặt, số lượng tối đa có thể đặt {sp.soLuongConLai}" });
                sp.soLuongConLai -= item.soLuongDat;
                //sp.soLuongDaBan += item.soLuongDat;
                ChiTietHD chitietdat = new ChiTietHD();
                chitietdat.HoaDonId = hoadon.HoaDonId;
                chitietdat.SanPhamId = sp.SanPhamId;
                SetValuesChitietHD(ref chitietdat, sp.tenSP, sp.hinhAnh, sp.giamGia, sp.giaTien, item.soLuongDat);
                _context.ChiTietHDs.Add(chitietdat);                 
                tongHoaDon += chitietdat.tongTien;
            }
            // xoa san pham trong gio hang
            var giohang = await _context.GioHangs.Where(gh => gh.NguoiDungId == NguoiDungId).ToListAsync();
            foreach (var item in giohang)
            {
                if (requestOrder.danhSachDat.Any(order => order.SanPhamId == item.SanPhamId))
                    _context.GioHangs.Remove(item);
            }

            //Tính tổng hóa đơn có thêm mã giảm giá
            var findMaGiamGia = await _context.MaGiamGias.Where(m => m.MaGiamGiaId == requestOrder.maGiamGiaId).ToListAsync();
            foreach (var maGiam in findMaGiamGia)
            {
                //Kiểm tra thời gian sử dụng
                if(maGiam.NgayBatDau.Value.Date <= hoadon.ngayXuatDon.Date && hoadon.ngayXuatDon.Date <= maGiam.NgayHetHang.Value.Date)
                {
                    //Kiểm tra số lượng mã giảm giá còn lại
                    if(maGiam.SoLuongSuDUng > 0)
                    {
                        //Kiểm tra đơn tối thiểu có đủ để sử dụng không
                        if (maGiam.DonToiThieu <= tongHoaDon)
                        {
                            //Tìm mã trong bảng MaCuaNgDung
                            var Ma = await _context.MaGiamGiaCuaNgDungs.Where(m => m.MaGiamGiaId == maGiam.MaGiamGiaId && m.NguoiDungId == NguoiDungId).ToListAsync();
                            //Kiểm tra mã của người dùng đã sử dụng không
                            foreach (var m in Ma)
                            {
                                if(m.DaSuDung == false)
                                {
                                    //Chuyển mã thành đã sử dụng
                                    m.DaSuDung = true;

                                    //Số lượng mã giảm giá giảm đi 1
                                    maGiam.SoLuongSuDUng--;

                                    //Nếu bằng 0 thì giảm trực tiếp vào đơn
                                    if (maGiam.KieuGiam == 0)
                                    {
                                        tongHoaDon = tongHoaDon - maGiam.GiamToiDa;
                                        if (tongHoaDon < 0) tongHoaDon = 0;
                                    }

                                    // Nếu bằng 1 thì giảm theo % 
                                    if (maGiam.KieuGiam == 1)
                                    {
                                        //Nếu phần số tiền giảm theo phần trăm lớn hơn giảm tối đa thì giảm trực tiếp
                                        if (tongHoaDon * maGiam.GiamGia > maGiam.GiamToiDa) tongHoaDon = tongHoaDon - maGiam.GiamGia;
                                        else //Nếu k lớn hơn thì giảm theo phần trăm
                                            tongHoaDon = tongHoaDon - (tongHoaDon * (maGiam.GiamGia/100));
                                    }
                                }
                                else
                                {
                                    //Trả về thông báo không thành công
                                    return BadRequest(new Response { Status = 400, Message = "Bạn đã sử dụng mã này rồi" });
                                }                                   
                            }                           
                        }
                        else
                        {
                            //Trả về thông báo không thành công
                            return BadRequest(new Response { Status = 400, Message = $"Không sử dụng được mã giảm giá vì không đủ đơn hàng tối thiểu! Đơn tối tối thiểu để sử dụng là {maGiam.DonToiThieu} VNĐ" });
                        }
                    }
                    else
                    {
                        //Trả về thông báo không thành công
                        return BadRequest(new Response { Status = 400, Message = "Mã giảm giá có thể sử dụng đã hết" });
                    }
                }
                else
                {
                    //trả về thông báo không thành công
                    if (maGiam.NgayBatDau.Value.Date > hoadon.ngayXuatDon.Date)
                    {
                        //Chưa tới ngày sử dụng
                        return BadRequest(new Response { Status = 400, Message = "Chưa tới ngày sử dụng" });
                    }
                    if (hoadon.ngayXuatDon.Date > maGiam.NgayHetHang.Value.Date)
                    {
                        //Mã đã hết hạn
                        return BadRequest(new Response { Status = 400, Message = "Mã đã hết hạn" });
                    }    
                        
                }              
            }
            hoadon.tongHoaDon = tongHoaDon;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = e.ToString() });
            }
            return Ok(new Response { Status = 200, Message = "Tạo hóa đơn thành công" });
        }

        [Authorize]
        [HttpPut("capnhattrangthaidonAdmin")]
        public async Task<IActionResult> CapNhatTrangThaiDonAdmin([FromBody] RequestUpdateStatusOrderModel request)
        {
            var NguoiDungRole = "";
            Request.Headers.TryGetValue("Authorization", out var tokenheaderValue);
            JwtSecurityToken token = null;
            try
            {
                token = _jwtAuthenticationManager.GetInFo(tokenheaderValue);
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = "Không xác thực được người dùng" });
            }
            NguoiDungRole = token.Claims.First(claim => claim.Type == "vaiTro").Value;
            if (NguoiDungRole != "admin" && NguoiDungRole != "staff")
                return BadRequest(new Response { Status = 400, Message = "Không có quyền!, vui lòng đăng nhập với tài khoản admin hoặc nhận viên!" });
            var findHoaDon = await _context.HoaDons.FindAsync(request.HoaDonId);
            if (findHoaDon == null)
            {
                return NotFound(new Response { Status = 404, Message = "Không tìm thấy hóa đơn" });
            }

            switch (request.TrangThaiGiaoHangId)
            {
                case "3":
                    if (findHoaDon.TrangThaiGiaoHangId != "1")
                        return BadRequest(new Response { Status = 400, Message = "Đơn hàng phải ở trạng thái chờ xác nhận trước!" });
                    break;
                case "4":
                    if (findHoaDon.TrangThaiGiaoHangId != "3")
                        return BadRequest(new Response { Status = 400, Message = "Đơn hàng phải ở trạng thái đang giao trước!" });
                    findHoaDon.daThanhToan = true;
                    var findCTHoaDon2 = await _context.ChiTietHDs.Where(hd => hd.HoaDonId == findHoaDon.HoaDonId).ToListAsync();
                    List<string> ListIdSP2 = findCTHoaDon2.Select(o => o.SanPhamId).ToList();
                    var listSP2 = _context.SanPhams.Where(e => ListIdSP2.Contains(e.SanPhamId));
                    Dictionary<string, int> soluongdat2 = findCTHoaDon2.ToDictionary(x => x.SanPhamId, x => x.soLuongDat);
                    foreach (var item in listSP2)
                    {

                        item.soLuongDaBan += soluongdat2[item.SanPhamId];
                    }
                    break;
                case "5":
                    if (findHoaDon.TrangThaiGiaoHangId == "4")
                        return BadRequest(new Response { Status = 400, Message = "Không thể hủy đơn hàng đã giao!" });
                    var findCTHoaDon = await _context.ChiTietHDs.Where(hd => hd.HoaDonId == findHoaDon.HoaDonId).ToListAsync();
                    List<string> ListIdSP = findCTHoaDon.Select(o => o.SanPhamId).ToList();
                    var listSP = _context.SanPhams.Where(e => ListIdSP.Contains(e.SanPhamId));
                    Dictionary<string, int> soluongdat = findCTHoaDon.ToDictionary(x => x.SanPhamId, x => x.soLuongDat);
                    foreach (var item in listSP)
                    {
                        item.soLuongConLai += soluongdat[item.SanPhamId];
                        //item.soLuongDaBan -= soluongdat[item.SanPhamId];
                    }
                    break;
                default:
                    // code block
                    break;
            }

            try
            {
                findHoaDon.TrangThaiGiaoHangId = request.TrangThaiGiaoHangId;
                await _context.SaveChangesAsync();
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = e.ToString() });
            }
            return Ok(new Response { Status = 200, Message = "Cập nhật trạng thái giao hàng thành công", Data = request });
        }

        [Authorize]
        [HttpPut("nguoidunghuydonhang/{id}")]
        public async Task<IActionResult> NguoiDungHuyDonHang(string id)
        {
            var NguoiDungId = "";
            Request.Headers.TryGetValue("Authorization", out var tokenheaderValue);
            JwtSecurityToken token = null;
            try
            {
                token = _jwtAuthenticationManager.GetInFo(tokenheaderValue);
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = "Không xác thực được người dùng" });
            }

            NguoiDungId = token.Claims.First(claim => claim.Type == "nguoiDungId").Value;

            var findHoaDon = await _context.HoaDons.FindAsync(id);
            if (findHoaDon == null)
            {
                return NotFound(new Response { Status = 404, Message = "Không tìm thấy hóa đơn" });
            }
            if (NguoiDungId != findHoaDon.NguoiDungId)
                return BadRequest(new Response { Status = 400, Message = "Hóa đơn yêu cầu hủy không trùng khách hàng Id!" });
            if (findHoaDon.TrangThaiGiaoHangId != "1")
                return BadRequest(new Response { Status = 400, Message = "Chỉ có thể hủy đơn hàng đang chờ xác nhận!" });
            var findCTHoaDon = await _context.ChiTietHDs.Where(hd => hd.HoaDonId == findHoaDon.HoaDonId).ToListAsync();
            List<string> ListIdSP = findCTHoaDon.Select(o => o.SanPhamId).ToList();
            var listSP = _context.SanPhams.Where(e => ListIdSP.Contains(e.SanPhamId));
            Dictionary<string, int> soluongdat = findCTHoaDon.ToDictionary(x => x.SanPhamId, x => x.soLuongDat);
            foreach (var item in listSP)
            {
                item.soLuongConLai += soluongdat[item.SanPhamId];
                item.soLuongDaBan -= soluongdat[item.SanPhamId];
            }
            try
            {
                findHoaDon.TrangThaiGiaoHangId = "5";
                await _context.SaveChangesAsync();
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(new Response { Status = 400, Message = e.ToString() });
            }
            return Ok(new Response { Status = 200, Message = "Hủy đơn hàng thành công" });
        }
    }
}
