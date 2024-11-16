using CFSM_WEB.Helpers;
using CFSM_WEB.Models;
using CFSM_WEB.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CFSM_WEB.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly QuanLyQuanCaPheContext db;

        public KhachHangController(QuanLyQuanCaPheContext context)
        {
            db = context;
        }
        [HttpGet]
        public IActionResult Login(string? ReturnUrl)
        {
			ViewBag.ReturnUrl = ReturnUrl;
			return View();
		}
        [HttpPost]
		public async Task<IActionResult> Login(LoginVM model, string? ReturnUrl)
		{
			ViewBag.ReturnUrl = ReturnUrl;

			if (ModelState.IsValid)
			{
				// Kiểm tra tài khoản khách hàng
				var acc = db.TTaiKhoans.SingleOrDefault(p => p.TenDangNhap == model.UserName);

				if (acc == null)
				{
					// Thêm lỗi nếu không tìm thấy tài khoản
					ModelState.AddModelError("UserName", "Không có khách hàng này");
					return View(model);
				}
				else
				{
                    if(acc.LoaiTaiKhoan == 1 && acc.MatKhau == model.Password )
                    {
                        return RedirectToAction("Index", "HomeAdmin", new { area = "admin" });
                    }
                    if(acc.MatKhau != model.Password)
                    {
                        ModelState.AddModelError("Password", "Sai thông tin đăng nhập");
                        return View(model);
                    }
					var salt = Convert.FromBase64String(acc.Salt);
                    var isPasswordValid = PasswordHelper.VerifyPassword(model.Password, acc.MatKhau, salt);
					if (!isPasswordValid)
					{
						// Thêm lỗi nếu mật khẩu sai
						ModelState.AddModelError("Password", "Sai thông tin đăng nhập");
						return View(model);
					}
                    else
                    {
                        var khachHang = db.TKhachHangs.SingleOrDefault(k => k.TenDangNhap == model.UserName);

                        
					// Tạo các Claims để đăng nhập
					var claims = new List<Claim>
		            {
			            new Claim(ClaimTypes.Name, khachHang.HoTen),
                        new Claim(MySetting.CLAIM_CUSTOMERID, khachHang.MaKhachHang.ToString()),
                        new Claim(ClaimTypes.Role, "Customer")
                    };

					var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
					var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

					// Duy trì phiên đăng nhập
					await HttpContext.SignInAsync(claimsPrincipal);

					// Kiểm tra đường dẫn trở về sau khi đăng nhập
					if (Url.IsLocalUrl(ReturnUrl))
					{
						return Redirect(ReturnUrl);
					}
					    else
					    {
						return Redirect("/");
					    }
                    }
					
				}
			}

			// Nếu ModelState không hợp lệ hoặc không phải POST request
			return View(model);
		}

		[Authorize]
		public IActionResult Profile()
        {
            var customerIdClaim = User.FindFirst(MySetting.CLAIM_CUSTOMERID);
            var customerId = int.Parse(customerIdClaim.Value);
            var khachHang = db.TKhachHangs.SingleOrDefault(k => k.MaKhachHang == customerId);
            return View(khachHang);
        }
		[Authorize]
		public async Task<IActionResult> LogOut()
		{
			await HttpContext.SignOutAsync();
			return Redirect("/");
		}

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterVM model)
        {
            if(ModelState.IsValid)
            {
                var acc = db.TTaiKhoans.FirstOrDefault(t => t.TenDangNhap == model.UserName);
                if(acc != null)
                {
                    ModelState.AddModelError("", "Tên đăng nhập đã tồn tại");
                    return View(model);
                }
                byte[] salt;
                var hashedPassword = PasswordHelper.HashPassword(model.Password, out salt);


                var taiKhoan = new TTaiKhoan
                {
                    TenDangNhap = model.UserName,
                    MatKhau = hashedPassword,
                    Salt = Convert.ToBase64String(salt),
                    LoaiTaiKhoan = 0 
                };
                db.TTaiKhoans.Add(taiKhoan);
                db.SaveChanges(); 

               
                var khachHang = new TKhachHang
                {
                    HoTen = model.FullName,
                    TenHienThi = model.UserName, 
                    Email = model.Email,
                    DiaChi = model.Address,
                    SoDienThoai = model.PhoneNumber,
                    TenDangNhap = model.UserName
               
                    
                };
                db.TKhachHangs.Add(khachHang);
                db.SaveChanges(); 

                return RedirectToAction("Login");
            }
            return View(model);
        }

    }
}
