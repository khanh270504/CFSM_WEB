using System;
using System.Collections.Generic;

namespace CFSM_WEB.Models;

public partial class THoaDon
{
    public int MaHoaDon { get; set; }

    public int? MaKhachHang { get; set; }

    public DateTime NgayLap { get; set; }

    public string HoTen { get; set; } = null!;

    public string DiaChi { get; set; } = null!;

    public string? SoDienThoai { get; set; }

    public string CachThanhToan { get; set; } = null!;

    public int? MaNhanVien { get; set; }

    public decimal? ThanhTien { get; set; }

    public string? TrangThaiHoaDon { get; set; }

    public string? GhiChu { get; set; }

    public virtual TKhachHang? MaKhachHangNavigation { get; set; }

    public virtual TNhanVien? MaNhanVienNavigation { get; set; }

    public virtual ICollection<TChiTietHd> TChiTietHds { get; set; } = new List<TChiTietHd>();
}
