using System;
using System.Collections.Generic;

namespace CFSM_WEB.Models;

public partial class TBlog
{
    public int MaBlog { get; set; }

    public string TieuDeBlog { get; set; } = null!;

    public string NoiDungBlog { get; set; } = null!;

    public string AnhTieuDe { get; set; } = null!;
}
