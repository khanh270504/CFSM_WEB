using System;
using System.Collections.Generic;

namespace CFSM_WEB.Models;

public partial class TBan
{
    public int MaBan { get; set; }

    public string? TenNguoiDat { get; set; }

    public string TrangThai { get; set; } = null!;

    public DateOnly ThoiGianDb { get; set; }

    public virtual ICollection<THoaDon> THoaDons { get; set; } = new List<THoaDon>();
}
