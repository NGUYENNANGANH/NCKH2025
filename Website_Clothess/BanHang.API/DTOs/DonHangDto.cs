using System.ComponentModel.DataAnnotations;

namespace BanHang.API.DTOs
{
    public class DonHangDto
    {
        public string Id_DonHang { get; set; } = string.Empty;
        public string Id_KH { get; set; } = string.Empty;
        public string TenKhachHang { get; set; } = string.Empty;
        public DateTime NgayDat { get; set; }
        public decimal TongTien { get; set; }
        public Models.TrangThaiDonHang TrangThai { get; set; }
        public string DiaChiGiaoHang { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public string? GhiChu { get; set; }
        public string PhuongThucThanhToan { get; set; } = string.Empty;
        public decimal PhiVanChuyen { get; set; } = 0;
        public string? MaGiamGia { get; set; }
        public List<ChiTietDonHangDto> ChiTietDonHangs { get; set; } = new List<ChiTietDonHangDto>();
    }

    public class ChiTietDonHangDto
    {
        public string Id_DonHang { get; set; } = string.Empty;
        public int Id_SanPham { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public decimal DonGia { get; set; }
        public decimal? GiaKhuyenMai { get; set; }
        public int SoLuong { get; set; }
        public int KichCo { get; set; }
        public string? MauSac { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;
    }

    public class CreateDonHangDto
    {
        [Required]
        [MaxLength(255)]
        public string DiaChiGiaoHang { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        [RegularExpression(@"^[0-9]{10,11}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SoDienThoai { get; set; } = string.Empty;

        public string? GhiChu { get; set; }

        [Required]
        [MaxLength(50)]
        public string PhuongThucThanhToan { get; set; } = "Tiền mặt";
    }

    public class UpdateDonHangStatusDto
    {
        [Required]
        public Models.TrangThaiDonHang TrangThai { get; set; }
    }
} 