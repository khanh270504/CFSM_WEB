namespace CFSM_WEB.ViewModels
{
    public class CartItem
    {
        public int MaDoAn { get; set; }
        public string TenDoAn { get; set; } = null!;

        public decimal DonGia { get; set; }
        public string AnhDoAn { get; set; } = null!;

        public string MoTaDoAn { get; set; } = null!;

        public int SoLuong { get; set; }

        public decimal ThanhTien => DonGia * SoLuong;

    }
}
