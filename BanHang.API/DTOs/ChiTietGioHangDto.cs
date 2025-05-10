namespace BanHang.API.DTOs
{
    public class GioHangDto
    {
        public string Id_GioHang { get; set; } = string.Empty;
        public string Id_User { get; set; } = string.Empty;
        public DateTime NgayTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public List<ChiTietGioHangDto> ChiTietGioHangs { get; set; } = new List<ChiTietGioHangDto>();
        public decimal TongTien => ChiTietGioHangs.Sum(c => c.ThanhTien);
        public int TongSoLuong => ChiTietGioHangs.Sum(c => c.SoLuong);
    }

    public class ChiTietGioHangDto
    {
        public string Id_GioHang { get; set; } = string.Empty;
        public int Id_SanPham { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public decimal GiaBan { get; set; }
        public decimal? GiaKhuyenMai { get; set; }
        public string? HinhAnh { get; set; }
        public int SoLuong { get; set; }
        public int KichCo { get; set; }
        public string? MauSac { get; set; }
        public decimal ThanhTien => SoLuong * (GiaKhuyenMai ?? GiaBan);
    }

    public class AddToCartDto
    {
        public int Id_SanPham { get; set; }
        public int SoLuong { get; set; } = 1;
        public int KichCo { get; set; }
        public string? MauSac { get; set; }
    }

    public class UpdateCartItemDto
    {
        public int Id_SanPham { get; set; }
        public int SoLuong { get; set; }
        public int KichCo { get; set; }
        public string? MauSac { get; set; }
    }
} 