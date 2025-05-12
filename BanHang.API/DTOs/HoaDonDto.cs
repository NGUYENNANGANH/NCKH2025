namespace BanHang.API.DTOs
{
    public class HoaDonDto
    {
        public int Id_HoaDon { get; set; }
        public DateTime CreateAt { get; set; }
        public string Id_User { get; set; } = string.Empty;
        public string TenNguoiLap { get; set; } = string.Empty;
        public string Id_DonHang { get; set; } = string.Empty;
        public DateTime? DateXuat { get; set; }
        public DonHangDto? DonHang { get; set; }
    }
} 