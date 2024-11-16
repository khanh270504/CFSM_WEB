using System;
using System.Collections.Generic;

namespace CFSM_WEB.Models;

public partial class TNhanVien
{
    public int MaNhanVien { get; set; }

    public string HoTen { get; set; } = null!;

    public string TenHienThi { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? DiaChi { get; set; }

    public string? SoDienThoai { get; set; }

    public string? ChucVu { get; set; }

    public int TrangThai { get; set; }

    public string TenDangNhap { get; set; } = null!;

    public virtual ICollection<THoaDon> THoaDons { get; set; } = new List<THoaDon>();

    public virtual TTaiKhoan TenDangNhapNavigation { get; set; } = null!;
}
