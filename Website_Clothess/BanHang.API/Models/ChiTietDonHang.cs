using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanHang.API.Models
{
    public class ChiTietDonHang
    {
        [Key, Column(Order = 0)]
        [ForeignKey("DonHang")]
        public string Id_DonHang { get; set; } = string.Empty;
        
        [Key, Column(Order = 1)]
        [ForeignKey("SanPham")]
        public int Id_SanPham { get; set; }
        
        [MaxLength(255)]
        public string TenSanPham { get; set; } = string.Empty;
        
        public decimal DonGia { get; set; }
        
        public decimal? GiaKhuyenMai { get; set; }
        
        public int SoLuong { get; set; }
        
        public int KichCo { get; set; }
        
        [MaxLength(50)]
        public string? MauSac { get; set; }
        
        // Navigation Properties
        public virtual DonHang? DonHang { get; set; }
        public virtual SanPham? SanPham { get; set; }
    }
} 