using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanHang.API.Models
{
    public class SanPham_KhuyenMai
    {
        [Key, Column(Order = 0)]
        [ForeignKey("KhuyenMai")]
        public int Id_KhuyenMai { get; set; }
        
        [Key, Column(Order = 1)]
        [ForeignKey("SanPham")]
        public int Id_SanPham { get; set; }
        
        public DateTime NgayApDung { get; set; }
        
        // Thuộc tính bổ sung
        public DateTime? NgayKetThuc { get; set; }
        
        public int? MucGiamGiaRieng { get; set; } // Phần trăm giảm giá riêng cho sản phẩm này, null thì sử dụng % giảm mặc định của khuyến mãi
        
        public int? SoLuongGioiHan { get; set; } // Số lượng sản phẩm được áp dụng khuyến mãi, null là không giới hạn
        
        public bool TrangThai { get; set; } = true; // true: đang áp dụng, false: tạm dừng
        
        public int ThuTuUuTien { get; set; } = 0; // Số càng lớn càng ưu tiên cao
        
        [MaxLength(500)]
        public string? GhiChu { get; set; }
        
        [ForeignKey("NguoiTao")]
        public string? Id_NguoiTao { get; set; }
        
        [ForeignKey("NguoiCapNhat")]
        public string? Id_NguoiCapNhat { get; set; }
        
        public DateTime NgayTao { get; set; } = DateTime.Now;
        
        public DateTime? NgayCapNhat { get; set; }
        
        // Navigation Properties
        public virtual KhuyenMai? KhuyenMai { get; set; }
        public virtual SanPham? SanPham { get; set; }
        public virtual ApplicationUser? NguoiTao { get; set; }
        public virtual ApplicationUser? NguoiCapNhat { get; set; }
    }
} 