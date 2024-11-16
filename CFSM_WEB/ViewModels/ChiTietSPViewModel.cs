using CFSM_WEB.Models;

namespace CFSM_WEB.ViewModels
{
    public class ChiTietSPViewModel
    {
        public TDoAn doAn { get; set; }
        public List<TDoAn> listDoAn { get; set; }
        public ChiTietSPViewModel(TDoAn doAn, List<TDoAn> listDoAn)
        {
            this.doAn = doAn;
            this.listDoAn = listDoAn;
        }
    }
}
