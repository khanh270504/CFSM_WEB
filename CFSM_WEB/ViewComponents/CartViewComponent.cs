using CFSM_WEB.Helpers;
using CFSM_WEB.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CFSM_WEB.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var count = HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();

            
            return View("CartPanel", new CartModel
            {
                Quantity = count.Sum(p => p.SoLuong),
                Total = count.Sum(p => (int)p.ThanhTien)
            });
        }
    }
}
