using BanHang.API.Data;
using BanHang.API.DTOs;
using BanHang.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace BanHang.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager")]
    public class ThongKeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ThongKeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ThongKe/doanh-thu
        [HttpGet("doanh-thu")]
        public async Task<ActionResult<DoanhThuDto>> GetDoanhThu([FromQuery] DateTime? tuNgay, [FromQuery] DateTime? denNgay)
        {
            var startDate = tuNgay ?? DateTime.Now.AddDays(-30);
            var endDate = denNgay ?? DateTime.Now;
            
            // Đảm bảo ngày bắt đầu <= ngày kết thúc
            if (startDate > endDate)
            {
                var temp = startDate;
                startDate = endDate;
                endDate = temp;
            }
            
            // Thêm 1 ngày cho ngày kết thúc để đảm bảo tính cả đơn hàng trong ngày kết thúc
            endDate = endDate.AddDays(1).AddTicks(-1);

            // Lấy tất cả đơn hàng trong khoảng thời gian
            var donHangs = await _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                .Where(d => d.NgayDat >= startDate && d.NgayDat <= endDate)
                .ToListAsync();

            // Tổng doanh thu
            decimal tongDoanhThu = donHangs.Sum(d => d.TongTien);
            
            // Số lượng đơn hàng
            int soLuongDonHang = donHangs.Count;
            
            // Số lượng đơn hàng đã hoàn thành
            int soLuongDonHangDaGiao = donHangs.Count(d => d.TrangThai == TrangThaiDonHang.DaGiao);
            
            // Số lượng sản phẩm đã bán
            int soLuongSanPhamDaBan = donHangs.Sum(d => d.ChiTietDonHangs?.Sum(c => c.SoLuong) ?? 0);
            
            // Doanh thu theo ngày
            var doanhThuTheoNgay = donHangs
                .GroupBy(d => d.NgayDat.Date)
                .Select(g => new DoanhThuNgayDto
                {
                    Ngay = g.Key,
                    DoanhThu = g.Sum(d => d.TongTien),
                    SoDonHang = g.Count()
                })
                .OrderBy(d => d.Ngay)
                .ToList();
            
            // Doanh thu theo tháng
            var doanhThuTheoThang = donHangs
                .GroupBy(d => new { d.NgayDat.Year, d.NgayDat.Month })
                .Select(g => new DoanhThuThangDto
                {
                    Nam = g.Key.Year,
                    Thang = g.Key.Month,
                    DoanhThu = g.Sum(d => d.TongTien),
                    SoDonHang = g.Count()
                })
                .OrderBy(d => d.Nam)
                .ThenBy(d => d.Thang)
                .ToList();

            // Kết quả
            var result = new DoanhThuDto
            {
                TuNgay = startDate,
                DenNgay = endDate,
                TongDoanhThu = tongDoanhThu,
                SoLuongDonHang = soLuongDonHang,
                SoLuongDonHangDaGiao = soLuongDonHangDaGiao,
                SoLuongSanPhamDaBan = soLuongSanPhamDaBan,
                DoanhThuTheoNgay = doanhThuTheoNgay,
                DoanhThuTheoThang = doanhThuTheoThang
            };

            return Ok(result);
        }

        // GET: api/ThongKe/san-pham-ban-chay
        [HttpGet("san-pham-ban-chay")]
        public async Task<ActionResult<IEnumerable<SanPhamBanChayDto>>> GetSanPhamBanChay(
            [FromQuery] DateTime? tuNgay, 
            [FromQuery] DateTime? denNgay, 
            [FromQuery] int? top,
            [FromQuery] int? danhMucId)
        {
            var startDate = tuNgay ?? DateTime.Now.AddDays(-30);
            var endDate = denNgay ?? DateTime.Now;
            var limit = top ?? 10; // Mặc định lấy top 10
            
            // Đảm bảo ngày bắt đầu <= ngày kết thúc
            if (startDate > endDate)
            {
                var temp = startDate;
                startDate = endDate;
                endDate = temp;
            }
            
            // Thêm 1 ngày cho ngày kết thúc để đảm bảo tính cả đơn hàng trong ngày kết thúc
            endDate = endDate.AddDays(1).AddTicks(-1);

            // Truy vấn cơ bản
            var query = _context.ChiTietDonHangs
                .Include(c => c.DonHang)
                .Include(c => c.SanPham)
                .ThenInclude(s => s.DanhMuc)
                .Where(c => c.DonHang != null && c.DonHang.NgayDat >= startDate && c.DonHang.NgayDat <= endDate);

            // Lọc theo danh mục nếu có
            if (danhMucId.HasValue)
            {
                query = query.Where(c => c.SanPham != null && c.SanPham.Id_DanhMuc == danhMucId.Value);
            }

            // Thực hiện truy vấn
            var sanPhamBanChay = await query
                .GroupBy(c => new { c.Id_SanPham, c.SanPham.TenSanPham, c.SanPham.HinhAnh, c.SanPham.GiaBan, DanhMucId = c.SanPham.Id_DanhMuc, TenDanhMuc = c.SanPham.DanhMuc.TenDanhMuc })
                .Select(g => new SanPhamBanChayDto
                {
                    Id_SanPham = g.Key.Id_SanPham,
                    TenSanPham = g.Key.TenSanPham,
                    HinhAnh = g.Key.HinhAnh,
                    GiaBan = g.Key.GiaBan,
                    Id_DanhMuc = g.Key.DanhMucId,
                    TenDanhMuc = g.Key.TenDanhMuc,
                    SoLuongBan = g.Sum(c => c.SoLuong),
                    DoanhThu = g.Sum(c => c.SoLuong * c.DonGia)
                })
                .OrderByDescending(s => s.SoLuongBan)
                .Take(limit)
                .ToListAsync();

            return Ok(sanPhamBanChay);
        }

        // GET: api/ThongKe/doanh-thu-theo-danh-muc
        [HttpGet("doanh-thu-theo-danh-muc")]
        public async Task<ActionResult<IEnumerable<DoanhThuDanhMucDto>>> GetDoanhThuTheoDanhMuc(
            [FromQuery] DateTime? tuNgay, 
            [FromQuery] DateTime? denNgay)
        {
            var startDate = tuNgay ?? DateTime.Now.AddDays(-30);
            var endDate = denNgay ?? DateTime.Now;
            
            // Đảm bảo ngày bắt đầu <= ngày kết thúc
            if (startDate > endDate)
            {
                var temp = startDate;
                startDate = endDate;
                endDate = temp;
            }
            
            // Thêm 1 ngày cho ngày kết thúc để đảm bảo tính cả đơn hàng trong ngày kết thúc
            endDate = endDate.AddDays(1).AddTicks(-1);

            var doanhThuTheoDanhMuc = await _context.ChiTietDonHangs
                .Include(c => c.DonHang)
                .Include(c => c.SanPham)
                .ThenInclude(s => s.DanhMuc)
                .Where(c => c.DonHang != null && c.DonHang.NgayDat >= startDate && c.DonHang.NgayDat <= endDate && c.SanPham != null)
                .GroupBy(c => new { c.SanPham.Id_DanhMuc, c.SanPham.DanhMuc.TenDanhMuc })
                .Select(g => new DoanhThuDanhMucDto
                {
                    Id_DanhMuc = g.Key.Id_DanhMuc,
                    TenDanhMuc = g.Key.TenDanhMuc,
                    SoLuongBan = g.Sum(c => c.SoLuong),
                    DoanhThu = g.Sum(c => c.SoLuong * c.DonGia),
                    SoSanPham = g.Select(c => c.Id_SanPham).Distinct().Count()
                })
                .OrderByDescending(d => d.DoanhThu)
                .ToListAsync();

            // Tính phần trăm doanh thu trên tổng
            decimal tongDoanhThu = doanhThuTheoDanhMuc.Sum(d => d.DoanhThu);
            if (tongDoanhThu > 0)
            {
                foreach (var danhMuc in doanhThuTheoDanhMuc)
                {
                    danhMuc.PhanTramDoanhThu = Math.Round((danhMuc.DoanhThu / tongDoanhThu) * 100, 2);
                }
            }

            return Ok(doanhThuTheoDanhMuc);
        }
        
        // GET: api/ThongKe/tong-quan
        [HttpGet("tong-quan")]
        public async Task<ActionResult<TongQuanDto>> GetTongQuan()
        {
            // Tổng doanh thu
            decimal tongDoanhThu = await _context.DonHangs
                .Where(d => d.TrangThai == TrangThaiDonHang.DaGiao)
                .SumAsync(d => d.TongTien);
                
            // Tổng số đơn hàng
            int tongSoDonHang = await _context.DonHangs.CountAsync();
            
            // Số đơn hàng đang xử lý
            int soDonHangDangXuLy = await _context.DonHangs
                .Where(d => d.TrangThai != TrangThaiDonHang.DaGiao && d.TrangThai != TrangThaiDonHang.DaHuy)
                .CountAsync();
                
            // Tổng số sản phẩm
            int tongSoSanPham = await _context.SanPhams.CountAsync();
            
            // Số sản phẩm còn hàng
            int soSanPhamConHang = await _context.SanPhams
                .Where(s => s.SoLuongTon > 0 && s.TrangThaiSanPham == TrangThaiSanPham.DangBan)
                .CountAsync();
                
            // Sản phẩm gần hết hàng (số lượng < 10)
            int soSanPhamGanHet = await _context.SanPhams
                .Where(s => s.SoLuongTon > 0 && s.SoLuongTon < 10 && s.TrangThaiSanPham == TrangThaiSanPham.DangBan)
                .CountAsync();
                
            // Sản phẩm hết hàng
            int soSanPhamHetHang = await _context.SanPhams
                .Where(s => s.SoLuongTon == 0 && s.TrangThaiSanPham == TrangThaiSanPham.DangBan)
                .CountAsync();
                
            // Doanh thu hôm nay
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            decimal doanhThuHomNay = await _context.DonHangs
                .Where(d => d.NgayDat >= today && d.NgayDat < tomorrow)
                .SumAsync(d => d.TongTien);
                
            // Doanh thu tuần này
            var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(7);
            decimal doanhThuTuanNay = await _context.DonHangs
                .Where(d => d.NgayDat >= startOfWeek && d.NgayDat < endOfWeek)
                .SumAsync(d => d.TongTien);
                
            // Doanh thu tháng này
            var startOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1);
            decimal doanhThuThangNay = await _context.DonHangs
                .Where(d => d.NgayDat >= startOfMonth && d.NgayDat < endOfMonth)
                .SumAsync(d => d.TongTien);
                
            // Kết quả
            var result = new TongQuanDto
            {
                TongDoanhThu = tongDoanhThu,
                TongSoDonHang = tongSoDonHang,
                SoDonHangDangXuLy = soDonHangDangXuLy,
                TongSoSanPham = tongSoSanPham,
                SoSanPhamConHang = soSanPhamConHang,
                SoSanPhamGanHet = soSanPhamGanHet,
                SoSanPhamHetHang = soSanPhamHetHang,
                DoanhThuHomNay = doanhThuHomNay,
                DoanhThuTuanNay = doanhThuTuanNay,
                DoanhThuThangNay = doanhThuThangNay
            };

            return Ok(result);
        }
    }
} 