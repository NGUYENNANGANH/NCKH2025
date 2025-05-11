using BanHang.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BanHang.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<DanhMuc> DanhMucs { get; set; }
        public DbSet<GioHang> GioHangs { get; set; }
        public DbSet<ChiTietGioHang> ChiTietGioHangs { get; set; }
        public DbSet<DanhGia> DanhGias { get; set; }
        public DbSet<KhuyenMai> KhuyenMais { get; set; }
        public DbSet<SanPham_KhuyenMai> SanPham_KhuyenMais { get; set; }
        public DbSet<DonHang> DonHangs { get; set; }
        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        
        // Thêm DbSet cho các mô hình mới
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình precision cho các trường decimal
            // SanPham
            modelBuilder.Entity<SanPham>()
                .Property(p => p.GiaBan)
                .HasPrecision(18, 2);

            // ChiTietDonHang
            modelBuilder.Entity<ChiTietDonHang>()
                .Property(c => c.DonGia)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ChiTietDonHang>()
                .Property(c => c.GiaKhuyenMai)
                .HasPrecision(18, 2);

            // ChiTietGioHang
            modelBuilder.Entity<ChiTietGioHang>()
                .Property(c => c.DonGia)
                .HasPrecision(18, 2);

            // DonHang
            modelBuilder.Entity<DonHang>()
                .Property(d => d.TongTien)
                .HasPrecision(18, 2);

            modelBuilder.Entity<DonHang>()
                .Property(d => d.PhiVanChuyen)
                .HasPrecision(18, 2);

            // Payment
            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            // Configure composite keys
            modelBuilder.Entity<ChiTietGioHang>()
                .HasKey(c => new { c.Id_GioHang, c.Id_SanPham });

            modelBuilder.Entity<ChiTietDonHang>()
                .HasKey(c => new { c.Id_DonHang, c.Id_SanPham });

            modelBuilder.Entity<SanPham_KhuyenMai>()
                .HasKey(c => new { c.Id_KhuyenMai, c.Id_SanPham });

            // Configure one-to-many relationships
            // DanhMuc -> SanPham
            modelBuilder.Entity<DanhMuc>()
                .HasMany(d => d.SanPhams)
                .WithOne(s => s.DanhMuc)
                .HasForeignKey(s => s.Id_DanhMuc)
                .OnDelete(DeleteBehavior.Restrict);

            // GioHang -> ChiTietGioHang
            modelBuilder.Entity<GioHang>()
                .HasMany(g => g.ChiTietGioHangs)
                .WithOne(c => c.GioHang)
                .HasForeignKey(c => c.Id_GioHang)
                .OnDelete(DeleteBehavior.Cascade);

            // DonHang -> ChiTietDonHang
            modelBuilder.Entity<DonHang>()
                .HasMany(d => d.ChiTietDonHangs)
                .WithOne(c => c.DonHang)
                .HasForeignKey(c => c.Id_DonHang)
                .OnDelete(DeleteBehavior.Cascade);

            // SanPham -> ChiTietGioHang
            modelBuilder.Entity<SanPham>()
                .HasMany(s => s.ChiTietGioHangs)
                .WithOne(c => c.SanPham)
                .HasForeignKey(c => c.Id_SanPham)
                .OnDelete(DeleteBehavior.Restrict);

            // SanPham -> ChiTietDonHang
            modelBuilder.Entity<SanPham>()
                .HasMany(s => s.ChiTietDonHangs)
                .WithOne(c => c.SanPham)
                .HasForeignKey(c => c.Id_SanPham)
                .OnDelete(DeleteBehavior.Restrict);

            // SanPham -> DanhGia
            modelBuilder.Entity<SanPham>()
                .HasMany(s => s.DanhGias)
                .WithOne(d => d.SanPham)
                .HasForeignKey(d => d.Id_SanPham)
                .OnDelete(DeleteBehavior.Cascade);

            // KhuyenMai -> SanPham_KhuyenMai
            modelBuilder.Entity<KhuyenMai>()
                .HasMany(k => k.SanPham_KhuyenMais)
                .WithOne(s => s.KhuyenMai)
                .HasForeignKey(s => s.Id_KhuyenMai)
                .OnDelete(DeleteBehavior.Cascade);

            // SanPham -> SanPham_KhuyenMai
            modelBuilder.Entity<SanPham>()
                .HasMany(s => s.SanPham_KhuyenMais)
                .WithOne(k => k.SanPham)
                .HasForeignKey(k => k.Id_SanPham)
                .OnDelete(DeleteBehavior.Cascade);

            // ApplicationUser -> GioHang
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<GioHang>()
                .WithOne(g => g.User)
                .HasForeignKey(g => g.Id_User)
                .OnDelete(DeleteBehavior.Restrict);

            // ApplicationUser -> DonHang
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<DonHang>()
                .WithOne(d => d.KhachHang)
                .HasForeignKey(d => d.User_Id)
                .OnDelete(DeleteBehavior.Cascade);

            // ApplicationUser -> DanhGia
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<DanhGia>()
                .WithOne(d => d.User)
                .HasForeignKey(d => d.Id_User)
                .OnDelete(DeleteBehavior.Restrict);

            // ApplicationUser -> HoaDon
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<HoaDon>()
                .WithOne(h => h.User)
                .HasForeignKey(h => h.Id_User)
                .OnDelete(DeleteBehavior.Restrict);

            // DonHang -> HoaDon (one-to-one)
            modelBuilder.Entity<DonHang>()
                .HasOne(d => d.HoaDon)
                .WithOne(h => h.DonHang)
                .HasForeignKey<HoaDon>(h => h.Id_DonHang)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Payment relationships
            modelBuilder.Entity<DonHang>()
                .HasMany<Payment>()
                .WithOne(p => p.DonHang)
                .HasForeignKey(p => p.Id_DonHang)
                .OnDelete(DeleteBehavior.Restrict);
                
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<Payment>()
                .WithOne(p => p.User)
                .HasForeignKey(p => p.Id_User)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 