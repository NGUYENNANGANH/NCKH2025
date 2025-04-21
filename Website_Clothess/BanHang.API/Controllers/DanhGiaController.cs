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
                Date = d.Date
            }).ToList();

            return Ok(danhGiaDto);
        }

        // POST: api/DanhGia
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<DanhGiaDTO>> CreateDanhGia(CreateDanhGiaDTO createDanhGiaDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var sanPham = await _context.SanPhams.FindAsync(createDanhGiaDto.Id_SanPham);
            if (sanPham == null)
            {
                return NotFound("Sản phẩm không tồn tại");
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

            var user = await _context.Users.FindAsync(userId);

            var danhGiaDto = new DanhGiaDTO
            {
                Id_DanhGia = danhGia.Id_DanhGia,
                Comment = danhGia.Comment,
                Vote = danhGia.Vote,
                Username = user?.UserName ?? "Unknown",
                Id_SanPham = danhGia.Id_SanPham,
                TenSanPham = sanPham.TenSanPham,
                Date = danhGia.Date
            };

            return CreatedAtAction(nameof(GetDanhGiaBySanPham), new { id = danhGia.Id_SanPham }, danhGiaDto);
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

        private bool DanhGiaExists(string id)
        {
            return _context.DanhGias.Any(e => e.Id_DanhGia == id);
        }
    }
} 