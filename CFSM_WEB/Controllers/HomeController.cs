using CFSM_WEB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using X.PagedList;
using CFSM_WEB.ViewModels;

namespace CFSM_WEB.Controllers
{
    public class HomeController : Controller
    {
        QuanLyQuanCaPheContext db = new QuanLyQuanCaPheContext();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult IndexHome(int? page)
        {
            int pageSize = 6;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var lstDoAn = db.TDoAns
                            .Include(x => x.MaMenuNavigation)
                            .AsNoTracking().OrderBy(x => x.MaDoAn);

            if (lstDoAn == null)
            {
                return NotFound();
            }
            PagedList<TDoAn> lst = new PagedList<TDoAn>(lstDoAn, pageNumber, pageSize);
            return View(lst);
        }
        public IActionResult Service()
        {
            return View();
        }
        public IActionResult Blog(int? page)
        {
			int pageSize = 6;
			int pageNumber = page == null || page < 0 ? 1 : page.Value;
			var lstBlog = db.TBlogs.AsNoTracking().OrderBy(x => x.MaBlog);

			if (lstBlog == null)
			{
				return NotFound();
			}
			PagedList<TBlog> lst = new PagedList<TBlog>(lstBlog, pageNumber, pageSize);
			return View(lst);
		}
        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Menu(int? page)
        {
            int pageSize = 6;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var lstDoAn = db.TDoAns
                            .Include(x => x.MaMenuNavigation)
                            .AsNoTracking().OrderBy(x => x.MaDoAn);

            if (lstDoAn == null)
            {
                return NotFound();
            }
            PagedList<TDoAn> lst = new PagedList<TDoAn>(lstDoAn, pageNumber, pageSize);
            return View(lst);
        }
        public IActionResult ChiTietDoAn(int maDoAn)
        {            
            var sp = db.TDoAns.SingleOrDefault(x => x.MaDoAn == maDoAn);
            var listSPLQ = db.TDoAns.Where(x => x.MaMenu == sp.MaMenu && x.MaDoAn != sp.MaDoAn).ToList();
            var ChiTietSP = new ChiTietSPViewModel(sp, listSPLQ);
            return View(ChiTietSP);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
