using System;
using System.Collections.Generic;

namespace CFSM_WEB.Models;

public partial class TDoAn
{
    public int MaDoAn { get; set; }

    public string TenDoAn { get; set; } = null!;

    public decimal DonGia { get; set; }

    public string MoTaDoAn { get; set; } = null!;

    public string? AnhDoAn { get; set; }

    public int? MaMenu { get; set; }

    public virtual TMenu? MaMenuNavigation { get; set; }

    public virtual ICollection<TChiTietHd> TChiTietHds { get; set; } = new List<TChiTietHd>();
}
