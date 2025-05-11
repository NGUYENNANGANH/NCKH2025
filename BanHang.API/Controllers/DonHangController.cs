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
    [Authorize]
    public class DonHangController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DonHangController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/DonHang
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DonHangDto>>> GetUserOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var donHangs = await _context.DonHangs
                .Include(d => d.KhachHang)
                .Include(d => d.ChiTietDonHangs)
                .ThenInclude(c => c.SanPham)
                .Where(d => d.User_Id == userId)
                .OrderByDescending(d => d.NgayDat)
                .ToListAsync();

            var donHangDtos = donHangs.Select(d => new DonHangDto
            {
                Id_DonHang = d.Id_DonHang,
                User_Id = d.User_Id,
                TenKhachHang = d.KhachHang?.FullName ?? d.KhachHang?.UserName ?? "Unknown",
                NgayDat = d.NgayDat,
                TongTien = d.TongTien,
                TrangThai = d.TrangThai,
                DiaChiGiaoHang = d.DiaChiGiaoHang,
                SoDienThoai = d.SoDienThoai,
                GhiChu = d.GhiChu,
                PhuongThucThanhToan = d.PhuongThucThanhToan,
                PhiVanChuyen = d.PhiVanChuyen,
                ChiTietDonHangs = d.ChiTietDonHangs?.Select(c => new ChiTietDonHangDto
                {
                    Id_DonHang = c.Id_DonHang,
                    Id_SanPham = c.Id_SanPham,
                    TenSanPham = c.TenSanPham,
                    DonGia = c.DonGia,
                    SoLuong = c.SoLuong
                }).ToList() ?? new List<ChiTietDonHangDto>()
            }).ToList();

            return Ok(donHangDtos);
        }

        // GET: api/DonHang/all
        [HttpGet("all")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<IEnumerable<DonHangDto>>> GetAllOrders([FromQuery] bool? trangThai = null)
        {
            var query = _context.DonHangs
                .Include(d => d.KhachHang)
                .Include(d => d.ChiTietDonHangs)
                .ThenInclude(c => c.SanPham)
                .AsQueryable();

            if (trangThai.HasValue)
            {
                var status = trangThai.Value ? Models.TrangThaiDonHang.DaGiao : Models.TrangThaiDonHang.ChoXacNhan;
                query = query.Where(d => d.TrangThai == status);
            }

            var donHangs = await query
                .OrderByDescending(d => d.NgayDat)
                .ToListAsync();

            var donHangDtos = donHangs.Select(d => new DonHangDto
            {
                Id_DonHang = d.Id_DonHang,
                User_Id = d.User_Id,
                TenKhachHang = d.KhachHang?.FullName ?? d.KhachHang?.UserName ?? "Unknown",
                NgayDat = d.NgayDat,
                TongTien = d.TongTien,
                TrangThai = d.TrangThai,
                DiaChiGiaoHang = d.DiaChiGiaoHang,
                SoDienThoai = d.SoDienThoai,
                GhiChu = d.GhiChu,
                PhuongThucThanhToan = d.PhuongThucThanhToan,
                PhiVanChuyen = d.PhiVanChuyen,
                ChiTietDonHangs = d.ChiTietDonHangs?.Select(c => new ChiTietDonHangDto
                {
                    Id_DonHang = c.Id_DonHang,
                    Id_SanPham = c.Id_SanPham,
                    TenSanPham = c.TenSanPham,
                    DonGia = c.DonGia,
                    SoLuong = c.SoLuong
                }).ToList() ?? new List<ChiTietDonHangDto>()
            }).ToList();

            return Ok(donHangDtos);
        }

        // GET: api/DonHang/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DonHangDto>> GetDonHang(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var donHang = await _context.DonHangs
                .Include(d => d.KhachHang)
                .Include(d => d.ChiTietDonHangs)
                .ThenInclude(c => c.SanPham)
                .FirstOrDefaultAsync(d => d.Id_DonHang == id);

            if (donHang == null)
            {
                return NotFound();
            }

            // Check if the user is the owner or an admin/manager
            if (donHang.User_Id != userId && !User.IsInRole("Admin") && !User.IsInRole("Manager"))
            {
                return Forbid();
            }

            var donHangDto = new DonHangDto
            {
                Id_DonHang = donHang.Id_DonHang,
                User_Id = donHang.User_Id,
                TenKhachHang = donHang.KhachHang?.FullName ?? donHang.KhachHang?.UserName ?? "Unknown",
                NgayDat = donHang.NgayDat,
                TongTien = donHang.TongTien,
                TrangThai = donHang.TrangThai,
                DiaChiGiaoHang = donHang.DiaChiGiaoHang,
                SoDienThoai = donHang.SoDienThoai,
                GhiChu = donHang.GhiChu,
                PhuongThucThanhToan = donHang.PhuongThucThanhToan,
                PhiVanChuyen = donHang.PhiVanChuyen,
                ChiTietDonHangs = donHang.ChiTietDonHangs?.Select(c => new ChiTietDonHangDto
                {
                    Id_DonHang = c.Id_DonHang,
                    Id_SanPham = c.Id_SanPham,
                    TenSanPham = c.TenSanPham,
                    DonGia = c.DonGia,
                    SoLuong = c.SoLuong
                }).ToList() ?? new List<ChiTietDonHangDto>()
            };

            return Ok(donHangDto);
        }

        // POST: api/DonHang
        [HttpPost]
        public async Task<ActionResult<DonHangDto>> CreateDonHang([FromBody] CreateDonHangDto createDonHangDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Get user's cart
            var gioHang = await _context.GioHangs
                .Include(g => g.ChiTietGioHangs)
                .ThenInclude(c => c.SanPham)
                .ThenInclude(s => s.SanPham_KhuyenMais)
                .ThenInclude(sk => sk.KhuyenMai)
                .FirstOrDefaultAsync(g => g.Id_User == userId);

            if (gioHang == null || gioHang.ChiTietGioHangs == null || !gioHang.ChiTietGioHangs.Any())
            {
                return BadRequest("Giỏ hàng trống, không thể tạo đơn hàng");
            }

            // Calculate total price
            decimal tongTien = 0;
            foreach (var item in gioHang.ChiTietGioHangs)
            {
                if (item.SanPham == null)
                {
                    continue;
                }

                // Tính giá giảm dựa trên khuyến mãi đang áp dụng
                var giaBan = item.SanPham.GiaBan;
                var giaGiam = giaBan;

                // Tìm khuyến mãi có phần trăm giảm cao nhất và đang trong thời gian áp dụng
                if (item.SanPham.SanPham_KhuyenMais != null && item.SanPham.SanPham_KhuyenMais.Any())
                {
                    var now = DateTime.Now;
                    var maxDiscount = item.SanPham.SanPham_KhuyenMais
                        .Where(sk => sk.KhuyenMai != null &&
                                   now >= sk.KhuyenMai.NgayBatDau &&
                                   now <= sk.KhuyenMai.NgayKetThuc)
                        .OrderByDescending(sk => sk.KhuyenMai.PhanTramGiam)
                        .FirstOrDefault();

                    if (maxDiscount != null && maxDiscount.KhuyenMai != null)
                    {
                        var discountPercent = maxDiscount.KhuyenMai.PhanTramGiam / 100.0m;
                        giaGiam = giaBan * (1 - discountPercent);
                    }
                }

                tongTien += giaGiam * item.SoLuong;
            }

            // Create new order
            var donHang = new DonHang
            {
                User_Id = userId,
                NgayDat = DateTime.Now,
                TongTien = tongTien + createDonHangDto.PhiVanChuyen, // Add shipping fee to total
                TrangThai = TrangThaiDonHang.ChoXacNhan, // Initial status is pending
                DiaChiGiaoHang = createDonHangDto.DiaChiGiaoHang,
                SoDienThoai = createDonHangDto.SoDienThoai,
                GhiChu = createDonHangDto.GhiChu,
                PhuongThucThanhToan = createDonHangDto.PhuongThucThanhToan,
                PhiVanChuyen = createDonHangDto.PhiVanChuyen
            };

            _context.DonHangs.Add(donHang);
            await _context.SaveChangesAsync();

            // Create order details
            foreach (var item in gioHang.ChiTietGioHangs)
            {
                if (item.SanPham == null)
                {
                    continue;
                }

                // Tính giá giảm dựa trên khuyến mãi đang áp dụng
                var giaBan = item.SanPham.GiaBan;
                var giaGiam = giaBan;

                // Tìm khuyến mãi có phần trăm giảm cao nhất và đang trong thời gian áp dụng
                if (item.SanPham.SanPham_KhuyenMais != null && item.SanPham.SanPham_KhuyenMais.Any())
                {
                    var now = DateTime.Now;
                    var maxDiscount = item.SanPham.SanPham_KhuyenMais
                        .Where(sk => sk.KhuyenMai != null &&
                                   now >= sk.KhuyenMai.NgayBatDau &&
                                   now <= sk.KhuyenMai.NgayKetThuc)
                        .OrderByDescending(sk => sk.KhuyenMai.PhanTramGiam)
                        .FirstOrDefault();

                    if (maxDiscount != null && maxDiscount.KhuyenMai != null)
                    {
                        var discountPercent = maxDiscount.KhuyenMai.PhanTramGiam / 100.0m;
                        giaGiam = giaBan * (1 - discountPercent);
                    }
                }

                var chiTietDonHang = new ChiTietDonHang
                {
                    Id_DonHang = donHang.Id_DonHang,
                    Id_SanPham = item.Id_SanPham,
                    TenSanPham = item.SanPham.TenSanPham,
                    DonGia = giaGiam,
                    SoLuong = item.SoLuong
                };

                _context.ChiTietDonHangs.Add(chiTietDonHang);

                // Update product stock and sold quantity
                var sanPham = item.SanPham;
                sanPham.SoLuongTon -= item.SoLuong;
                sanPham.SoLuongDaBan += item.SoLuong;
                _context.Entry(sanPham).State = EntityState.Modified;
            }

            // Clear the cart
            _context.ChiTietGioHangs.RemoveRange(gioHang.ChiTietGioHangs);
            gioHang.NgayCapNhat = DateTime.Now;
            _context.Entry(gioHang).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            // Return the created order
            var donHangDto = new DonHangDto
            {
                Id_DonHang = donHang.Id_DonHang,
                User_Id = donHang.User_Id,
                NgayDat = donHang.NgayDat,
                TongTien = donHang.TongTien,
                TrangThai = donHang.TrangThai,
                DiaChiGiaoHang = donHang.DiaChiGiaoHang,
                SoDienThoai = donHang.SoDienThoai,
                GhiChu = donHang.GhiChu,
                PhuongThucThanhToan = donHang.PhuongThucThanhToan,
                PhiVanChuyen = donHang.PhiVanChuyen,
                ChiTietDonHangs = await _context.ChiTietDonHangs
                    .Where(c => c.Id_DonHang == donHang.Id_DonHang)
                    .Select(c => new ChiTietDonHangDto
                    {
                        Id_DonHang = c.Id_DonHang,
                        Id_SanPham = c.Id_SanPham,
                        TenSanPham = c.TenSanPham,
                        DonGia = c.DonGia,
                        SoLuong = c.SoLuong
                    }).ToListAsync()
            };

            return CreatedAtAction("GetDonHang", new { id = donHang.Id_DonHang }, donHangDto);
        }

        // PUT: api/DonHang/5/status
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateOrderStatus(string id, [FromBody] UpdateDonHangStatusDto updateStatusDto)
        {
            var donHang = await _context.DonHangs.FindAsync(id);
            if (donHang == null)
            {
                return NotFound();
            }

            donHang.TrangThai = updateStatusDto.TrangThai;
            _context.Entry(donHang).State = EntityState.Modified;

            // If order is completed, create invoice
            if (updateStatusDto.TrangThai == Models.TrangThaiDonHang.DaGiao)
            {
                var existingHoaDon = await _context.HoaDons.FirstOrDefaultAsync(h => h.Id_DonHang == id);
                
                if (existingHoaDon == null)
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var hoaDon = new HoaDon
                    {
                        CreateAt = DateTime.Now,
                        Id_User = userId,
                        Id_DonHang = id,
                        DateXuat = DateTime.Now
                    };
                    
                    _context.HoaDons.Add(hoaDon);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DonHangExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/DonHang/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDonHang(string id)
        {
            var donHang = await _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                .FirstOrDefaultAsync(d => d.Id_DonHang == id);
                
            if (donHang == null)
            {
                return NotFound();
            }

            // Check if order is already completed
            if (donHang.TrangThai == Models.TrangThaiDonHang.DaGiao)
            {
                return BadRequest("Không thể xóa đơn hàng đã hoàn thành");
            }

            // Get order details to restore product stock
            var chiTietDonHangs = donHang.ChiTietDonHangs;
            if (chiTietDonHangs != null)
            {
                foreach (var item in chiTietDonHangs)
                {
                    var sanPham = await _context.SanPhams.FindAsync(item.Id_SanPham);
                    if (sanPham != null)
                    {
                        // Restore stock and sold quantity
                        sanPham.SoLuongTon += item.SoLuong;
                        sanPham.SoLuongDaBan -= item.SoLuong;
                        _context.Entry(sanPham).State = EntityState.Modified;
                    }
                }
                
                // Remove order details
                _context.ChiTietDonHangs.RemoveRange(chiTietDonHangs);
            }

            // Remove order
            _context.DonHangs.Remove(donHang);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DonHangExists(string id)
        {
            return _context.DonHangs.Any(e => e.Id_DonHang == id);
        }
    }
} 