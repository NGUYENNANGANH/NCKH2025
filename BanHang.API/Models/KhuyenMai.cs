using System.ComponentModel.DataAnnotations;

namespace BanHang.API.Models
{
    public class KhuyenMai
    {
        [Key]
        public int Id_KhuyenMai { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string TenKhuyenMai { get; set; } = string.Empty;
        
        public DateTime NgayBatDau { get; set; }
        
        public DateTime NgayKetThuc { get; set; }
        
        public int PhanTramGiam { get; set; }
        
        public bool ApDungToanHeThong { get; set; }
        
        public bool TrangThai { get; set; } = true;
        
        public string? HinhAnh { get; set; }
        
        // Navigation Properties
        public virtual ICollection<SanPham_KhuyenMai>? SanPham_KhuyenMais { get; set; }
    }
} 