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
    public class GioHangController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GioHangController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/GioHang
        [HttpGet]
        public async Task<ActionResult<GioHangDto>> GetCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var gioHang = await _context.GioHangs
                .Include(g => g.ChiTietGioHangs)
                .ThenInclude(c => c.SanPham)
                .ThenInclude(s => s.SanPham_KhuyenMais)
                .ThenInclude(sk => sk.KhuyenMai)
                .FirstOrDefaultAsync(g => g.Id_User == userId);

            if (gioHang == null)
            {
                // Create new cart if not exists
                gioHang = new GioHang
                {
                    Id_User = userId,
                    NgayTao = DateTime.Now
                };
                _context.GioHangs.Add(gioHang);
                await _context.SaveChangesAsync();

                return new GioHangDto
                {
                    Id_GioHang = gioHang.Id_GioHang,
                    Id_User = gioHang.Id_User,
                    NgayTao = gioHang.NgayTao,
                    NgayCapNhat = gioHang.NgayCapNhat,
                    ChiTietGioHangs = new List<ChiTietGioHangDto>()
                };
            }

            var gioHangDto = new GioHangDto
            {
                Id_GioHang = gioHang.Id_GioHang,
                Id_User = gioHang.Id_User,
                NgayTao = gioHang.NgayTao,
                NgayCapNhat = gioHang.NgayCapNhat,
                ChiTietGioHangs = gioHang.ChiTietGioHangs?
                    .Select(c => {
                        // Tính giá khuyến mãi dựa trên KhuyenMai đang áp dụng
                        var giaBan = c.SanPham.GiaBan;
                        decimal? giaKhuyenMai = null;

                        // Tìm khuyến mãi có phần trăm giảm cao nhất và đang trong thời gian áp dụng
                        if (c.SanPham.SanPham_KhuyenMais != null && c.SanPham.SanPham_KhuyenMais.Any())
                        {
                            var now = DateTime.Now;
                            var maxDiscount = c.SanPham.SanPham_KhuyenMais
                                .Where(sk => sk.KhuyenMai != null &&
                                           now >= sk.KhuyenMai.NgayBatDau &&
                                           now <= sk.KhuyenMai.NgayKetThuc)
                                .OrderByDescending(sk => sk.KhuyenMai.PhanTramGiam)
                                .FirstOrDefault();

                            if (maxDiscount != null && maxDiscount.KhuyenMai != null)
                            {
                                var discountPercent = maxDiscount.KhuyenMai.PhanTramGiam / 100.0m;
                                giaKhuyenMai = giaBan * (1 - discountPercent);
                            }
                        }

                        return new ChiTietGioHangDto
                        {
                            Id_GioHang = c.Id_GioHang,
                            Id_SanPham = c.Id_SanPham,
                            TenSanPham = c.SanPham.TenSanPham,
                            GiaBan = giaBan,
                            GiaKhuyenMai = giaKhuyenMai,
                            HinhAnh = c.SanPham.HinhAnh,
                            SoLuong = c.SoLuong,
                            KichCo = c.KichCo,
                            MauSac = c.MauSac
                        };
                    }).ToList() ?? new List<ChiTietGioHangDto>()
            };

            return Ok(gioHangDto);
        }

        // POST: api/GioHang/add
        [HttpPost("add")]
        public async Task<ActionResult<GioHangDto>> AddToCart([FromBody] AddToCartDto addToCartDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (addToCartDto.SoLuong <= 0)
            {
                return BadRequest("Số lượng sản phẩm phải lớn hơn 0");
            }

            var sanPham = await _context.SanPhams.FindAsync(addToCartDto.Id_SanPham);
            if (sanPham == null)
            {
                return NotFound("Sản phẩm không tồn tại");
            }

            if (sanPham.TrangThaiSanPham != TrangThaiSanPham.DangBan)
            {
                return BadRequest("Sản phẩm đã ngừng kinh doanh");
            }

            if (sanPham.SoLuongTon < addToCartDto.SoLuong)
            {
                return BadRequest("Số lượng sản phẩm trong kho không đủ");
            }

            var gioHang = await _context.GioHangs
                .Include(g => g.ChiTietGioHangs)
                .FirstOrDefaultAsync(g => g.Id_User == userId);

            if (gioHang == null)
            {
                // Create new cart if not exists
                gioHang = new GioHang
                {
                    Id_User = userId,
                    NgayTao = DateTime.Now
                };
                _context.GioHangs.Add(gioHang);
                await _context.SaveChangesAsync();
            }

            // Check if product already in cart with same size and color
            var cartItem = await _context.ChiTietGioHangs
                .FirstOrDefaultAsync(c => c.Id_GioHang == gioHang.Id_GioHang && 
                                     c.Id_SanPham == addToCartDto.Id_SanPham &&
                                     c.KichCo == addToCartDto.KichCo &&
                                     c.MauSac == addToCartDto.MauSac);

            if (cartItem != null)
            {
                // Update quantity if already exists
                cartItem.SoLuong += addToCartDto.SoLuong;
                
                // Check if quantity exceeds stock
                if (cartItem.SoLuong > sanPham.SoLuongTon)
                {
                    return BadRequest("Số lượng sản phẩm trong kho không đủ");
                }
                
                _context.Entry(cartItem).State = EntityState.Modified;
            }
            else
            {
                // Add new item to cart
                var chiTietGioHang = new ChiTietGioHang
                {
                    Id_GioHang = gioHang.Id_GioHang,
                    Id_SanPham = addToCartDto.Id_SanPham,
                    SoLuong = addToCartDto.SoLuong,
                    KichCo = addToCartDto.KichCo,
                    MauSac = addToCartDto.MauSac
                };
                _context.ChiTietGioHangs.Add(chiTietGioHang);
            }

            gioHang.NgayCapNhat = DateTime.Now;
            _context.Entry(gioHang).State = EntityState.Modified;
            
            await _context.SaveChangesAsync();

            return await GetCart();
        }

        // PUT: api/GioHang/update
        [HttpPut("update")]
        public async Task<ActionResult<GioHangDto>> UpdateCartItem([FromBody] UpdateCartItemDto updateCartDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var gioHang = await _context.GioHangs.FirstOrDefaultAsync(g => g.Id_User == userId);
            if (gioHang == null)
            {
                return NotFound("Giỏ hàng không tồn tại");
            }

            var cartItem = await _context.ChiTietGioHangs
                .FirstOrDefaultAsync(c => c.Id_GioHang == gioHang.Id_GioHang && 
                                     c.Id_SanPham == updateCartDto.Id_SanPham &&
                                     c.KichCo == updateCartDto.KichCo &&
                                     c.MauSac == updateCartDto.MauSac);

            if (cartItem == null)
            {
                return NotFound("Sản phẩm không có trong giỏ hàng");
            }

            if (updateCartDto.SoLuong <= 0)
            {
                // Remove item if quantity is 0 or less
                _context.ChiTietGioHangs.Remove(cartItem);
            }
            else
            {
                var sanPham = await _context.SanPhams.FindAsync(updateCartDto.Id_SanPham);
                if (sanPham == null)
                {
                    return NotFound("Sản phẩm không tồn tại");
                }

                if (updateCartDto.SoLuong > sanPham.SoLuongTon)
                {
                    return BadRequest("Số lượng sản phẩm trong kho không đủ");
                }

                cartItem.SoLuong = updateCartDto.SoLuong;
                _context.Entry(cartItem).State = EntityState.Modified;
            }

            gioHang.NgayCapNhat = DateTime.Now;
            _context.Entry(gioHang).State = EntityState.Modified;
            
            await _context.SaveChangesAsync();

            return await GetCart();
        }

        // DELETE: api/GioHang/remove/5/38/Red
        [HttpDelete("remove/{sanPhamId}/{kichCo}/{mauSac?}")]
        public async Task<ActionResult<GioHangDto>> RemoveCartItem(int sanPhamId, int kichCo, string? mauSac)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var gioHang = await _context.GioHangs.FirstOrDefaultAsync(g => g.Id_User == userId);
            if (gioHang == null)
            {
                return NotFound("Giỏ hàng không tồn tại");
            }

            var cartItem = await _context.ChiTietGioHangs
                .FirstOrDefaultAsync(c => c.Id_GioHang == gioHang.Id_GioHang && 
                                     c.Id_SanPham == sanPhamId &&
                                     c.KichCo == kichCo &&
                                     c.MauSac == mauSac);

            if (cartItem == null)
            {
                return NotFound("Sản phẩm không có trong giỏ hàng");
            }

            _context.ChiTietGioHangs.Remove(cartItem);
            
            gioHang.NgayCapNhat = DateTime.Now;
            _context.Entry(gioHang).State = EntityState.Modified;
            
            await _context.SaveChangesAsync();

            return await GetCart();
        }

        // DELETE: api/GioHang/clear
        [HttpDelete("clear")]
        public async Task<ActionResult<GioHangDto>> ClearCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var gioHang = await _context.GioHangs
                .Include(g => g.ChiTietGioHangs)
                .FirstOrDefaultAsync(g => g.Id_User == userId);

            if (gioHang == null || gioHang.ChiTietGioHangs == null || !gioHang.ChiTietGioHangs.Any())
            {
                return Ok(new GioHangDto
                {
                    Id_GioHang = gioHang?.Id_GioHang ?? string.Empty,
                    Id_User = userId,
                    NgayTao = gioHang?.NgayTao ?? DateTime.Now,
                    NgayCapNhat = DateTime.Now,
                    ChiTietGioHangs = new List<ChiTietGioHangDto>()
                });
            }

            _context.ChiTietGioHangs.RemoveRange(gioHang.ChiTietGioHangs);
            
            gioHang.NgayCapNhat = DateTime.Now;
            _context.Entry(gioHang).State = EntityState.Modified;
            
            await _context.SaveChangesAsync();

            return Ok(new GioHangDto
            {
                Id_GioHang = gioHang.Id_GioHang,
                Id_User = gioHang.Id_User,
                NgayTao = gioHang.NgayTao,
                NgayCapNhat = gioHang.NgayCapNhat,
                ChiTietGioHangs = new List<ChiTietGioHangDto>()
            });
        }
    }
} 