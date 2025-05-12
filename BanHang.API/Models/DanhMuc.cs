using System.ComponentModel.DataAnnotations;

namespace BanHang.API.Models
{
    public class DanhMuc
    {
        [Key]
        public int Id_DanhMuc { get; set; }
        
        [Required]
        
        [MaxLength(255)]
        public string TenDanhMuc { get; set; } = string.Empty;
        
        public string? MoTa { get; set; }
        
        public bool TrangThai { get; set; } = true;
        
        // Navigation Properties
        public virtual ICollection<SanPham>? SanPhams { get; set; }
    }
} 