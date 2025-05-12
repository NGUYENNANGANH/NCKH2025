using System.ComponentModel.DataAnnotations;

namespace BanHang.API.DTOs
{
    public class DanhGiaDTO
    {
        public string Id_DanhGia { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public int Vote { get; set; }
        public string Username { get; set; } = string.Empty;
        public int Id_SanPham { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public string? HinhAnh { get; set; }
        public DateTime Date { get; set; }
    }

    public class CreateDanhGiaDTO
    {
        [Required]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Comment must be between 10 and 500 characters")]
        public string Comment { get; set; } = string.Empty;

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Vote { get; set; }

        [Required]
        public int Id_SanPham { get; set; }
    }

    public class UpdateDanhGiaDTO
    {
        [Required]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Comment must be between 10 and 500 characters")]
        public string Comment { get; set; } = string.Empty;

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Vote { get; set; }
    }

    public class SanPhamChuaDanhGiaDTO
    {
        public int Id_SanPham { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public string? HinhAnh { get; set; }
        public decimal GiaBan { get; set; }
        public int SoLuong { get; set; }
        public string Id_DonHang { get; set; } = string.Empty;
        public DateTime NgayMua { get; set; }
    }

    public class SanPhamDaDanhGiaDTO
    {
        public string Id_DanhGia { get; set; } = string.Empty;
        public int Id_SanPham { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public string? HinhAnh { get; set; }
        public string Comment { get; set; } = string.Empty;
        public int Vote { get; set; }
        public DateTime NgayDanhGia { get; set; }
    }

    public class SanPhamDaMuaDTO
    {
        public int Id_SanPham { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public string? HinhAnh { get; set; }
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }
        public string Id_DonHang { get; set; } = string.Empty;
        public DateTime NgayDat { get; set; }
    }
} 