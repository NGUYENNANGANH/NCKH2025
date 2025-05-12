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
            try
            {
                // Không sử dụng Include để tránh lỗi khi truy vấn cột không tồn tại trong bảng DanhMuc
                var sanPhams = await _context.SanPhams.ToListAsync();
                var danhMucs = await _context.DanhMucs.ToListAsync();

                var sanPhamDtos = new List<SanPhamDto>();
                
                foreach (var sanPham in sanPhams)
                {
                    var danhMuc = danhMucs.FirstOrDefault(d => d.Id_DanhMuc == sanPham.Id_DanhMuc);
                    
                    sanPhamDtos.Add(new SanPhamDto
                    {
                        Id_SanPham = sanPham.Id_SanPham,
                        TenSanPham = sanPham.TenSanPham,
                        MoTa = sanPham.MoTa,
                        GiaBan = sanPham.GiaBan,
                        SoLuongTon = sanPham.SoLuongTon,
                        HinhAnh = sanPham.HinhAnh,
                        HinhAnhPhu = sanPham.HinhAnhPhu,
                        SKU = sanPham.SKU,
                        TrangThaiSanPham = sanPham.TrangThaiSanPham,
                        NgayTao = sanPham.NgayTao,
                        NgayCapNhat = sanPham.NgayCapNhat,
                        Id_DanhMuc = sanPham.Id_DanhMuc,
                        TenDanhMuc = danhMuc?.TenDanhMuc ?? string.Empty,
                        SoLuongDaBan = sanPham.SoLuongDaBan
                    });
                }

                return Ok(sanPhamDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        // GET: api/SanPham/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SanPhamDto>> GetSanPham(int id)
        {
            try
            {
                // Không sử dụng Include để tránh lỗi khi truy vấn cột không tồn tại trong bảng DanhMuc
                var sanPham = await _context.SanPhams.FindAsync(id);

                if (sanPham == null)
                {
                    return NotFound();
                }

                var danhMuc = await _context.DanhMucs.FindAsync(sanPham.Id_DanhMuc);

                var sanPhamDto = new SanPhamDto
                {
                    Id_SanPham = sanPham.Id_SanPham,
                    TenSanPham = sanPham.TenSanPham,
                    MoTa = sanPham.MoTa,
                    GiaBan = sanPham.GiaBan,
                    SoLuongTon = sanPham.SoLuongTon,
                    HinhAnh = sanPham.HinhAnh,
                    HinhAnhPhu = sanPham.HinhAnhPhu,
                    SKU = sanPham.SKU,
                    TrangThaiSanPham = sanPham.TrangThaiSanPham,
                    NgayTao = sanPham.NgayTao,
                    NgayCapNhat = sanPham.NgayCapNhat,
                    Id_DanhMuc = sanPham.Id_DanhMuc,
                    TenDanhMuc = danhMuc?.TenDanhMuc ?? string.Empty,
                    SoLuongDaBan = sanPham.SoLuongDaBan
                };

                return Ok(sanPhamDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        // GET: api/SanPham/DanhMuc/5
        [HttpGet("DanhMuc/{danhMucId}")]
        public async Task<ActionResult<IEnumerable<SanPhamDto>>> GetSanPhamsByDanhMuc(int danhMucId)
        {
            try
            {
                var danhMuc = await _context.DanhMucs.FindAsync(danhMucId);
                if (danhMuc == null)
                {
                    return NotFound("Danh mục không tồn tại");
                }

                // Sửa lỗi: Sử dụng TrangThaiSanPham_Value thay vì TrangThaiSanPham trong truy vấn SQL
                var sanPhams = await _context.SanPhams
                    .Where(s => s.Id_DanhMuc == danhMucId && s.TrangThaiSanPham_Value == (int)TrangThaiSanPham.DangBan)
                    .ToListAsync();

                var sanPhamDtos = new List<SanPhamDto>();
                
                foreach (var sanPham in sanPhams)
                {
                    sanPhamDtos.Add(new SanPhamDto
                    {
                        Id_SanPham = sanPham.Id_SanPham,
                        TenSanPham = sanPham.TenSanPham,
                        MoTa = sanPham.MoTa,
                        GiaBan = sanPham.GiaBan,
                        SoLuongTon = sanPham.SoLuongTon,
                        HinhAnh = sanPham.HinhAnh,
                        HinhAnhPhu = sanPham.HinhAnhPhu,
                        SKU = sanPham.SKU,
                        TrangThaiSanPham = sanPham.TrangThaiSanPham,
                        NgayTao = sanPham.NgayTao,
                        NgayCapNhat = sanPham.NgayCapNhat,
                        Id_DanhMuc = sanPham.Id_DanhMuc,
                        TenDanhMuc = danhMuc.TenDanhMuc,
                        SoLuongDaBan = sanPham.SoLuongDaBan
                    });
                }

                return Ok(sanPhamDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        // GET: api/SanPham/search
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<SanPhamDto>>> SearchSanPhams([FromQuery] string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return BadRequest("Từ khóa tìm kiếm không được để trống");
                }

                // Không sử dụng Include để tránh lỗi khi truy vấn cột không tồn tại trong bảng DanhMuc
                var sanPhams = await _context.SanPhams
                    .Where(s => s.TenSanPham.Contains(keyword) || 
                               (s.MoTa != null && s.MoTa.Contains(keyword)))
                    .ToListAsync();
                
                var danhMucIds = sanPhams.Select(s => s.Id_DanhMuc).Distinct().ToList();
                var danhMucs = await _context.DanhMucs
                    .Where(d => danhMucIds.Contains(d.Id_DanhMuc))
                    .ToListAsync();

                var sanPhamDtos = new List<SanPhamDto>();
                
                foreach (var sanPham in sanPhams)
                {
                    var danhMuc = danhMucs.FirstOrDefault(d => d.Id_DanhMuc == sanPham.Id_DanhMuc);
                    
                    sanPhamDtos.Add(new SanPhamDto
                    {
                        Id_SanPham = sanPham.Id_SanPham,
                        TenSanPham = sanPham.TenSanPham,
                        MoTa = sanPham.MoTa,
                        GiaBan = sanPham.GiaBan,
                        SoLuongTon = sanPham.SoLuongTon,
                        HinhAnh = sanPham.HinhAnh,
                        HinhAnhPhu = sanPham.HinhAnhPhu,
                        SKU = sanPham.SKU,
                        TrangThaiSanPham = sanPham.TrangThaiSanPham,
                        NgayTao = sanPham.NgayTao,
                        NgayCapNhat = sanPham.NgayCapNhat,
                        Id_DanhMuc = sanPham.Id_DanhMuc,
                        TenDanhMuc = danhMuc?.TenDanhMuc ?? string.Empty,
                        SoLuongDaBan = sanPham.SoLuongDaBan
                    });
                }

                return Ok(sanPhamDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        // POST: api/SanPham
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<object>> CreateSanPham([FromBody] SanPhamCreateDto sanPhamDto)
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

            // Trả về thông báo thành công kèm id của sản phẩm mới
            return CreatedAtAction(nameof(GetSanPham), new { id = sanPham.Id_SanPham }, new
            {
                message = "Sản phẩm đã được tạo thành công",
                id = sanPham.Id_SanPham
            });
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
                return NotFound($"Không tìm thấy sản phẩm với id = {id}");
            }

            var danhMuc = await _context.DanhMucs.FindAsync(sanPhamDto.Id_DanhMuc);
            if (danhMuc == null)
            {
                return BadRequest("Danh mục không tồn tại");
            }

            // Update product properties
            sanPham.TenSanPham = sanPhamDto.TenSanPham;
            sanPham.MoTa = sanPhamDto.MoTa;
            sanPham.GiaBan = sanPhamDto.GiaBan;
            sanPham.SoLuongTon = sanPhamDto.SoLuongTon;
            sanPham.HinhAnh = sanPhamDto.HinhAnh;
            sanPham.HinhAnhPhu = sanPhamDto.HinhAnhPhu;
            sanPham.SKU = sanPhamDto.SKU;
            sanPham.TrangThaiSanPham = sanPhamDto.TrangThaiSanPham;
            sanPham.Id_DanhMuc = sanPhamDto.Id_DanhMuc;
            sanPham.NgayCapNhat = DateTime.Now;

            _context.Entry(sanPham).State = EntityState.Modified;

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

            // Chỉ trả về thông báo thành công
            return Ok(new 
            { 
                message = "Sản phẩm đã được cập nhật thành công",
                id = id
            });
        }

        // DELETE: api/SanPham/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSanPham(int id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null)
            {
                return NotFound($"Không tìm thấy sản phẩm với id = {id}");
            }

            // Kiểm tra xem sản phẩm đã được bán hay chưa
            var hasSold = await _context.ChiTietDonHangs.AnyAsync(c => c.Id_SanPham == id);
            if (hasSold)
            {
                // Soft delete: Đổi trạng thái thành "NgungKinhDoanh" thay vì xóa khỏi database
                sanPham.TrangThaiSanPham = TrangThaiSanPham.NgungKinhDoanh;
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    message = "Sản phẩm đã được ngừng kinh doanh (do đã có đơn hàng)",
                    id = id
                });
            }

            // Nếu chưa có đơn hàng nào, có thể xóa cứng
            _context.SanPhams.Remove(sanPham);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Sản phẩm đã được xóa thành công",
                id = id
            });
        }

        private bool SanPhamExists(int id)
        {
            return _context.SanPhams.Any(e => e.Id_SanPham == id);
        }
    }
} 