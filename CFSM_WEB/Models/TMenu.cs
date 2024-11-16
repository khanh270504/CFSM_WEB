using System;
using System.Collections.Generic;

namespace CFSM_WEB.Models;

public partial class TMenu
{
    public int MaMenu { get; set; }

    public string TenMenu { get; set; } = null!;

    public virtual ICollection<TDoAn> TDoAns { get; set; } = new List<TDoAn>();
}
