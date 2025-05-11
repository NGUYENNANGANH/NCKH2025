using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BanHang.API.Data;
using BanHang.API.DTOs;
using BanHang.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanHang.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DanhGiaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DanhGiaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/DanhGia/SanPham/{id}
        [HttpGet("SanPham/{id}")]
        public async Task<ActionResult<IEnumerable<DanhGiaDTO>>> GetDanhGiaBySanPham(int id)
        {
            var danhGiaList = await _context.DanhGias
                .Include(d => d.User)
                .Include(d => d.SanPham)
                .Where(d => d.Id_SanPham == id)
                .OrderByDescending(d => d.Date)
                .ToListAsync();

            if (!danhGiaList.Any())
            {
                return Ok(new List<DanhGiaDTO>());
            }

            var danhGiaDto = danhGiaList.Select(d => new DanhGiaDTO
            {
                Id_DanhGia = d.Id_DanhGia,
                Comment = d.Comment,
                Vote = d.Vote,
                Username = d.User?.UserName ?? "Unknown",
                Id_SanPham = d.Id_SanPham,
                TenSanPham = d.SanPham?.TenSanPham ?? "Unknown",
                HinhAnh = d.SanPham?.HinhAnh,
                Date = d.Date
            }).ToList();

            return Ok(danhGiaDto);
        }

        // GET: api/DanhGia/User
        [HttpGet("User")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<DanhGiaDTO>>> GetDanhGiaByUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var danhGiaList = await _context.DanhGias
                .Include(d => d.User)
                .Include(d => d.SanPham)
                .Where(d => d.Id_User == userId)
                .OrderByDescending(d => d.Date)
                .ToListAsync();

            var danhGiaDto = danhGiaList.Select(d => new DanhGiaDTO
            {
                Id_DanhGia = d.Id_DanhGia,
                Comment = d.Comment,
                Vote = d.Vote,
                Username = d.User?.UserName ?? "Unknown",
                Id_SanPham = d.Id_SanPham,
                TenSanPham = d.SanPham?.TenSanPham ?? "Unknown",
                HinhAnh = d.SanPham?.HinhAnh,
                Date = d.Date
            }).ToList();

            return Ok(danhGiaDto);
        }

        // POST: api/DanhGia
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateDanhGia(CreateDanhGiaDTO createDanhGiaDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            Console.WriteLine($"Creating review for user ID: {userId}, product ID: {createDanhGiaDto.Id_SanPham}");

            var sanPham = await _context.SanPhams.FindAsync(createDanhGiaDto.Id_SanPham);
            if (sanPham == null)
            {
                return NotFound("Sản phẩm không tồn tại");
            }

            // Kiểm tra các đơn hàng của người dùng - chỉ lấy ID
            var donHangIds = await _context.DonHangs
                .Where(d => d.User_Id == userId && d.TrangThai == TrangThaiDonHang.DaGiao)
                .Select(d => d.Id_DonHang)
                .ToListAsync();

            Console.WriteLine($"Found {donHangIds.Count} completed orders for user {userId}");
            
            // Kiểm tra xem sản phẩm có trong đơn hàng của người dùng hay không
            var chiTietDonHang = await _context.ChiTietDonHangs
                .Where(c => donHangIds.Contains(c.Id_DonHang) && c.Id_SanPham == createDanhGiaDto.Id_SanPham)
                .ToListAsync();
                
            Console.WriteLine($"Found {chiTietDonHang.Count} order details containing product {createDanhGiaDto.Id_SanPham}");
            
            var hasBought = chiTietDonHang.Any();

            if (!hasBought)
            {
                return BadRequest("Bạn chỉ có thể đánh giá sản phẩm đã mua và đã giao hàng");
            }

            // Check if user already reviewed this product
            var existingReview = await _context.DanhGias
                .Where(d => d.Id_User == userId && d.Id_SanPham == createDanhGiaDto.Id_SanPham)
                .FirstOrDefaultAsync();

            if (existingReview != null)
            {
                return BadRequest("Bạn đã đánh giá sản phẩm này rồi");
            }

            var danhGia = new DanhGia
            {
                Id_DanhGia = Guid.NewGuid().ToString(),
                Comment = createDanhGiaDto.Comment,
                Vote = createDanhGiaDto.Vote,
                Id_User = userId,
                Id_SanPham = createDanhGiaDto.Id_SanPham,
                Date = DateTime.Now
            };

            _context.DanhGias.Add(danhGia);
            await _context.SaveChangesAsync();

            // Trả về thông báo thành công thay vì toàn bộ thông tin đánh giá
            return Ok(new { message = "Đã đánh giá sản phẩm thành công", success = true });
        }

        // PUT: api/DanhGia/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateDanhGia(string id, UpdateDanhGiaDTO updateDanhGiaDto)
        {
            var danhGia = await _context.DanhGias.FindAsync(id);
            if (danhGia == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || danhGia.Id_User != userId)
            {
                return Forbid();
            }

            danhGia.Comment = updateDanhGiaDto.Comment;
            danhGia.Vote = updateDanhGiaDto.Vote;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DanhGiaExists(id))
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

        // DELETE: api/DanhGia/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteDanhGia(string id)
        {
            var danhGia = await _context.DanhGias.FindAsync(id);
            if (danhGia == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || danhGia.Id_User != userId)
            {
                return Forbid();
            }

            _context.DanhGias.Remove(danhGia);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/DanhGia/SanPhamChuaDanhGia
        [HttpGet("SanPhamChuaDanhGia")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<SanPhamChuaDanhGiaDTO>>> GetSanPhamChuaDanhGia()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                Console.WriteLine($"Getting products not yet reviewed for user {userId}");
                
                // Sử dụng LINQ thay vì SQL thuần - chỉ lấy ID đơn hàng
                var donHangsDaGiao = await _context.DonHangs
                    .Where(d => d.User_Id == userId && d.TrangThai == TrangThaiDonHang.DaGiao)
                    .Select(d => new { d.Id_DonHang, d.NgayDat })
                    .ToListAsync();
                
                Console.WriteLine($"Found {donHangsDaGiao.Count} completed orders");
                
                if (!donHangsDaGiao.Any())
                {
                    return Ok(new List<SanPhamChuaDanhGiaDTO>());
                }
                
                var donHangIds = donHangsDaGiao.Select(d => d.Id_DonHang).ToList();
                
                // Lấy thông tin sản phẩm đã mua
                var sanPhamDaMua = await _context.ChiTietDonHangs
                    .Where(c => donHangIds.Contains(c.Id_DonHang))
                    .Join(_context.SanPhams,
                        c => c.Id_SanPham,
                        s => s.Id_SanPham,
                        (c, s) => new 
                        {
                            Id_SanPham = s.Id_SanPham,
                            TenSanPham = s.TenSanPham,
                            HinhAnh = s.HinhAnh,
                            DonGia = c.DonGia,
                            SoLuong = c.SoLuong,
                            Id_DonHang = c.Id_DonHang
                        })
                    .ToListAsync();

                Console.WriteLine($"Found {sanPhamDaMua.Count} purchased products");
                
                if (!sanPhamDaMua.Any())
                {
                    return Ok(new List<SanPhamChuaDanhGiaDTO>());
                }
                
                // Map thông tin đơn hàng với sản phẩm
                var sanPhamDaMuaDto = sanPhamDaMua
                    .Join(donHangsDaGiao,
                        s => s.Id_DonHang,
                        d => d.Id_DonHang,
                        (s, d) => new SanPhamDaMuaDTO
                        {
                            Id_SanPham = s.Id_SanPham,
                            TenSanPham = s.TenSanPham,
                            HinhAnh = s.HinhAnh,
                            DonGia = s.DonGia,
                            SoLuong = s.SoLuong,
                            Id_DonHang = s.Id_DonHang,
                            NgayDat = d.NgayDat
                        })
                    .ToList();

                // Lấy tất cả sản phẩm đã đánh giá của người dùng
                var sanPhamDaDanhGia = await _context.DanhGias
                    .Where(d => d.Id_User == userId)
                    .Select(d => d.Id_SanPham)
                    .ToListAsync();

                Console.WriteLine($"Found {sanPhamDaDanhGia.Count} already reviewed products");

                // Lọc ra các sản phẩm chưa đánh giá
                var sanPhamChuaDanhGia = sanPhamDaMuaDto
                    .Where(s => !sanPhamDaDanhGia.Contains(s.Id_SanPham))
                    .GroupBy(s => s.Id_SanPham) // Nhóm theo Id_SanPham để loại bỏ trùng lặp
                    .Select(g => g.OrderByDescending(x => x.NgayDat).First()) // Lấy đơn hàng mới nhất
                    .Select(s => new SanPhamChuaDanhGiaDTO
                    {
                        Id_SanPham = s.Id_SanPham,
                        TenSanPham = s.TenSanPham,
                        HinhAnh = s.HinhAnh,
                        GiaBan = s.DonGia,
                        SoLuong = s.SoLuong,
                        Id_DonHang = s.Id_DonHang,
                        NgayMua = s.NgayDat
                    })
                    .ToList();

                Console.WriteLine($"Found {sanPhamChuaDanhGia.Count} products not yet reviewed");
                
                return Ok(sanPhamChuaDanhGia);
            }
            catch (Exception ex)
            {
                // Log lỗi
                Console.WriteLine($"Error in GetSanPhamChuaDanhGia: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/DanhGia/SanPhamDaDanhGia
        [HttpGet("SanPhamDaDanhGia")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<SanPhamDaDanhGiaDTO>>> GetSanPhamDaDanhGia()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            Console.WriteLine($"Getting reviewed products for user {userId}");

            // Lấy tất cả sản phẩm đã đánh giá của người dùng
            var danhGiaList = await _context.DanhGias
                .Include(d => d.SanPham)
                .Include(d => d.User)
                .Where(d => d.Id_User == userId)
                .OrderByDescending(d => d.Date)
                .ToListAsync();

            Console.WriteLine($"Found {danhGiaList.Count} reviews");

            if (!danhGiaList.Any())
            {
                Console.WriteLine("No reviews found for this user");
                return Ok(new List<SanPhamDaDanhGiaDTO>());
            }

            var sanPhamDaDanhGia = danhGiaList.Select(d => new SanPhamDaDanhGiaDTO
            {
                Id_DanhGia = d.Id_DanhGia,
                Id_SanPham = d.Id_SanPham,
                TenSanPham = d.SanPham?.TenSanPham ?? "Unknown",
                HinhAnh = d.SanPham?.HinhAnh,
                Comment = d.Comment,
                Vote = d.Vote,
                NgayDanhGia = d.Date
            }).ToList();

            foreach (var item in sanPhamDaDanhGia)
            {
                Console.WriteLine($"Reviewed product: ID={item.Id_SanPham}, Name={item.TenSanPham}, Rating={item.Vote}, Date={item.NgayDanhGia}");
            }

            return Ok(sanPhamDaDanhGia);
        }

        private bool DanhGiaExists(string id)
        {
            return _context.DanhGias.Any(e => e.Id_DanhGia == id);
        }
    }
} 