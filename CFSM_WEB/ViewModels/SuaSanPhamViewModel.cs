using CFSM_WEB.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CFSM_WEB.ViewModels
{
    public class SuaSanPhamViewModel
    {
        public TDoAn SanPham { get; set; }
        public IEnumerable<SelectListItem> MaMenuList { get; set; }
    }

}
