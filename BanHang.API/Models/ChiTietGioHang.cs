using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanHang.API.Models
{
    public class ChiTietGioHang
    {
        [Key, Column(Order = 0)]
        [ForeignKey("GioHang")]
        public string Id_GioHang { get; set; } = string.Empty;
        
        [Key, Column(Order = 1)]
        [ForeignKey("SanPham")]
        public int Id_SanPham { get; set; }
        
        public int SoLuong { get; set; } = 1;
        
        public int KichCo { get; set; }
        
        [MaxLength(50)]
        public string? MauSac { get; set; }
        
        public decimal DonGia { get; set; }
        
        // Navigation Properties
        public virtual GioHang? GioHang { get; set; }
        public virtual SanPham? SanPham { get; set; }
    }
} 