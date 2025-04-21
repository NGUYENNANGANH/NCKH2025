using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanHang.API.Models
{
    public enum TrangThaiDonHang
    {
        ChoXacNhan = 0,
        DangXuLy = 1,
        DangGiao = 2,
        DaGiao = 3,
        DaHuy = 4
    }

    public class DonHang
    {
        [Key]
        public string Id_DonHang { get; set; } = Guid.NewGuid().ToString();
        
        [ForeignKey("KhachHang")]
        public string Id_KH { get; set; } = string.Empty;
        
        public DateTime NgayDat { get; set; } = DateTime.Now;
        
        public decimal TongTien { get; set; }
        
        public TrangThaiDonHang TrangThai { get; set; } = TrangThaiDonHang.ChoXacNhan;
        
        [MaxLength(255)]
        public string DiaChiGiaoHang { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string SoDienThoai { get; set; } = string.Empty;
        
        public string? GhiChu { get; set; }
        
        [MaxLength(50)]
        public string PhuongThucThanhToan { get; set; } = "Tiền mặt";
        
        public decimal PhiVanChuyen { get; set; } = 0;
        
        [MaxLength(50)]
        public string? MaGiamGia { get; set; }
        
        // Navigation Properties
        public virtual ApplicationUser? KhachHang { get; set; }
        public virtual ICollection<ChiTietDonHang>? ChiTietDonHangs { get; set; }
        public virtual HoaDon? HoaDon { get; set; }
    }
} 