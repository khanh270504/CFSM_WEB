using CFSM_WEB.Helpers;
using CFSM_WEB.Models;
using CFSM_WEB.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace CFSM_WEB.Controllers
{
    public class CartController : Controller
    {
        private readonly QuanLyQuanCaPheContext db;
		private readonly PaypalClient _paypalClient;
		public CartController(QuanLyQuanCaPheContext context, PaypalClient paypalClient)
        {
            _paypalClient = paypalClient;
            db = context;
        }

        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ??
            new List<CartItem>();
        public IActionResult Index()
        {
            return View(Cart);
        }

        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaDoAn == id);
            if (item == null)
            {
                var sanPham = db.TDoAns.SingleOrDefault(p => p.MaDoAn == id);
                if(sanPham == null)
                {
                    TempData["Message"] = $"Khong tim thay {id}";
                    return Redirect("/Cart/Error");
                }
                item = new CartItem
                {
                    MaDoAn = sanPham.MaDoAn,
                    TenDoAn = sanPham.TenDoAn,
                    DonGia = sanPham.DonGia,
                    AnhDoAn = sanPham.AnhDoAn ?? string.Empty,
                    SoLuong = quantity
                };
                gioHang.Add(item);
            }
            else
            {
                item.SoLuong += quantity;
            }
            HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            return RedirectToAction("Index");
        }

        public IActionResult RemoveCart(int id)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaDoAn == id);
            if (item != null)
            {
                gioHang.Remove(item);
                HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            }
            return RedirectToAction("Index");
        }
        [Authorize]
        [HttpGet]
        public IActionResult CheckOut()
        {
            if (Cart.Count == 0)
            {
                return Redirect("/");
            }
			ViewBag.PaypalClientdId = _paypalClient.ClientId;
			return View(Cart);
        }
       [Authorize]
        [HttpPost]
        public IActionResult CheckOut(CheckoutVM model)
        {
            if (ModelState.IsValid)
            {
                // Lấy customerId từ Claims và chuyển nó thành kiểu int
                var customerId = int.Parse(HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID)?.Value);
                var khachHang = new TKhachHang();
                if (model.GiongKhachHang)
                {
                    khachHang = db.TKhachHangs.SingleOrDefault(p => p.MaKhachHang == customerId);
                }

                // Tạo đối tượng hóa đơn
                var hoaDon = new THoaDon
                {   MaNhanVien = 1,
                    MaKhachHang = customerId,
                    HoTen = model.HoTen ?? khachHang.HoTen,
                    DiaChi = model.DiaChi ?? khachHang.DiaChi,
                    NgayLap = DateTime.Now,
                    ThanhTien = Cart.Sum(p => p.ThanhTien),
                    CachThanhToan = "COD",
                    GhiChu = model.GhiChu
                };

                if (db.Database.CurrentTransaction == null)
                {
                    db.Database.BeginTransaction(); // Bắt đầu giao dịch nếu không có giao dịch nào mở
                }

                try
                {
                    db.Add(hoaDon);
                    db.SaveChanges(); // Lưu hóa đơn trước để lấy mã hóa đơn

                    var cthd = new List<TChiTietHd>();
                    foreach (var item in Cart)
                    {
                        cthd.Add(new TChiTietHd
                        {
                            MaHoaDon = hoaDon.MaHoaDon, // Liên kết chi tiết với mã hóa đơn vừa tạo
                            SoLuong = item.SoLuong,
                            DonGia = item.DonGia,
                            MaDoAn = item.MaDoAn
                        });
                    }

                    db.AddRange(cthd);
                    db.SaveChanges(); // Lưu tất cả chi tiết hóa đơn vào CSDL cùng một lúc

                    db.Database.CommitTransaction(); // Commit giao dịch sau khi tất cả thay đổi được lưu thành công

                    HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>()); // Xóa giỏ hàng trong session

                    return View("Success");
                }
                catch (Exception)
                {
                    if (db.Database.CurrentTransaction != null)
                    {
                        db.Database.RollbackTransaction(); // Rollback giao dịch nếu có lỗi
                    }
                    // Log exception hoặc xử lý lỗi nếu cần thiết
                }
            }
            return View(Cart);
        }
		//Paypal
		[Authorize]
		public IActionResult PaymentSuccess()
		{
			return View("Success");
		}

		//Paypal payment
		[Authorize]
		[HttpPost("/Cart/create-paypal-order")]
		public async Task<IActionResult> CreatePaypalOrder(CancellationToken cancellationToken)
		{
			
			decimal usdToVndRate = 25390m; 
			var tongTienVnd = Cart.Sum(p => p.ThanhTien);

			var tongTienUsd = tongTienVnd / usdToVndRate;			
			var tongTien = tongTienUsd.ToString("F2", CultureInfo.InvariantCulture);			
			var donViTienTe = "USD";

			// Mã đơn hàng tham chiếu
			var maDonHangThamChieu = "DH" + DateTime.Now.Ticks.ToString();

			try
			{
				var response = await _paypalClient.CreateOrder(tongTien, donViTienTe, maDonHangThamChieu);

				return Ok(response);
			}
			catch (Exception ex)
			{
				var error = new { ex.GetBaseException().Message };
				return BadRequest(error);
			}
		}

		[Authorize]
		[HttpPost("/Cart/capture-paypal-order")]
        public async Task<IActionResult> CapturePaypalOrder(string orderID, CancellationToken cancellationToken)
        {
            try
            {
                // Gọi API PayPal để capture đơn hàng
                var response = await _paypalClient.CaptureOrder(orderID);

                if (response == null  || response.status != "COMPLETED")
                {
                    // Kiểm tra nếu PayPal trả về lỗi hoặc giao dịch không thành công
                    return BadRequest("Lỗi khi xử lý giao dịch PayPal.");
                }

                // Lấy ID khách hàng từ session
                var customerId = int.Parse(HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID)?.Value);
                var khachHang = db.TKhachHangs.SingleOrDefault(p => p.MaKhachHang == customerId);
                // Tạo hóa đơn
                var hoaDon = new THoaDon
                {
                    MaKhachHang = customerId,
                    HoTen = response.payer.name.given_name,
                    DiaChi = khachHang.DiaChi,
                    NgayLap = DateTime.Now,
                    CachThanhToan = "PayPal",
                    MaNhanVien = 1,  // Bạn có thể thay đổi giá trị này theo yêu cầu
                    ThanhTien = Cart.Sum(p => p.ThanhTien),
                    TrangThaiHoaDon = "Đã thanh toán",
                    GhiChu = "Thanh toán qua PayPal"
                };

                // Thêm hóa đơn vào cơ sở dữ liệu
                db.Add(hoaDon);
                await db.SaveChangesAsync();  // Lưu hóa đơn để lấy MaHoaDon

                // Tạo chi tiết hóa đơn
                var cthd = new List<TChiTietHd>();
                foreach (var item in Cart)
                {
                    cthd.Add(new TChiTietHd
                    {
                        MaHoaDon = hoaDon.MaHoaDon,  // Liên kết chi tiết hóa đơn với hóa đơn vừa tạo
                        SoLuong = item.SoLuong,
                        DonGia = item.DonGia,
                        MaDoAn = item.MaDoAn
                    });
                }

                // Thêm chi tiết hóa đơn vào cơ sở dữ liệu
                db.AddRange(cthd);
                await db.SaveChangesAsync();  // Lưu tất cả chi tiết hóa đơn vào CSDL

                // Xóa giỏ hàng trong session sau khi thanh toán thành công
                HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());

                // Trả về view Success sau khi thanh toán thành công
                return View("Success");
            }
            catch (Exception ex)
            {
                // Log exception hoặc xử lý lỗi nếu cần thiết
                var error = new { ex.GetBaseException().Message };
                return BadRequest(error);
            }
        }




    }
}
