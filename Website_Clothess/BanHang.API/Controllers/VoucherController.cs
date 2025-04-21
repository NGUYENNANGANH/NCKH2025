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
    public class VoucherController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VoucherController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Voucher
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<IEnumerable<VoucherDto>>> GetVouchers()
        {
            var vouchers = await _context.Vouchers
                .Include(v => v.NguoiTao)
                .OrderByDescending(v => v.NgayTao)
                .ToListAsync();

            var voucherDtos = vouchers.Select(v => new VoucherDto
            {
                Id_Voucher = v.Id_Voucher,
                MaVoucher = v.MaVoucher,
                TenVoucher = v.TenVoucher,
                MoTa = v.MoTa,
                LoaiVoucher = v.LoaiVoucher,
                GiaTri = v.GiaTri,
                GiaTriToiDa = v.GiaTriToiDa,
                GiaTriDonHangToiThieu = v.GiaTriDonHangToiThieu,
                NgayBatDau = v.NgayBatDau,
                NgayKetThuc = v.NgayKetThuc,
                SoLuong = v.SoLuong,
                SoLuongDaSuDung = v.SoLuongDaSuDung,
                ApDungChoDonHangDauTien = v.ApDungChoDonHangDauTien,
                ApDungChoTatCaSanPham = v.ApDungChoTatCaSanPham,
                TrangThai = v.TrangThai,
                NgayTao = v.NgayTao,
                NgayCapNhat = v.NgayCapNhat
            }).ToList();

            return Ok(voucherDtos);
        }

        // GET: api/Voucher/active
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<VoucherDto>>> GetActiveVouchers()
        {
            var now = DateTime.Now;
            var vouchers = await _context.Vouchers
                .Where(v => v.TrangThai && v.NgayBatDau <= now && v.NgayKetThuc >= now && 
                         (!v.SoLuong.HasValue || v.SoLuongDaSuDung < v.SoLuong.Value))
                .OrderByDescending(v => v.NgayTao)
                .ToListAsync();

            var voucherDtos = vouchers.Select(v => new VoucherDto
            {
                Id_Voucher = v.Id_Voucher,
                MaVoucher = v.MaVoucher,
                TenVoucher = v.TenVoucher,
                MoTa = v.MoTa,
                LoaiVoucher = v.LoaiVoucher,
                GiaTri = v.GiaTri,
                GiaTriToiDa = v.GiaTriToiDa,
                GiaTriDonHangToiThieu = v.GiaTriDonHangToiThieu,
                NgayBatDau = v.NgayBatDau,
                NgayKetThuc = v.NgayKetThuc,
                SoLuong = v.SoLuong,
                SoLuongDaSuDung = v.SoLuongDaSuDung,
                ApDungChoDonHangDauTien = v.ApDungChoDonHangDauTien,
                ApDungChoTatCaSanPham = v.ApDungChoTatCaSanPham,
                TrangThai = v.TrangThai,
                NgayTao = v.NgayTao,
                NgayCapNhat = v.NgayCapNhat
            }).ToList();

            return Ok(voucherDtos);
        }

        // GET: api/Voucher/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VoucherDto>> GetVoucher(string id)
        {
            var voucher = await _context.Vouchers
                .Include(v => v.NguoiTao)
                .Include(v => v.VoucherDanhMucs)
                .ThenInclude(vd => vd.DanhMuc)
                .Include(v => v.VoucherSanPhams)
                .ThenInclude(vs => vs.SanPham)
                .FirstOrDefaultAsync(v => v.Id_Voucher == id);

            if (voucher == null)
            {
                return NotFound();
            }

            var voucherDto = new VoucherDto
            {
                Id_Voucher = voucher.Id_Voucher,
                MaVoucher = voucher.MaVoucher,
                TenVoucher = voucher.TenVoucher,
                MoTa = voucher.MoTa,
                LoaiVoucher = voucher.LoaiVoucher,
                GiaTri = voucher.GiaTri,
                GiaTriToiDa = voucher.GiaTriToiDa,
                GiaTriDonHangToiThieu = voucher.GiaTriDonHangToiThieu,
                NgayBatDau = voucher.NgayBatDau,
                NgayKetThuc = voucher.NgayKetThuc,
                SoLuong = voucher.SoLuong,
                SoLuongDaSuDung = voucher.SoLuongDaSuDung,
                ApDungChoDonHangDauTien = voucher.ApDungChoDonHangDauTien,
                ApDungChoTatCaSanPham = voucher.ApDungChoTatCaSanPham,
                TrangThai = voucher.TrangThai,
                NgayTao = voucher.NgayTao,
                NgayCapNhat = voucher.NgayCapNhat
            };

            return Ok(voucherDto);
        }

        // POST: api/Voucher
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<VoucherDto>> CreateVoucher([FromBody] CreateVoucherDto createVoucherDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra mã voucher đã tồn tại chưa
            var existingVoucher = await _context.Vouchers.FirstOrDefaultAsync(v => v.MaVoucher == createVoucherDto.MaVoucher);
            if (existingVoucher != null)
            {
                return BadRequest("Mã voucher đã tồn tại");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var voucher = new Voucher
            {
                MaVoucher = createVoucherDto.MaVoucher.ToUpper(),
                TenVoucher = createVoucherDto.TenVoucher,
                MoTa = createVoucherDto.MoTa,
                LoaiVoucher = createVoucherDto.LoaiVoucher,
                GiaTri = createVoucherDto.GiaTri,
                GiaTriToiDa = createVoucherDto.GiaTriToiDa,
                GiaTriDonHangToiThieu = createVoucherDto.GiaTriDonHangToiThieu,
                NgayBatDau = createVoucherDto.NgayBatDau,
                NgayKetThuc = createVoucherDto.NgayKetThuc,
                SoLuong = createVoucherDto.SoLuong,
                SoLuongDaSuDung = 0,
                ApDungChoDonHangDauTien = createVoucherDto.ApDungChoDonHangDauTien,
                ApDungChoTatCaSanPham = createVoucherDto.ApDungChoTatCaSanPham,
                TrangThai = createVoucherDto.TrangThai,
                Id_NguoiTao = userId,
                NgayTao = DateTime.Now
            };

            _context.Vouchers.Add(voucher);
            await _context.SaveChangesAsync();

            // Nếu không áp dụng cho tất cả sản phẩm, thêm các ràng buộc
            if (!createVoucherDto.ApDungChoTatCaSanPham)
            {
                // Thêm danh mục áp dụng
                if (createVoucherDto.DanhMucIds != null && createVoucherDto.DanhMucIds.Any())
                {
                    foreach (var danhMucId in createVoucherDto.DanhMucIds)
                    {
                        if (await _context.DanhMucs.AnyAsync(d => d.Id_DanhMuc == danhMucId))
                        {
                            _context.VoucherDanhMucs.Add(new VoucherDanhMuc
                            {
                                Id_Voucher = voucher.Id_Voucher,
                                Id_DanhMuc = danhMucId
                            });
                        }
                    }
                }

                // Thêm sản phẩm áp dụng
                if (createVoucherDto.SanPhamIds != null && createVoucherDto.SanPhamIds.Any())
                {
                    foreach (var sanPhamId in createVoucherDto.SanPhamIds)
                    {
                        if (await _context.SanPhams.AnyAsync(s => s.Id_SanPham == sanPhamId))
                        {
                            _context.VoucherSanPhams.Add(new VoucherSanPham
                            {
                                Id_Voucher = voucher.Id_Voucher,
                                Id_SanPham = sanPhamId
                            });
                        }
                    }
                }

                await _context.SaveChangesAsync();
            }

            var voucherDto = new VoucherDto
            {
                Id_Voucher = voucher.Id_Voucher,
                MaVoucher = voucher.MaVoucher,
                TenVoucher = voucher.TenVoucher,
                MoTa = voucher.MoTa,
                LoaiVoucher = voucher.LoaiVoucher,
                GiaTri = voucher.GiaTri,
                GiaTriToiDa = voucher.GiaTriToiDa,
                GiaTriDonHangToiThieu = voucher.GiaTriDonHangToiThieu,
                NgayBatDau = voucher.NgayBatDau,
                NgayKetThuc = voucher.NgayKetThuc,
                SoLuong = voucher.SoLuong,
                SoLuongDaSuDung = voucher.SoLuongDaSuDung,
                ApDungChoDonHangDauTien = voucher.ApDungChoDonHangDauTien,
                ApDungChoTatCaSanPham = voucher.ApDungChoTatCaSanPham,
                TrangThai = voucher.TrangThai,
                NgayTao = voucher.NgayTao,
                NgayCapNhat = voucher.NgayCapNhat
            };

            return CreatedAtAction("GetVoucher", new { id = voucher.Id_Voucher }, voucherDto);
        }

        // PUT: api/Voucher/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateVoucher(string id, [FromBody] UpdateVoucherDto updateVoucherDto)
        {
            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher == null)
            {
                return NotFound();
            }

            // Cập nhật các thuộc tính nếu được gửi lên
            if (!string.IsNullOrEmpty(updateVoucherDto.TenVoucher))
                voucher.TenVoucher = updateVoucherDto.TenVoucher;

            if (updateVoucherDto.MoTa != null)
                voucher.MoTa = updateVoucherDto.MoTa;

            if (updateVoucherDto.LoaiVoucher.HasValue)
                voucher.LoaiVoucher = updateVoucherDto.LoaiVoucher.Value;

            if (updateVoucherDto.GiaTri.HasValue)
                voucher.GiaTri = updateVoucherDto.GiaTri.Value;

            if (updateVoucherDto.GiaTriToiDa.HasValue)
                voucher.GiaTriToiDa = updateVoucherDto.GiaTriToiDa;

            if (updateVoucherDto.GiaTriDonHangToiThieu.HasValue)
                voucher.GiaTriDonHangToiThieu = updateVoucherDto.GiaTriDonHangToiThieu;

            if (updateVoucherDto.NgayBatDau.HasValue)
                voucher.NgayBatDau = updateVoucherDto.NgayBatDau.Value;

            if (updateVoucherDto.NgayKetThuc.HasValue)
                voucher.NgayKetThuc = updateVoucherDto.NgayKetThuc.Value;

            if (updateVoucherDto.SoLuong.HasValue)
                voucher.SoLuong = updateVoucherDto.SoLuong;

            if (updateVoucherDto.ApDungChoDonHangDauTien.HasValue)
                voucher.ApDungChoDonHangDauTien = updateVoucherDto.ApDungChoDonHangDauTien.Value;

            if (updateVoucherDto.ApDungChoTatCaSanPham.HasValue)
                voucher.ApDungChoTatCaSanPham = updateVoucherDto.ApDungChoTatCaSanPham.Value;

            if (updateVoucherDto.TrangThai.HasValue)
                voucher.TrangThai = updateVoucherDto.TrangThai.Value;

            voucher.NgayCapNhat = DateTime.Now;

            _context.Entry(voucher).State = EntityState.Modified;

            // Cập nhật danh mục và sản phẩm áp dụng nếu không áp dụng cho tất cả sản phẩm
            if (updateVoucherDto.ApDungChoTatCaSanPham.HasValue && !updateVoucherDto.ApDungChoTatCaSanPham.Value)
            {
                // Xóa các áp dụng cũ
                var existingDanhMucs = await _context.VoucherDanhMucs.Where(vd => vd.Id_Voucher == id).ToListAsync();
                _context.VoucherDanhMucs.RemoveRange(existingDanhMucs);

                var existingSanPhams = await _context.VoucherSanPhams.Where(vs => vs.Id_Voucher == id).ToListAsync();
                _context.VoucherSanPhams.RemoveRange(existingSanPhams);

                // Thêm danh mục áp dụng mới
                if (updateVoucherDto.DanhMucIds != null && updateVoucherDto.DanhMucIds.Any())
                {
                    foreach (var danhMucId in updateVoucherDto.DanhMucIds)
                    {
                        if (await _context.DanhMucs.AnyAsync(d => d.Id_DanhMuc == danhMucId))
                        {
                            _context.VoucherDanhMucs.Add(new VoucherDanhMuc
                            {
                                Id_Voucher = voucher.Id_Voucher,
                                Id_DanhMuc = danhMucId
                            });
                        }
                    }
                }

                // Thêm sản phẩm áp dụng mới
                if (updateVoucherDto.SanPhamIds != null && updateVoucherDto.SanPhamIds.Any())
                {
                    foreach (var sanPhamId in updateVoucherDto.SanPhamIds)
                    {
                        if (await _context.SanPhams.AnyAsync(s => s.Id_SanPham == sanPhamId))
                        {
                            _context.VoucherSanPhams.Add(new VoucherSanPham
                            {
                                Id_Voucher = voucher.Id_Voucher,
                                Id_SanPham = sanPhamId
                            });
                        }
                    }
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VoucherExists(id))
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

        // DELETE: api/Voucher/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteVoucher(string id)
        {
            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher == null)
            {
                return NotFound();
            }

            // Kiểm tra xem voucher đã được sử dụng chưa
            bool hasBeenUsed = await _context.VoucherSuDungs.AnyAsync(v => v.Id_Voucher == id);
            if (hasBeenUsed)
            {
                // Không xóa, chỉ đánh dấu là không còn hiệu lực
                voucher.TrangThai = false;
                voucher.NgayCapNhat = DateTime.Now;
                _context.Entry(voucher).State = EntityState.Modified;
            }
            else
            {
                // Xóa các ràng buộc trước
                var voucherDanhMucs = await _context.VoucherDanhMucs.Where(vd => vd.Id_Voucher == id).ToListAsync();
                _context.VoucherDanhMucs.RemoveRange(voucherDanhMucs);

                var voucherSanPhams = await _context.VoucherSanPhams.Where(vs => vs.Id_Voucher == id).ToListAsync();
                _context.VoucherSanPhams.RemoveRange(voucherSanPhams);

                // Xóa voucher
                _context.Vouchers.Remove(voucher);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Voucher/check
        [HttpPost("check")]
        [Authorize]
        public async Task<ActionResult<VoucherAppliedDto>> CheckVoucher([FromBody] ApplyVoucherDto applyVoucherDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Lấy thông tin giỏ hàng
            var gioHang = await _context.GioHangs
                .Include(g => g.ChiTietGioHangs)
                .ThenInclude(c => c.SanPham)
                .ThenInclude(s => s.SanPham_KhuyenMais)
                .ThenInclude(sk => sk.KhuyenMai)
                .FirstOrDefaultAsync(g => g.Id_User == userId);

            if (gioHang == null || gioHang.ChiTietGioHangs == null || !gioHang.ChiTietGioHangs.Any())
            {
                return BadRequest("Giỏ hàng trống");
            }

            // Tính tổng tiền
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
                                   sk.TrangThai &&
                                   now >= sk.KhuyenMai.NgayBatDau &&
                                   now <= sk.KhuyenMai.NgayKetThuc)
                        .OrderByDescending(sk => sk.MucGiamGiaRieng ?? sk.KhuyenMai!.PhanTramGiam)
                        .FirstOrDefault();

                    if (maxDiscount != null && maxDiscount.KhuyenMai != null)
                    {
                        var phanTramGiam = maxDiscount.MucGiamGiaRieng ?? maxDiscount.KhuyenMai!.PhanTramGiam;
                        var discountPercent = phanTramGiam / 100.0m;
                        giaGiam = giaBan * (1 - discountPercent);
                    }
                }

                tongTien += giaGiam * item.SoLuong;
            }

            // Tìm voucher theo mã
            var voucher = await _context.Vouchers
                .Include(v => v.VoucherDanhMucs)
                .Include(v => v.VoucherSanPhams)
                .FirstOrDefaultAsync(v => v.MaVoucher == applyVoucherDto.MaVoucher.ToUpper() && v.TrangThai);

            if (voucher == null)
            {
                return NotFound("Mã voucher không tồn tại hoặc đã hết hạn");
            }

            // Kiểm tra điều kiện hiệu lực
            var currentTime = DateTime.Now;
            if (currentTime < voucher.NgayBatDau || currentTime > voucher.NgayKetThuc)
            {
                return BadRequest("Voucher chưa đến hoặc đã hết thời gian áp dụng");
            }

            // Kiểm tra số lượng còn lại
            if (voucher.SoLuong.HasValue && voucher.SoLuongDaSuDung >= voucher.SoLuong.Value)
            {
                return BadRequest("Voucher đã hết lượt sử dụng");
            }

            // Kiểm tra điều kiện đơn hàng đầu tiên
            if (voucher.ApDungChoDonHangDauTien)
            {
                bool hasPreviousOrder = await _context.DonHangs.AnyAsync(d => d.Id_KH == userId);
                if (hasPreviousOrder)
                {
                    return BadRequest("Voucher chỉ áp dụng cho đơn hàng đầu tiên");
                }
            }

            // Kiểm tra giá trị đơn hàng tối thiểu
            if (voucher.GiaTriDonHangToiThieu.HasValue && tongTien < voucher.GiaTriDonHangToiThieu.Value)
            {
                return BadRequest($"Giá trị đơn hàng tối thiểu để áp dụng voucher là {voucher.GiaTriDonHangToiThieu.Value:N0} VNĐ");
            }

            // Kiểm tra điều kiện áp dụng cho danh mục/sản phẩm cụ thể
            if (!voucher.ApDungChoTatCaSanPham)
            {
                // Lấy danh sách ID danh mục và sản phẩm được áp dụng
                var danhMucIds = voucher.VoucherDanhMucs?.Select(vd => vd.Id_DanhMuc).ToList() ?? new List<int>();
                var sanPhamIds = voucher.VoucherSanPhams?.Select(vs => vs.Id_SanPham).ToList() ?? new List<int>();

                // Kiểm tra xem giỏ hàng có chứa ít nhất một sản phẩm thuộc danh mục hoặc sản phẩm được áp dụng không
                bool hasValidProduct = false;
                foreach (var item in gioHang.ChiTietGioHangs)
                {
                    if (sanPhamIds.Contains(item.Id_SanPham) || 
                        (item.SanPham != null && danhMucIds.Contains(item.SanPham.Id_DanhMuc)))
                    {
                        hasValidProduct = true;
                        break;
                    }
                }

                if (!hasValidProduct)
                {
                    return BadRequest("Voucher không áp dụng cho các sản phẩm trong giỏ hàng");
                }
            }

            // Tính giá trị được giảm
            decimal giaTriGiam = 0;
            
            switch (voucher.LoaiVoucher)
            {
                case LoaiVoucher.GiamTheoPhanTram:
                    giaTriGiam = tongTien * (voucher.GiaTri / 100);
                    // Kiểm tra giới hạn giảm giá
                    if (voucher.GiaTriToiDa.HasValue && giaTriGiam > voucher.GiaTriToiDa.Value)
                    {
                        giaTriGiam = voucher.GiaTriToiDa.Value;
                    }
                    break;
                    
                case LoaiVoucher.GiamTheoSoTien:
                    giaTriGiam = voucher.GiaTri;
                    if (giaTriGiam > tongTien)
                    {
                        giaTriGiam = tongTien;
                    }
                    break;
                    
                case LoaiVoucher.FreeShip:
                    // Giá trị giảm là phí vận chuyển, tạm tính là giá trị voucher
                    giaTriGiam = voucher.GiaTri;
                    // Nếu có giới hạn tối đa
                    if (voucher.GiaTriToiDa.HasValue && giaTriGiam > voucher.GiaTriToiDa.Value)
                    {
                        giaTriGiam = voucher.GiaTriToiDa.Value;
                    }
                    break;
            }

            // Trả về kết quả
            var voucherAppliedDto = new VoucherAppliedDto
            {
                Id_Voucher = voucher.Id_Voucher,
                MaVoucher = voucher.MaVoucher,
                TenVoucher = voucher.TenVoucher,
                LoaiVoucher = voucher.LoaiVoucher,
                GiaTri = voucher.GiaTri,
                GiaTriApDung = giaTriGiam,
                TongTienTruocGiam = tongTien,
                TongTienSauGiam = tongTien - giaTriGiam
            };

            return Ok(voucherAppliedDto);
        }

        // Hàm kiểm tra voucher tồn tại
        private bool VoucherExists(string id)
        {
            return _context.Vouchers.Any(v => v.Id_Voucher == id);
        }
    }
} 