
using CFSM_WEB.Models;
using Microsoft.AspNetCore.Mvc;

namespace DemoWebQLQCF.ViewComponents
{
	public class MenuViewComponent : ViewComponent
	{
		private QuanLyQuanCaPheContext _context;
		public MenuViewComponent(QuanLyQuanCaPheContext context)
		{
			_context = context;
		}
		public IViewComponentResult Invoke()
		{
			var listMenu = _context.TMenus.OrderBy(x => x.TenMenu);
			return View(listMenu);
		}
	}
}
