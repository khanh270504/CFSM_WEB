using System;
using System.Collections.Generic;

namespace CFSM_WEB.Models;

public partial class TChiTietHd
{
    public int MaCthd { get; set; }

    public int MaHoaDon { get; set; }

    public int MaDoAn { get; set; }

    public int SoLuong { get; set; }

    public decimal DonGia { get; set; }

    public decimal? TongTien { get; set; }

    public virtual TDoAn MaDoAnNavigation { get; set; } = null!;

    public virtual THoaDon MaHoaDonNavigation { get; set; } = null!;
}
