using BanHang.API.Data;
using BanHang.API.DTOs;
using BanHang.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanHang.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DanhMucController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DanhMucController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/DanhMuc
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DanhMucDto>>> GetDanhMucs()
        {
            var danhMucs = await _context.DanhMucs
                .Select(d => new DanhMucDto
                {
                    Id_DanhMuc = d.Id_DanhMuc,
                    TenDanhMuc = d.TenDanhMuc,
                    MoTa = d.MoTa,
                    TrangThai = d.TrangThai,
                    SoLuongSanPham = _context.SanPhams.Count(s => s.Id_DanhMuc == d.Id_DanhMuc && s.TrangThaiSanPham == TrangThaiSanPham.DangBan)
                })
                .ToListAsync();

            return Ok(danhMucs);
        }

        // GET: api/DanhMuc/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DanhMucDto>> GetDanhMuc(int id)
        {
            var danhMuc = await _context.DanhMucs.FindAsync(id);

            if (danhMuc == null)
            {
                return NotFound();
            }

            var danhMucDto = new DanhMucDto
            {
                Id_DanhMuc = danhMuc.Id_DanhMuc,
                TenDanhMuc = danhMuc.TenDanhMuc,
                MoTa = danhMuc.MoTa,
                TrangThai = danhMuc.TrangThai,
                SoLuongSanPham = await _context.SanPhams.CountAsync(s => s.Id_DanhMuc == id && s.TrangThaiSanPham == TrangThaiSanPham.DangBan)
            };

            return Ok(danhMucDto);
        }

        // POST: api/DanhMuc
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<DanhMucDto>> CreateDanhMuc([FromBody] DanhMucCreateDto danhMucDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if category with same name exists
            var existingDanhMuc = await _context.DanhMucs
                .FirstOrDefaultAsync(d => d.TenDanhMuc.ToLower() == danhMucDto.TenDanhMuc.ToLower());
                
            if (existingDanhMuc != null)
            {
                return BadRequest("Danh mục với tên này đã tồn tại");
            }

            var danhMuc = new DanhMuc
            {
                TenDanhMuc = danhMucDto.TenDanhMuc,
                MoTa = danhMucDto.MoTa,
                TrangThai = danhMucDto.TrangThai
            };

            _context.DanhMucs.Add(danhMuc);
            await _context.SaveChangesAsync();

            var result = new DanhMucDto
            {
                Id_DanhMuc = danhMuc.Id_DanhMuc,
                TenDanhMuc = danhMuc.TenDanhMuc,
                MoTa = danhMuc.MoTa,
                TrangThai = danhMuc.TrangThai,
                SoLuongSanPham = 0
            };

            return CreatedAtAction("GetDanhMuc", new { id = danhMuc.Id_DanhMuc }, result);
        }

        // PUT: api/DanhMuc/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateDanhMuc(int id, [FromBody] DanhMucUpdateDto danhMucDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var danhMuc = await _context.DanhMucs.FindAsync(id);
            if (danhMuc == null)
            {
                return NotFound();
            }

            // Check if category with same name exists (excluding current category)
            var existingDanhMuc = await _context.DanhMucs
                .FirstOrDefaultAsync(d => d.TenDanhMuc.ToLower() == danhMucDto.TenDanhMuc.ToLower() 
                                     && d.Id_DanhMuc != id);
                
            if (existingDanhMuc != null)
            {
                return BadRequest("Danh mục với tên này đã tồn tại");
            }

            danhMuc.TenDanhMuc = danhMucDto.TenDanhMuc;
            danhMuc.MoTa = danhMucDto.MoTa;
            danhMuc.TrangThai = danhMucDto.TrangThai;

            _context.Entry(danhMuc).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DanhMucExists(id))
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

        // DELETE: api/DanhMuc/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDanhMuc(int id)
        {
            var danhMuc = await _context.DanhMucs.FindAsync(id);
            if (danhMuc == null)
            {
                return NotFound();
            }

            // Check if category is used in any product
            var hasProducts = await _context.SanPhams.AnyAsync(s => s.Id_DanhMuc == id);
            if (hasProducts)
            {
                return BadRequest("Không thể xóa danh mục đang có sản phẩm");
            }

            _context.DanhMucs.Remove(danhMuc);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DanhMucExists(int id)
        {
            return _context.DanhMucs.Any(e => e.Id_DanhMuc == id);
        }
    }
} 