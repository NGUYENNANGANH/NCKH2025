namespace BanHang.API.DTOs
{
    // DTO cho thống kê doanh thu
    public class DoanhThuDto
    {
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public decimal TongDoanhThu { get; set; }
        public int SoLuongDonHang { get; set; }
        public int SoLuongDonHangDaGiao { get; set; }
        public int SoLuongSanPhamDaBan { get; set; }
        public List<DoanhThuNgayDto> DoanhThuTheoNgay { get; set; } = new List<DoanhThuNgayDto>();
        public List<DoanhThuThangDto> DoanhThuTheoThang { get; set; } = new List<DoanhThuThangDto>();
    }

    // DTO cho doanh thu theo ngày
    public class DoanhThuNgayDto
    {
        public DateTime Ngay { get; set; }
        public decimal DoanhThu { get; set; }
        public int SoDonHang { get; set; }
    }

    // DTO cho doanh thu theo tháng
    public class DoanhThuThangDto
    {
        public int Nam { get; set; }
        public int Thang { get; set; }
        public decimal DoanhThu { get; set; }
        public int SoDonHang { get; set; }
        public string TenThang => $"Tháng {Thang}/{Nam}";
    }

    // DTO cho thống kê sản phẩm bán chạy
    public class SanPhamBanChayDto
    {
        public int Id_SanPham { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public string? HinhAnh { get; set; }
        public decimal GiaBan { get; set; }
        public int Id_DanhMuc { get; set; }
        public string TenDanhMuc { get; set; } = string.Empty;
        public int SoLuongBan { get; set; }
        public decimal DoanhThu { get; set; }
    }

    // DTO cho thống kê doanh thu theo danh mục
    public class DoanhThuDanhMucDto
    {
        public int Id_DanhMuc { get; set; }
        public string TenDanhMuc { get; set; } = string.Empty;
        public int SoLuongBan { get; set; }
        public decimal DoanhThu { get; set; }
        public int SoSanPham { get; set; }
        public decimal PhanTramDoanhThu { get; set; }
    }

    // DTO cho tổng quan thống kê
    public class TongQuanDto
    {
        public decimal TongDoanhThu { get; set; }
        public int TongSoDonHang { get; set; }
        public int SoDonHangDangXuLy { get; set; }
        public int TongSoSanPham { get; set; }
        public int SoSanPhamConHang { get; set; }
        public int SoSanPhamGanHet { get; set; }
        public int SoSanPhamHetHang { get; set; }
        public decimal DoanhThuHomNay { get; set; }
        public decimal DoanhThuTuanNay { get; set; }
        public decimal DoanhThuThangNay { get; set; }
    }
} 