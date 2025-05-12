using System.ComponentModel.DataAnnotations;

namespace BanHang.API.DTOs
{
    public class DanhMucDto
    {
        public int Id_DanhMuc { get; set; }
        
        public string TenDanhMuc { get; set; } = string.Empty;
        
        public string? MoTa { get; set; }
        
        public bool TrangThai { get; set; }
        
        public int SoLuongSanPham { get; set; }
    }

    public class DanhMucCreateDto
    {
        [Required]
        [MaxLength(255)]
        public string TenDanhMuc { get; set; } = string.Empty;
        
        public string? MoTa { get; set; }
        
        public bool TrangThai { get; set; } = true;
    }

    public class DanhMucUpdateDto
    {
        [Required]
        [MaxLength(255)]
        public string TenDanhMuc { get; set; } = string.Empty;
        
        public string? MoTa { get; set; }
        
        public bool TrangThai { get; set; }
    }
} 