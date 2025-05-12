using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanHang.API.Models
{
    public enum TrangThaiSanPham
    {
        DangBan = 1,      // Đang được bán bình thường
        NgungBan = 2,     // Tạm ngừng kinh doanh, có thể bán lại sau
        HetHang = 3,      // Hết hàng, không thể mua
        SapRaMat = 4,     // Sắp ra mắt, chờ ngày mở bán
        NgungKinhDoanh = 5 // Đã ngừng kinh doanh hoàn toàn
    }

    public class SanPham
    {
        [Key]
        public int Id_SanPham { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string TenSanPham { get; set; } = string.Empty;
        
        public string? MoTa { get; set; }
        
        [Required]
        public decimal GiaBan { get; set; }
        
        public int SoLuongTon { get; set; }
        
        public string? HinhAnh { get; set; }
        
        public string? HinhAnhPhu { get; set; }
        
        [MaxLength(50)]
        public string? SKU { get; set; }
        
        // Trạng thái sản phẩm, lưu dưới dạng số nguyên trong database
        public int TrangThaiSanPham_Value { get; set; } = (int)TrangThaiSanPham.DangBan;
        
        // Thuộc tính cung cấp interface để làm việc với enum
        [NotMapped]
        public TrangThaiSanPham TrangThaiSanPham 
        { 
            get => (TrangThaiSanPham)TrangThaiSanPham_Value;
            set 
            {
                TrangThaiSanPham_Value = (int)value;
            }
        }
        
        public DateTime NgayTao { get; set; } = DateTime.Now;
        
        public DateTime? NgayCapNhat { get; set; }
        
        [ForeignKey("DanhMuc")]
        public int Id_DanhMuc { get; set; }
        
        public int SoLuongDaBan { get; set; } = 0;
        
        // Navigation Properties
        public virtual DanhMuc? DanhMuc { get; set; }
        public virtual ICollection<ChiTietGioHang>? ChiTietGioHangs { get; set; }
        public virtual ICollection<ChiTietDonHang>? ChiTietDonHangs { get; set; }
        public virtual ICollection<DanhGia>? DanhGias { get; set; }
        public virtual ICollection<SanPham_KhuyenMai>? SanPham_KhuyenMais { get; set; }
    }
} 