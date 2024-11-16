using CFSM_WEB.Models;
using CFSM_WEB.Models.DoAnModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CFSM_WEB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoAnAPIController : ControllerBase
    {
        QuanLyQuanCaPheContext db = new QuanLyQuanCaPheContext();
        [HttpGet]
        public IEnumerable<DoAn> GetAllProduct()
        {
            var sanPham = (from d in db.TDoAns
                           select new DoAn
                           {
                               MaDoAn = d.MaDoAn,
                               TenDoAn = d.TenDoAn,
                               MoTaDoAn = d.MoTaDoAn,
                               MaMenu = d.MaMenu,
                               AnhDoAn = d.AnhDoAn,
                               DonGia = d.DonGia
                           }).ToList();
            return sanPham;
        }
        [HttpGet("{maloai}")]
        public IEnumerable<DoAn> GetProductByCategory(int maloai)
        {
            var sanPham = (from d in db.TDoAns
                           where d.MaMenu == maloai
                           select new DoAn
                           {
                               MaDoAn = d.MaDoAn,
                               TenDoAn = d.TenDoAn,
                               MoTaDoAn = d.MoTaDoAn,
                               MaMenu = d.MaMenu,
                               AnhDoAn = d.AnhDoAn,
                               DonGia = d.DonGia
                           }).ToList();
            return sanPham;
        }
    }
}
