using System.ComponentModel.DataAnnotations;
using BanHang.API.Models;

namespace BanHang.API.DTOs
{
    public class SanPhamDto
    {
        public int Id_SanPham { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public decimal GiaBan { get; set; }
        public decimal? GiaKhuyenMai { get; set; }
        
        // Thuộc tính tính toán để kiểm tra có khuyến mãi hay không
        public bool CoKhuyenMai => GiaKhuyenMai.HasValue && GiaKhuyenMai < GiaBan;
        
        // Thuộc tính tính toán phần trăm giảm giá
        public int? PhanTramGiam => CoKhuyenMai ? (int)((1 - GiaKhuyenMai.Value / GiaBan) * 100) : null;
        
        public int SoLuongTon { get; set; }
        public string? HinhAnh { get; set; }
        public string? HinhAnhPhu { get; set; }
        public string? SKU { get; set; }
        public TrangThaiSanPham TrangThaiSanPham { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public int Id_DanhMuc { get; set; }
        public string TenDanhMuc { get; set; } = string.Empty;
        public int SoLuongDaBan { get; set; }
        
        // Danh sách khuyến mãi đang áp dụng (có thể thêm nếu cần)
        public List<KhuyenMaiApDungDto>? KhuyenMais { get; set; }
    }

    public class KhuyenMaiApDungDto
    {
        public int Id_KhuyenMai { get; set; }
        public string TenKhuyenMai { get; set; } = string.Empty;
        public decimal TyLeGiam { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
    }

    public class SanPhamCreateDto
    {
        [Required]
        [MaxLength(255)]
        public string TenSanPham { get; set; } = string.Empty;
        
        public string? MoTa { get; set; }
        
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Giá bán phải lớn hơn hoặc bằng 0")]
        public decimal GiaBan { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn phải lớn hơn hoặc bằng 0")]
        public int SoLuongTon { get; set; }
        
        public string? HinhAnh { get; set; }
        
        public string? HinhAnhPhu { get; set; }

        [MaxLength(50)]
        public string? SKU { get; set; }
        
        public TrangThaiSanPham TrangThaiSanPham { get; set; } = TrangThaiSanPham.DangBan;
        
        [Required]
        public int Id_DanhMuc { get; set; }
    }

    public class SanPhamUpdateDto
    {
        [Required]
        [MaxLength(255)]
        public string TenSanPham { get; set; } = string.Empty;
        
        public string? MoTa { get; set; }
        
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Giá bán phải lớn hơn hoặc bằng 0")]
        public decimal GiaBan { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn phải lớn hơn hoặc bằng 0")]
        public int SoLuongTon { get; set; }
        
        public string? HinhAnh { get; set; }
        
        public string? HinhAnhPhu { get; set; }

        [MaxLength(50)]
        public string? SKU { get; set; }
        
        public TrangThaiSanPham TrangThaiSanPham { get; set; }
        
        [Required]
        public int Id_DanhMuc { get; set; }
    }
} 