using BanHang.API.Data;
using BanHang.API.DTOs;
using BanHang.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BanHang.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager")]
    public class HoaDonController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HoaDonController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/HoaDon
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HoaDonDto>>> GetHoaDons()
        {
            var hoaDons = await _context.HoaDons
                .Include(h => h.User)
                .Include(h => h.DonHang)
                .ThenInclude(d => d.ChiTietDonHangs)
                .OrderByDescending(h => h.CreateAt)
                .ToListAsync();

            var hoaDonDtos = hoaDons.Select(h => new HoaDonDto
            {
                Id_HoaDon = h.Id_HoaDon,
                CreateAt = h.CreateAt,
                Id_User = h.Id_User,
                TenNguoiLap = h.User?.FullName ?? h.User?.UserName ?? "Unknown",
                Id_DonHang = h.Id_DonHang,
                DateXuat = h.DateXuat,
                DonHang = h.DonHang != null ? new DonHangDto
                {
                    Id_DonHang = h.DonHang.Id_DonHang,
                    Id_KH = h.DonHang.Id_KH,
                    NgayDat = h.DonHang.NgayDat,
                    TongTien = h.DonHang.TongTien,
                    TrangThai = h.DonHang.TrangThai,
                    DiaChiGiaoHang = h.DonHang.DiaChiGiaoHang,
                    SoDienThoai = h.DonHang.SoDienThoai,
                    GhiChu = h.DonHang.GhiChu,
                    PhuongThucThanhToan = h.DonHang.PhuongThucThanhToan,
                    ChiTietDonHangs = h.DonHang.ChiTietDonHangs?.Select(c => new ChiTietDonHangDto
                    {
                        Id_DonHang = c.Id_DonHang,
                        Id_SanPham = c.Id_SanPham,
                        TenSanPham = c.TenSanPham,
                        DonGia = c.DonGia,
                        SoLuong = c.SoLuong
                    }).ToList() ?? new List<ChiTietDonHangDto>()
                } : null
            }).ToList();

            return Ok(hoaDonDtos);
        }

        // GET: api/HoaDon/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HoaDonDto>> GetHoaDon(int id)
        {
            var hoaDon = await _context.HoaDons
                .Include(h => h.User)
                .Include(h => h.DonHang)
                .ThenInclude(d => d.ChiTietDonHangs)
                .FirstOrDefaultAsync(h => h.Id_HoaDon == id);

            if (hoaDon == null)
            {
                return NotFound();
            }

            var hoaDonDto = new HoaDonDto
            {
                Id_HoaDon = hoaDon.Id_HoaDon,
                CreateAt = hoaDon.CreateAt,
                Id_User = hoaDon.Id_User,
                TenNguoiLap = hoaDon.User?.FullName ?? hoaDon.User?.UserName ?? "Unknown",
                Id_DonHang = hoaDon.Id_DonHang,
                DateXuat = hoaDon.DateXuat,
                DonHang = hoaDon.DonHang != null ? new DonHangDto
                {
                    Id_DonHang = hoaDon.DonHang.Id_DonHang,
                    Id_KH = hoaDon.DonHang.Id_KH,
                    NgayDat = hoaDon.DonHang.NgayDat,
                    TongTien = hoaDon.DonHang.TongTien,
                    TrangThai = hoaDon.DonHang.TrangThai,
                    DiaChiGiaoHang = hoaDon.DonHang.DiaChiGiaoHang,
                    SoDienThoai = hoaDon.DonHang.SoDienThoai,
                    GhiChu = hoaDon.DonHang.GhiChu,
                    PhuongThucThanhToan = hoaDon.DonHang.PhuongThucThanhToan,
                    ChiTietDonHangs = hoaDon.DonHang.ChiTietDonHangs?.Select(c => new ChiTietDonHangDto
                    {
                        Id_DonHang = c.Id_DonHang,
                        Id_SanPham = c.Id_SanPham,
                        TenSanPham = c.TenSanPham,
                        DonGia = c.DonGia,
                        SoLuong = c.SoLuong
                    }).ToList() ?? new List<ChiTietDonHangDto>()
                } : null
            };

            return Ok(hoaDonDto);
        }

        // GET: api/HoaDon/donhang/5
        [HttpGet("donhang/{donHangId}")]
        public async Task<ActionResult<HoaDonDto>> GetHoaDonByDonHangId(string donHangId)
        {
            var hoaDon = await _context.HoaDons
                .Include(h => h.User)
                .Include(h => h.DonHang)
                .ThenInclude(d => d.ChiTietDonHangs)
                .FirstOrDefaultAsync(h => h.Id_DonHang == donHangId);

            if (hoaDon == null)
            {
                return NotFound();
            }

            var hoaDonDto = new HoaDonDto
            {
                Id_HoaDon = hoaDon.Id_HoaDon,
                CreateAt = hoaDon.CreateAt,
                Id_User = hoaDon.Id_User,
                TenNguoiLap = hoaDon.User?.FullName ?? hoaDon.User?.UserName ?? "Unknown",
                Id_DonHang = hoaDon.Id_DonHang,
                DateXuat = hoaDon.DateXuat,
                DonHang = hoaDon.DonHang != null ? new DonHangDto
                {
                    Id_DonHang = hoaDon.DonHang.Id_DonHang,
                    Id_KH = hoaDon.DonHang.Id_KH,
                    NgayDat = hoaDon.DonHang.NgayDat,
                    TongTien = hoaDon.DonHang.TongTien,
                    TrangThai = hoaDon.DonHang.TrangThai,
                    DiaChiGiaoHang = hoaDon.DonHang.DiaChiGiaoHang,
                    SoDienThoai = hoaDon.DonHang.SoDienThoai,
                    GhiChu = hoaDon.DonHang.GhiChu,
                    PhuongThucThanhToan = hoaDon.DonHang.PhuongThucThanhToan,
                    ChiTietDonHangs = hoaDon.DonHang.ChiTietDonHangs?.Select(c => new ChiTietDonHangDto
                    {
                        Id_DonHang = c.Id_DonHang,
                        Id_SanPham = c.Id_SanPham,
                        TenSanPham = c.TenSanPham,
                        DonGia = c.DonGia,
                        SoLuong = c.SoLuong
                    }).ToList() ?? new List<ChiTietDonHangDto>()
                } : null
            };

            return Ok(hoaDonDto);
        }

        // POST: api/HoaDon
        [HttpPost("{donHangId}")]
        public async Task<ActionResult<HoaDonDto>> CreateHoaDon(string donHangId)
        {
            var donHang = await _context.DonHangs.FindAsync(donHangId);
            if (donHang == null)
            {
                return NotFound("Đơn hàng không tồn tại");
            }

            if (donHang.TrangThai != Models.TrangThaiDonHang.DaGiao)
            {
                return BadRequest("Đơn hàng chưa hoàn thành, không thể tạo hóa đơn");
            }

            var existingHoaDon = await _context.HoaDons.FirstOrDefaultAsync(h => h.Id_DonHang == donHangId);
            if (existingHoaDon != null)
            {
                return BadRequest("Hóa đơn cho đơn hàng này đã tồn tại");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var hoaDon = new HoaDon
            {
                CreateAt = DateTime.Now,
                Id_User = userId,
                Id_DonHang = donHangId,
                DateXuat = DateTime.Now
            };

            _context.HoaDons.Add(hoaDon);
            await _context.SaveChangesAsync();

            var createdHoaDon = await _context.HoaDons
                .Include(h => h.User)
                .Include(h => h.DonHang)
                .ThenInclude(d => d.ChiTietDonHangs)
                .FirstOrDefaultAsync(h => h.Id_HoaDon == hoaDon.Id_HoaDon);

            var hoaDonDto = new HoaDonDto
            {
                Id_HoaDon = createdHoaDon.Id_HoaDon,
                CreateAt = createdHoaDon.CreateAt,
                Id_User = createdHoaDon.Id_User,
                TenNguoiLap = createdHoaDon.User?.FullName ?? createdHoaDon.User?.UserName ?? "Unknown",
                Id_DonHang = createdHoaDon.Id_DonHang,
                DateXuat = createdHoaDon.DateXuat,
                DonHang = createdHoaDon.DonHang != null ? new DonHangDto
                {
                    Id_DonHang = createdHoaDon.DonHang.Id_DonHang,
                    Id_KH = createdHoaDon.DonHang.Id_KH,
                    NgayDat = createdHoaDon.DonHang.NgayDat,
                    TongTien = createdHoaDon.DonHang.TongTien,
                    TrangThai = createdHoaDon.DonHang.TrangThai,
                    DiaChiGiaoHang = createdHoaDon.DonHang.DiaChiGiaoHang,
                    SoDienThoai = createdHoaDon.DonHang.SoDienThoai,
                    GhiChu = createdHoaDon.DonHang.GhiChu,
                    PhuongThucThanhToan = createdHoaDon.DonHang.PhuongThucThanhToan,
                    ChiTietDonHangs = createdHoaDon.DonHang.ChiTietDonHangs?.Select(c => new ChiTietDonHangDto
                    {
                        Id_DonHang = c.Id_DonHang,
                        Id_SanPham = c.Id_SanPham,
                        TenSanPham = c.TenSanPham,
                        DonGia = c.DonGia,
                        SoLuong = c.SoLuong
                    }).ToList() ?? new List<ChiTietDonHangDto>()
                } : null
            };

            return CreatedAtAction("GetHoaDon", new { id = hoaDon.Id_HoaDon }, hoaDonDto);
        }

        // DELETE: api/HoaDon/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteHoaDon(int id)
        {
            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon == null)
            {
                return NotFound();
            }

            _context.HoaDons.Remove(hoaDon);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
} 