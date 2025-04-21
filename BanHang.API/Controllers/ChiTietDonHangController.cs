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
    public class ChiTietDonHangController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ChiTietDonHangController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ChiTietDonHang/by-product/5
        [HttpGet("by-product/{productId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<IEnumerable<ChiTietDonHangDto>>> GetChiTietDonHangBySanPham(int productId)
        {
            var chiTietDonHangs = await _context.ChiTietDonHangs
                .Include(c => c.DonHang)
                .Include(c => c.SanPham)
                .Where(c => c.Id_SanPham == productId)
                .OrderByDescending(c => c.DonHang.NgayDat)
                .ToListAsync();

            var chiTietDtos = chiTietDonHangs.Select(c => new ChiTietDonHangDto
            {
                Id_DonHang = c.Id_DonHang,
                Id_SanPham = c.Id_SanPham,
                TenSanPham = c.TenSanPham,
                DonGia = c.DonGia,
                SoLuong = c.SoLuong
            }).ToList();

            return Ok(chiTietDtos);
        }

        // GET: api/ChiTietDonHang/by-order/ORDER123
        [HttpGet("by-order/{orderId}")]
        public async Task<ActionResult<IEnumerable<ChiTietDonHangDto>>> GetChiTietDonHangByDonHang(string orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Kiểm tra xem đơn hàng có thuộc về người dùng hiện tại hoặc là admin/manager không
            var donHang = await _context.DonHangs.FindAsync(orderId);
            if (donHang == null)
            {
                return NotFound();
            }

            if (donHang.Id_KH != userId && !User.IsInRole("Admin") && !User.IsInRole("Manager"))
            {
                return Forbid();
            }

            var chiTietDonHangs = await _context.ChiTietDonHangs
                .Include(c => c.SanPham)
                .Where(c => c.Id_DonHang == orderId)
                .ToListAsync();

            var chiTietDtos = chiTietDonHangs.Select(c => new ChiTietDonHangDto
            {
                Id_DonHang = c.Id_DonHang,
                Id_SanPham = c.Id_SanPham,
                TenSanPham = c.TenSanPham,
                DonGia = c.DonGia,
                SoLuong = c.SoLuong
            }).ToList();

            return Ok(chiTietDtos);
        }

        // POST: api/ChiTietDonHang
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ChiTietDonHangDto>> CreateChiTietDonHang([FromBody] ChiTietDonHangDto chiTietDto)
        {
            if (string.IsNullOrEmpty(chiTietDto.Id_DonHang))
            {
                return BadRequest("Id đơn hàng không được để trống");
            }

            // Kiểm tra tồn tại của đơn hàng
            var donHang = await _context.DonHangs.FindAsync(chiTietDto.Id_DonHang);
            if (donHang == null)
            {
                return BadRequest("Đơn hàng không tồn tại");
            }

            // Kiểm tra tồn tại của sản phẩm
            var sanPham = await _context.SanPhams
                .Include(s => s.SanPham_KhuyenMais)
                .ThenInclude(sk => sk.KhuyenMai)
                .FirstOrDefaultAsync(s => s.Id_SanPham == chiTietDto.Id_SanPham);
                
            if (sanPham == null)
            {
                return BadRequest("Sản phẩm không tồn tại");
            }

            // Kiểm tra xem chi tiết đơn hàng đã tồn tại chưa
            var existingChiTiet = await _context.ChiTietDonHangs
                .FirstOrDefaultAsync(c => c.Id_DonHang == chiTietDto.Id_DonHang && c.Id_SanPham == chiTietDto.Id_SanPham);

            if (existingChiTiet != null)
            {
                return BadRequest("Chi tiết đơn hàng đã tồn tại, hãy sử dụng phương thức PUT để cập nhật");
            }

            // Tính giá giảm dựa trên khuyến mãi đang áp dụng
            var giaBan = sanPham.GiaBan;
            var giaGiam = giaBan;

            // Tìm khuyến mãi có phần trăm giảm cao nhất và đang trong thời gian áp dụng
            if (sanPham.SanPham_KhuyenMais != null && sanPham.SanPham_KhuyenMais.Any())
            {
                var now = DateTime.Now;
                var maxDiscount = sanPham.SanPham_KhuyenMais
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

            // Tạo chi tiết đơn hàng mới
            var chiTietDonHang = new ChiTietDonHang
            {
                Id_DonHang = chiTietDto.Id_DonHang,
                Id_SanPham = chiTietDto.Id_SanPham,
                TenSanPham = sanPham.TenSanPham,
                DonGia = giaGiam,
                SoLuong = chiTietDto.SoLuong
            };

            _context.ChiTietDonHangs.Add(chiTietDonHang);

            // Cập nhật tổng tiền đơn hàng
            await UpdateOrderTotal(chiTietDto.Id_DonHang);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ChiTietDonHangExists(chiTietDto.Id_DonHang, chiTietDto.Id_SanPham))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            // Trả về DTO với thông tin đã tạo
            var returnDto = new ChiTietDonHangDto
            {
                Id_DonHang = chiTietDonHang.Id_DonHang,
                Id_SanPham = chiTietDonHang.Id_SanPham,
                TenSanPham = chiTietDonHang.TenSanPham,
                DonGia = chiTietDonHang.DonGia,
                SoLuong = chiTietDonHang.SoLuong
            };

            return CreatedAtAction(nameof(GetChiTietDonHangByDonHang), new { orderId = chiTietDto.Id_DonHang }, returnDto);
        }

        // PUT: api/ChiTietDonHang/ORDER123/5
        [HttpPut("{orderId}/{productId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateChiTietDonHang(string orderId, int productId, [FromBody] ChiTietDonHangDto chiTietDto)
        {
            if (orderId != chiTietDto.Id_DonHang || productId != chiTietDto.Id_SanPham)
            {
                return BadRequest("ID trong đường dẫn và trong body không khớp");
            }

            var chiTietDonHang = await _context.ChiTietDonHangs.FindAsync(orderId, productId);
            if (chiTietDonHang == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin chi tiết đơn hàng
            chiTietDonHang.TenSanPham = chiTietDto.TenSanPham;
            chiTietDonHang.DonGia = chiTietDto.DonGia;
            chiTietDonHang.SoLuong = chiTietDto.SoLuong;

            try
            {
                await _context.SaveChangesAsync();
                
                // Cập nhật tổng tiền đơn hàng
                await UpdateOrderTotal(orderId);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChiTietDonHangExists(orderId, productId))
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

        // DELETE: api/ChiTietDonHang/ORDER123/5
        [HttpDelete("{orderId}/{productId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteChiTietDonHang(string orderId, int productId)
        {
            var chiTietDonHang = await _context.ChiTietDonHangs.FindAsync(orderId, productId);
            if (chiTietDonHang == null)
            {
                return NotFound();
            }

            _context.ChiTietDonHangs.Remove(chiTietDonHang);
            await _context.SaveChangesAsync();

            // Cập nhật tổng tiền đơn hàng
            await UpdateOrderTotal(orderId);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Phương thức để cập nhật tổng tiền đơn hàng
        private async Task UpdateOrderTotal(string orderId)
        {
            var donHang = await _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                .FirstOrDefaultAsync(d => d.Id_DonHang == orderId);

            if (donHang != null && donHang.ChiTietDonHangs != null)
            {
                donHang.TongTien = donHang.ChiTietDonHangs.Sum(c => c.DonGia * c.SoLuong);
                _context.Entry(donHang).State = EntityState.Modified;
            }
        }

        private bool ChiTietDonHangExists(string orderId, int productId)
        {
            return _context.ChiTietDonHangs.Any(e => e.Id_DonHang == orderId && e.Id_SanPham == productId);
        }
    }
} 