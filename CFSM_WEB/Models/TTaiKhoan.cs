using System;
using System.Collections.Generic;

namespace CFSM_WEB.Models;

public partial class TTaiKhoan
{
    public string TenDangNhap { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string? Salt { get; set; }

    public int LoaiTaiKhoan { get; set; }

    public virtual ICollection<TKhachHang> TKhachHangs { get; set; } = new List<TKhachHang>();

    public virtual ICollection<TNhanVien> TNhanViens { get; set; } = new List<TNhanVien>();
}
