using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanHang.API.Models
{
    public enum LoaiVoucher
    {
        GiamTheoPhanTram = 0,
        GiamTheoSoTien = 1,
        FreeShip = 2
    }

    public class Voucher
    {
        [Key]
        public string Id_Voucher { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [MaxLength(50)]
        public string MaVoucher { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(255)]
        public string TenVoucher { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? MoTa { get; set; }
        
        public LoaiVoucher LoaiVoucher { get; set; } = LoaiVoucher.GiamTheoPhanTram;
        
        public decimal GiaTri { get; set; } // Phần trăm hoặc số tiền tùy thuộc vào loại voucher
        
        public decimal? GiaTriToiDa { get; set; } // Giá trị tối đa cho voucher loại phần trăm
        
        public decimal? GiaTriDonHangToiThieu { get; set; } // Giá trị tối thiểu của đơn hàng để áp dụng voucher
        
        public DateTime NgayBatDau { get; set; }
        
        public DateTime NgayKetThuc { get; set; }
        
        public int? SoLuong { get; set; } // Null là không giới hạn
        
        public int SoLuongDaSuDung { get; set; } = 0;
        
        public bool ApDungChoDonHangDauTien { get; set; } = false;
        
        public bool ApDungChoTatCaSanPham { get; set; } = true;
        
        public bool TrangThai { get; set; } = true;
        
        [ForeignKey("NguoiTao")]
        public string? Id_NguoiTao { get; set; }
        
        public DateTime NgayTao { get; set; } = DateTime.Now;
        
        public DateTime? NgayCapNhat { get; set; }
        
        // Navigation Properties
        public virtual ApplicationUser? NguoiTao { get; set; }
        public virtual ICollection<VoucherSuDung>? LichSuSuDung { get; set; }
        public virtual ICollection<VoucherDanhMuc>? VoucherDanhMucs { get; set; }
        public virtual ICollection<VoucherSanPham>? VoucherSanPhams { get; set; }
    }
    
    // Lưu lịch sử sử dụng voucher
    public class VoucherSuDung
    {
        [Key]
        public string Id_VoucherSuDung { get; set; } = Guid.NewGuid().ToString();
        
        [ForeignKey("Voucher")]
        public string Id_Voucher { get; set; } = string.Empty;
        
        [ForeignKey("DonHang")]
        public string Id_DonHang { get; set; } = string.Empty;
        
        [ForeignKey("User")]
        public string Id_User { get; set; } = string.Empty;
        
        public DateTime NgaySuDung { get; set; } = DateTime.Now;
        
        public decimal GiaTriApDung { get; set; } // Giá trị thực tế được giảm
        
        // Navigation Properties
        public virtual Voucher? Voucher { get; set; }
        public virtual DonHang? DonHang { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
    
    // Áp dụng voucher cho danh mục sản phẩm
    public class VoucherDanhMuc
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Voucher")]
        public string Id_Voucher { get; set; } = string.Empty;
        
        [Key, Column(Order = 1)]
        [ForeignKey("DanhMuc")]
        public int Id_DanhMuc { get; set; }
        
        // Navigation Properties
        public virtual Voucher? Voucher { get; set; }
        public virtual DanhMuc? DanhMuc { get; set; }
    }
    
    // Áp dụng voucher cho sản phẩm cụ thể
    public class VoucherSanPham
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Voucher")]
        public string Id_Voucher { get; set; } = string.Empty;
        
        [Key, Column(Order = 1)]
        [ForeignKey("SanPham")]
        public int Id_SanPham { get; set; }
        
        // Navigation Properties
        public virtual Voucher? Voucher { get; set; }
        public virtual SanPham? SanPham { get; set; }
    }
} 