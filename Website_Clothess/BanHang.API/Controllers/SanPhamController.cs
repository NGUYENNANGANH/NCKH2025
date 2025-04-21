using BanHang.API.Data;
using BanHang.API.DTOs;
using BanHang.API.Models;
using BanHang.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanHang.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SanPhamController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ISanPhamService _sanPhamService;

        public SanPhamController(ApplicationDbContext context, ISanPhamService sanPhamService)
        {
            _context = context;
            _sanPhamService = sanPhamService;
        }

        // GET: api/SanPham
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SanPhamDto>>> GetSanPhams()
        {
            var sanPhams = await _context.SanPhams
                .Include(s => s.DanhMuc)
                .ToListAsync();

            var sanPhamDtos = new List<SanPhamDto>();
            
            foreach (var sanPham in sanPhams)
            {
                var giaKhuyenMai = await _sanPhamService.TinhGiaKhuyenMai(sanPham.Id_SanPham, sanPham.GiaBan);
                var khuyenMais = await _sanPhamService.GetKhuyenMaiApDung(sanPham.Id_SanPham);
                
                sanPhamDtos.Add(new SanPhamDto
                {
                    Id_SanPham = sanPham.Id_SanPham,
                    TenSanPham = sanPham.TenSanPham,
                    MoTa = sanPham.MoTa,
                    GiaBan = sanPham.GiaBan,
                    GiaKhuyenMai = giaKhuyenMai,
                    SoLuongTon = sanPham.SoLuongTon,
                    HinhAnh = sanPham.HinhAnh,
                    HinhAnhPhu = sanPham.HinhAnhPhu,
                    SKU = sanPham.SKU,
                    TrangThaiSanPham = sanPham.TrangThaiSanPham,
                    NgayTao = sanPham.NgayTao,
                    NgayCapNhat = sanPham.NgayCapNhat,
                    Id_DanhMuc = sanPham.Id_DanhMuc,
                    TenDanhMuc = sanPham.DanhMuc?.TenDanhMuc ?? string.Empty,
                    SoLuongDaBan = sanPham.SoLuongDaBan,
                    KhuyenMais = khuyenMais
                });
            }

            return Ok(sanPhamDtos);
        }

        // GET: api/SanPham/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SanPhamDto>> GetSanPham(int id)
        {
            var sanPham = await _context.SanPhams
                .Include(s => s.DanhMuc)
                .FirstOrDefaultAsync(s => s.Id_SanPham == id);

            if (sanPham == null)
            {
                return NotFound();
            }

            var giaKhuyenMai = await _sanPhamService.TinhGiaKhuyenMai(sanPham.Id_SanPham, sanPham.GiaBan);
            var khuyenMais = await _sanPhamService.GetKhuyenMaiApDung(sanPham.Id_SanPham);
            
            var sanPhamDto = new SanPhamDto
            {
                Id_SanPham = sanPham.Id_SanPham,
                TenSanPham = sanPham.TenSanPham,
                MoTa = sanPham.MoTa,
                GiaBan = sanPham.GiaBan,
                GiaKhuyenMai = giaKhuyenMai,
                SoLuongTon = sanPham.SoLuongTon,
                HinhAnh = sanPham.HinhAnh,
                HinhAnhPhu = sanPham.HinhAnhPhu,
                SKU = sanPham.SKU,
                TrangThaiSanPham = sanPham.TrangThaiSanPham,
                NgayTao = sanPham.NgayTao,
                NgayCapNhat = sanPham.NgayCapNhat,
                Id_DanhMuc = sanPham.Id_DanhMuc,
                TenDanhMuc = sanPham.DanhMuc?.TenDanhMuc ?? string.Empty,
                SoLuongDaBan = sanPham.SoLuongDaBan,
                KhuyenMais = khuyenMais
            };

            return Ok(sanPhamDto);
        }

        // GET: api/SanPham/DanhMuc/5
        [HttpGet("DanhMuc/{danhMucId}")]
        public async Task<ActionResult<IEnumerable<SanPhamDto>>> GetSanPhamsByDanhMuc(int danhMucId)
        {
            var danhMuc = await _context.DanhMucs.FindAsync(danhMucId);
            if (danhMuc == null)
            {
                return NotFound("Danh mục không tồn tại");
            }

            var sanPhams = await _context.SanPhams
                .Where(s => s.Id_DanhMuc == danhMucId && s.TrangThaiSanPham == TrangThaiSanPham.DangBan)
                .Include(s => s.DanhMuc)
                .ToListAsync();

            var sanPhamDtos = new List<SanPhamDto>();
            
            foreach (var sanPham in sanPhams)
            {
                var giaKhuyenMai = await _sanPhamService.TinhGiaKhuyenMai(sanPham.Id_SanPham, sanPham.GiaBan);
                var khuyenMais = await _sanPhamService.GetKhuyenMaiApDung(sanPham.Id_SanPham);
                
                sanPhamDtos.Add(new SanPhamDto
                {
                    Id_SanPham = sanPham.Id_SanPham,
                    TenSanPham = sanPham.TenSanPham,
                    MoTa = sanPham.MoTa,
                    GiaBan = sanPham.GiaBan,
                    GiaKhuyenMai = giaKhuyenMai,
                    SoLuongTon = sanPham.SoLuongTon,
                    HinhAnh = sanPham.HinhAnh,
                    HinhAnhPhu = sanPham.HinhAnhPhu,
                    SKU = sanPham.SKU,
                    TrangThaiSanPham = sanPham.TrangThaiSanPham,
                    NgayTao = sanPham.NgayTao,
                    NgayCapNhat = sanPham.NgayCapNhat,
                    Id_DanhMuc = sanPham.Id_DanhMuc,
                    TenDanhMuc = sanPham.DanhMuc?.TenDanhMuc ?? string.Empty,
                    SoLuongDaBan = sanPham.SoLuongDaBan,
                    KhuyenMais = khuyenMais
                });
            }

            return Ok(sanPhamDtos);
        }

        // POST: api/SanPham
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<SanPhamDto>> CreateSanPham([FromBody] SanPhamCreateDto sanPhamDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var danhMuc = await _context.DanhMucs.FindAsync(sanPhamDto.Id_DanhMuc);
            if (danhMuc == null)
            {
                return BadRequest("Danh mục không tồn tại");
            }

            var sanPham = new SanPham
            {
                TenSanPham = sanPhamDto.TenSanPham,
                MoTa = sanPhamDto.MoTa,
                GiaBan = sanPhamDto.GiaBan,
                SoLuongTon = sanPhamDto.SoLuongTon,
                HinhAnh = sanPhamDto.HinhAnh,
                HinhAnhPhu = sanPhamDto.HinhAnhPhu,
                SKU = sanPhamDto.SKU,
                TrangThaiSanPham = sanPhamDto.TrangThaiSanPham,
                NgayTao = DateTime.Now,
                Id_DanhMuc = sanPhamDto.Id_DanhMuc,
                SoLuongDaBan = 0
            };

            _context.SanPhams.Add(sanPham);
            await _context.SaveChangesAsync();

            var giaKhuyenMai = await _sanPhamService.TinhGiaKhuyenMai(sanPham.Id_SanPham, sanPham.GiaBan);
            var khuyenMais = await _sanPhamService.GetKhuyenMaiApDung(sanPham.Id_SanPham);
            
            var result = new SanPhamDto
            {
                Id_SanPham = sanPham.Id_SanPham,
                TenSanPham = sanPham.TenSanPham,
                MoTa = sanPham.MoTa,
                GiaBan = sanPham.GiaBan,
                GiaKhuyenMai = giaKhuyenMai,
                SoLuongTon = sanPham.SoLuongTon,
                HinhAnh = sanPham.HinhAnh,
                HinhAnhPhu = sanPham.HinhAnhPhu,
                SKU = sanPham.SKU,
                TrangThaiSanPham = sanPham.TrangThaiSanPham,
                NgayTao = sanPham.NgayTao,
                NgayCapNhat = sanPham.NgayCapNhat,
                Id_DanhMuc = sanPham.Id_DanhMuc,
                TenDanhMuc = danhMuc.TenDanhMuc,
                SoLuongDaBan = sanPham.SoLuongDaBan,
                KhuyenMais = khuyenMais
            };

            return CreatedAtAction(nameof(GetSanPham), new { id = sanPham.Id_SanPham }, result);
        }

        // PUT: api/SanPham/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateSanPham(int id, [FromBody] SanPhamUpdateDto sanPhamDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null)
            {
                return NotFound();
            }

            var danhMuc = await _context.DanhMucs.FindAsync(sanPhamDto.Id_DanhMuc);
            if (danhMuc == null)
            {
                return BadRequest("Danh mục không tồn tại");
            }

            sanPham.TenSanPham = sanPhamDto.TenSanPham;
            sanPham.MoTa = sanPhamDto.MoTa;
            sanPham.GiaBan = sanPhamDto.GiaBan;
            sanPham.SoLuongTon = sanPhamDto.SoLuongTon;
            sanPham.HinhAnh = sanPhamDto.HinhAnh;
            sanPham.HinhAnhPhu = sanPhamDto.HinhAnhPhu;
            sanPham.SKU = sanPhamDto.SKU;
            sanPham.TrangThaiSanPham = sanPhamDto.TrangThaiSanPham;
            sanPham.NgayCapNhat = DateTime.Now;
            sanPham.Id_DanhMuc = sanPhamDto.Id_DanhMuc;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SanPhamExists(id))
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

        // DELETE: api/SanPham/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSanPham(int id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null)
            {
                return NotFound();
            }

            // Kiểm tra xem sản phẩm đã được bán hay chưa
            var hasSold = await _context.ChiTietDonHangs.AnyAsync(c => c.Id_SanPham == id);
            if (hasSold)
            {
                // Soft delete: Đổi trạng thái thành "NgungKinhDoanh" thay vì xóa khỏi database
                sanPham.TrangThaiSanPham = TrangThaiSanPham.NgungKinhDoanh;
                await _context.SaveChangesAsync();
                return NoContent();
            }

            // Nếu chưa có đơn hàng nào, có thể xóa cứng
            _context.SanPhams.Remove(sanPham);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }

        // GET: api/SanPham/search?keyword=abc
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<SanPhamDto>>> SearchSanPhams([FromQuery] string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return BadRequest("Từ khóa tìm kiếm không được để trống");
            }

            var sanPhams = await _context.SanPhams
                .Where(s => s.TrangThaiSanPham == TrangThaiSanPham.DangBan && (s.TenSanPham.Contains(keyword) || s.MoTa.Contains(keyword)))
                .Include(s => s.DanhMuc)
                .ToListAsync();

            var sanPhamDtos = new List<SanPhamDto>();
            
            foreach (var sanPham in sanPhams)
            {
                var giaKhuyenMai = await _sanPhamService.TinhGiaKhuyenMai(sanPham.Id_SanPham, sanPham.GiaBan);
                var khuyenMais = await _sanPhamService.GetKhuyenMaiApDung(sanPham.Id_SanPham);
                
                sanPhamDtos.Add(new SanPhamDto
                {
                    Id_SanPham = sanPham.Id_SanPham,
                    TenSanPham = sanPham.TenSanPham,
                    MoTa = sanPham.MoTa,
                    GiaBan = sanPham.GiaBan,
                    GiaKhuyenMai = giaKhuyenMai,
                    SoLuongTon = sanPham.SoLuongTon,
                    HinhAnh = sanPham.HinhAnh,
                    HinhAnhPhu = sanPham.HinhAnhPhu,
                    SKU = sanPham.SKU,
                    TrangThaiSanPham = sanPham.TrangThaiSanPham,
                    NgayTao = sanPham.NgayTao,
                    NgayCapNhat = sanPham.NgayCapNhat,
                    Id_DanhMuc = sanPham.Id_DanhMuc,
                    TenDanhMuc = sanPham.DanhMuc?.TenDanhMuc ?? string.Empty,
                    SoLuongDaBan = sanPham.SoLuongDaBan,
                    KhuyenMais = khuyenMais
                });
            }

            return Ok(sanPhamDtos);
        }

        private bool SanPhamExists(int id)
        {
            return _context.SanPhams.Any(e => e.Id_SanPham == id);
        }
    }
} 