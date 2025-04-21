using BanHang.API.Models;
using System.ComponentModel.DataAnnotations;

namespace BanHang.API.DTOs
{
    public class VoucherDto
    {
        public string Id_Voucher { get; set; } = string.Empty;
        public string MaVoucher { get; set; } = string.Empty;
        public string TenVoucher { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public LoaiVoucher LoaiVoucher { get; set; }
        public decimal GiaTri { get; set; }
        public decimal? GiaTriToiDa { get; set; }
        public decimal? GiaTriDonHangToiThieu { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public int? SoLuong { get; set; }
        public int SoLuongDaSuDung { get; set; }
        public bool ApDungChoDonHangDauTien { get; set; }
        public bool ApDungChoTatCaSanPham { get; set; }
        public bool TrangThai { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        
        // Thuộc tính phái sinh
        public bool ConHieuLuc => NgayBatDau <= DateTime.Now && NgayKetThuc >= DateTime.Now && TrangThai;
        public bool ConSoLuong => !SoLuong.HasValue || SoLuongDaSuDung < SoLuong.Value;
    }
    
    public class CreateVoucherDto
    {
        [Required]
        [MaxLength(50)]
        public string MaVoucher { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(255)]
        public string TenVoucher { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? MoTa { get; set; }
        
        public LoaiVoucher LoaiVoucher { get; set; } = LoaiVoucher.GiamTheoPhanTram;
        
        [Required]
        [Range(0.01, 100, ErrorMessage = "Giá trị phải từ 0.01 đến 100")]
        public decimal GiaTri { get; set; }
        
        public decimal? GiaTriToiDa { get; set; }
        
        public decimal? GiaTriDonHangToiThieu { get; set; }
        
        [Required]
        public DateTime NgayBatDau { get; set; }
        
        [Required]
        public DateTime NgayKetThuc { get; set; }
        
        public int? SoLuong { get; set; }
        
        public bool ApDungChoDonHangDauTien { get; set; } = false;
        
        public bool ApDungChoTatCaSanPham { get; set; } = true;
        
        public bool TrangThai { get; set; } = true;
        
        // Danh sách ID danh mục áp dụng (nếu không áp dụng cho tất cả sản phẩm)
        public List<int>? DanhMucIds { get; set; }
        
        // Danh sách ID sản phẩm áp dụng (nếu không áp dụng cho tất cả sản phẩm)
        public List<int>? SanPhamIds { get; set; }
    }
    
    public class UpdateVoucherDto
    {
        [MaxLength(255)]
        public string? TenVoucher { get; set; }
        
        [MaxLength(500)]
        public string? MoTa { get; set; }
        
        public LoaiVoucher? LoaiVoucher { get; set; }
        
        [Range(0.01, 100, ErrorMessage = "Giá trị phải từ 0.01 đến 100")]
        public decimal? GiaTri { get; set; }
        
        public decimal? GiaTriToiDa { get; set; }
        
        public decimal? GiaTriDonHangToiThieu { get; set; }
        
        public DateTime? NgayBatDau { get; set; }
        
        public DateTime? NgayKetThuc { get; set; }
        
        public int? SoLuong { get; set; }
        
        public bool? ApDungChoDonHangDauTien { get; set; }
        
        public bool? ApDungChoTatCaSanPham { get; set; }
        
        public bool? TrangThai { get; set; }
        
        // Danh sách ID danh mục áp dụng (nếu không áp dụng cho tất cả sản phẩm)
        public List<int>? DanhMucIds { get; set; }
        
        // Danh sách ID sản phẩm áp dụng (nếu không áp dụng cho tất cả sản phẩm)
        public List<int>? SanPhamIds { get; set; }
    }
    
    public class ApplyVoucherDto
    {
        [Required]
        [MaxLength(50)]
        public string MaVoucher { get; set; } = string.Empty;
    }
    
    public class VoucherAppliedDto
    {
        public string Id_Voucher { get; set; } = string.Empty;
        public string MaVoucher { get; set; } = string.Empty;
        public string TenVoucher { get; set; } = string.Empty;
        public LoaiVoucher LoaiVoucher { get; set; }
        public decimal GiaTri { get; set; }
        public decimal GiaTriApDung { get; set; } // Số tiền thực tế được giảm
        public decimal TongTienTruocGiam { get; set; }
        public decimal TongTienSauGiam { get; set; }
    }
} 