using Microsoft.AspNetCore.Mvc;
using CFSM_WEB.Models;
using X.PagedList;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Azure;
using X.PagedList.Extensions;
using CFSM_WEB.ViewModels;
using CFSM_WEB.Helpers;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Net.NetworkInformation;
using System;

namespace CFSM_WEB.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/homeadmin")]
    public class HomeAdminController : Controller
    {
        QuanLyQuanCaPheContext db = new QuanLyQuanCaPheContext();
        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            var customerIdClaim = User.FindFirst(MySetting.CLAIM_CUSTOMERID);
            var customerId = int.Parse(customerIdClaim.Value);
            var nhanVien = db.TNhanViens.SingleOrDefault(p => p.MaNhanVien == customerId);

            if (nhanVien == null)
            {
                return NotFound();  
            }     
            return View(nhanVien);  
        }
        [Route("DanhMucSP")]
        public IActionResult DanhMucSP(int? page)
        {
            int pageSize = 8;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            var lstSanPham = db.TDoAns.AsNoTracking().OrderBy(x => x.TenDoAn).Include(x => x.MaMenuNavigation);
            PagedList<TDoAn> lst = new PagedList<TDoAn>(lstSanPham, pageNumber, pageSize);
            return View(lst);
        }

        [Route("ThemSanPhamMoi")]
        [HttpGet]
        public IActionResult ThemSanPhamMoi()
        {
            ViewBag.MaMenu = new SelectList(db.TMenus.ToList(), "MaMenu", "TenMenu");
            return View();
        }
        [Route("ThemSanPhamMoi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemSanPhamMoi(TDoAn sanPham, IFormFile AnhFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra xem có file ảnh không
                    if (AnhFile != null && AnhFile.Length > 0)
                    {
                        // Tạo tên file mới để tránh trùng lặp
                        var fileName = Path.GetFileNameWithoutExtension(AnhFile.FileName);
                        var extension = Path.GetExtension(AnhFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImagesMenu", fileName + extension);

                        // Lưu ảnh vào thư mục "wwwroot/images/"
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            AnhFile.CopyTo(stream);
                        }

                        // Lưu đường dẫn ảnh vào thuộc tính AnhDoAn
                        sanPham.AnhDoAn = "" + fileName + extension;
                    }

                    // Thêm sản phẩm vào database
                    db.TDoAns.Add(sanPham);
                    db.SaveChanges();

                    return RedirectToAction("DanhMucSP");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi lưu sản phẩm: " + ex.Message);
                }
            }

            // Đảm bảo ViewBag.MaMenu được gán lại khi có lỗi
            ViewBag.MaMenu = new SelectList(db.TMenus.ToList(), "MaMenu", "TenMenu");
            return View(sanPham);
        }


        [Route("SuaSanPham")]
        [HttpGet]
        public IActionResult SuaSanPham(int maSanPham)
        {
            ViewBag.MaMenu = new SelectList(db.TMenus.ToList(), "MaMenu", "TenMenu");
            var sanPham = db.TDoAns.Find(maSanPham);
            return View(sanPham);
        }
        [Route("SuaSanPham")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaSanPham(TDoAn sanPham)
        {

            if (ModelState.IsValid)
            {
                db.TDoAns.Attach(sanPham);
                db.Entry(sanPham).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DanhMucSP", "HomeAdmin");
            }
            return View(sanPham);
        }

        [Route("XoaSanPham")]
        [HttpGet]
        public IActionResult XoaSanPham(int maSanPham)
        {
            TempData["XoaSanPham"] = "";
            var chiTietHd = db.TChiTietHds.Where(x => x.MaDoAn == maSanPham).ToList();
            if (chiTietHd.Count() > 0)
            {
                TempData["Message"] = "Khong ban duoc san pham nay";
                return RedirectToAction("DanhMucSP", "HomeAdmin");
            }
            db.Remove(db.TDoAns.Find(maSanPham));
            db.SaveChanges();
            TempData["Message"] = "San pham da duoc xoa";
            ViewBag.MaMenu = new SelectList(db.TMenus.ToList(), "MaMenu", "TenMenu");
            return RedirectToAction("DanhMucSP", "HomeAdmin");
        }

        [Route("DanhSachHD")]
        [HttpGet]
        public IActionResult DanhSachHD(int? page)
        {
            int pageSize = 8;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            var listHD = db.THoaDons
                           .Include(x => x.MaKhachHangNavigation)
                           .Include(x => x.MaNhanVienNavigation)
                           .ToList();

            PagedList<THoaDon> lst = new PagedList<THoaDon>(listHD, pageNumber, pageSize);
            return View(lst);
        }

        [Route("DanhMucChiTietHD")]
        [HttpGet]
        public IActionResult DanhMucChiTietHD(int maHD)
        {
            // Lấy danh sách chi tiết hóa đơn có mã hóa đơn = maHD
            var listChiTietHD = db.TChiTietHds
                                  .Where(x => x.MaHoaDon == maHD)
                                  .Include(x => x.MaDoAnNavigation)
                                  .Include(x => x.MaHoaDonNavigation)
                                  .ToList();

            // Kiểm tra xem danh sách có dữ liệu hay không
            if (!listChiTietHD.Any())
            {
                ViewBag.Message = "Không tìm thấy chi tiết hóa đơn nào cho mã hóa đơn này.";
            }

            return View(listChiTietHD);
        }
        [Route("DanhSachKhachHang")]
        [HttpGet]
        public IActionResult DanhSachKhachHang()
        {
            var listkh = db.TKhachHangs.ToList();

            return View(listkh);
        }
        [Route("DanhSachKhachHangOn")]
        [HttpGet]
        public IActionResult DanhSachKhachHangOn(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 6;

            var danhSachKhachHangOn = db.TKhachHangs
                                        .Where(k => k.TrangThai == 1)
                                        .ToPagedList(pageNumber, pageSize);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_DanhSachKhachHang", danhSachKhachHangOn);
            }

            return View(danhSachKhachHangOn);
        }
        [Route("DungHoatDongKhach")]
        [HttpGet]
        public IActionResult DungHoatDongKhach(int maKhach)
        {
            TempData["DungHoatDongKhach"] = "";

            // Tìm khách hàng theo tên đăng nhập
            var khachHang = db.TKhachHangs.FirstOrDefault(k => k.MaKhachHang == maKhach);

            // Kiểm tra nếu khách hàng tồn tại
            if (khachHang != null)
            {
                // Cập nhật trạng thái khách hàng thành 0
                khachHang.TrangThai = 0;
                db.SaveChanges();

                TempData["DungHoatDongKhach"] = "Khách hàng đã bị dừng hoạt động";
            }
            else
            {
                TempData["DungHoatDongKhach"] = "Khách hàng không tồn tại";
            }

            return RedirectToAction("DanhSachKhachHang", "HomeAdmin");
        }
        [Route("DanhSachKhachHangOff")]
        [HttpGet]
        public IActionResult DanhSachKhachHangOff(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 6;

            var danhSachKhachHangOff = db.TKhachHangs
                                         .Where(k => k.TrangThai == 0)
                                         .ToPagedList(pageNumber, pageSize);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_DanhSachKhachHang", danhSachKhachHangOff);
            }

            return View(danhSachKhachHangOff);
        }
        [Route("MoHoatDongKhach")]
        [HttpGet]
        public IActionResult MoHoatDongKhach(int maKhach)
        {
            TempData["MoHoatDongKhach"] = "";

            // Tìm khách hàng theo tên đăng nhập
            var khachHang = db.TKhachHangs.FirstOrDefault(k => k.MaKhachHang == maKhach);

            // Kiểm tra nếu khách hàng tồn tại
            if (khachHang != null)
            {
                // Cập nhật trạng thái khách hàng thành 1
                khachHang.TrangThai = 1;
                db.SaveChanges();

                TempData["MoHoatDongKhach"] = "Khách hàng được mở hoạt động";
            }
            else
            {
                TempData["MoHoatDongKhach"] = "Khách hàng không tồn tại";
            }

            return RedirectToAction("DanhSachKhachHang", "HomeAdmin");
        }
        [Route("TimKiemKhachHang")]
        [HttpGet]
        public IActionResult TimKiemKhachHang(int? maKhach)
        {
            if (maKhach == null)
            {
                ViewBag.Message = "Vui lòng nhập mã khách hàng để tìm kiếm.";
                return View("DanhSachKhachHang", new List<TKhachHang>()); // Trả về một danh sách trống nếu không nhập mã
            }

            // Tìm khách hàng dựa vào mã khách hàng
            var khachHang = db.TKhachHangs.FirstOrDefault(k => k.MaKhachHang == maKhach.Value);

            // Kiểm tra nếu tìm thấy khách hàng
            if (khachHang != null)
            {
                return View("DanhSachKhachHang", new List<TKhachHang> { khachHang });
            }
            else
            {
                ViewBag.Message = "Không tìm thấy khách hàng với mã này.";
                return View("DanhSachKhachHang", new List<TKhachHang>());
            }
        }
        [Route("DanhSachNhanVien")]
        [HttpGet]
        public IActionResult DanhSachNhanVien()
        {
            var listNhanVien = db.TNhanViens.ToList();

            return View(listNhanVien);
        }
        [Route("DungHoatDongNV")]
        [HttpGet]
        public IActionResult DungHoatDongNV(int maNV)
        {
            TempData["DungHoatDongNV"] = "";

            // Tìm khách hàng theo tên đăng nhập
            var nhanvien = db.TNhanViens.FirstOrDefault(k => k.MaNhanVien == maNV);

            // Kiểm tra nếu khách hàng tồn tại
            if (nhanvien != null)
            {
                // Cập nhật trạng thái khách hàng thành 0
                nhanvien.TrangThai = 0;
                db.SaveChanges();

                TempData["DungHoatDongNV"] = "Nhân viên đã bị dừng hoạt động";
            }
            else
            {
                TempData["DungHoatDongNV"] = "Nhân viên không tồn tại";
            }

            return RedirectToAction("DanhSachNHanVien", "HomeAdmin");
        }
        [Route("ThemTaiKhoanNhanVien")]
        [HttpGet]

        public IActionResult ThemTaiKhoanNhanVien()
        {
            return View();
        }
        [Route("ThemTaiKhoanNhanVien")]
        [HttpPost]
        public IActionResult ThemTaiKhoanNhanVien(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                    var acc = db.TTaiKhoans.FirstOrDefault(t => t.TenDangNhap == model.UserName);
                    if (acc != null)
                    {
                        ModelState.AddModelError("", "Tên đăng nhập đã tồn tại");
                        return View(model);
                    }
                    var taiKhoan = new TTaiKhoan
                    {
                        TenDangNhap = model.UserName,
                        MatKhau = model.Password,           
                        LoaiTaiKhoan = 1
                    };

                    // Thêm sản phẩm vào database
                    db.TTaiKhoans.Add(taiKhoan);
                    db.SaveChanges();
                    var nhanVien = new TNhanVien
                    {
                        HoTen = model.FullName,                   
                        Email = model.Email,
                        DiaChi = model.Address,
                        SoDienThoai = model.PhoneNumber,
                        TenDangNhap = model.UserName,
                        ChucVu = "Nhân viên",
                        TenHienThi = "a",
                        TrangThai = 1

                    };
                    db.TNhanViens.Add(nhanVien);
                    db.SaveChanges();
                    return RedirectToAction("DanhSachNhanVien");
                }
               
                   return View(model);
        }

    }
}
