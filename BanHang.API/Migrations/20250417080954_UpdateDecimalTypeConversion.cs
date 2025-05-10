using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BanHang.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDecimalTypeConversion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "GiaBan",
                table: "SanPhams",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AddColumn<string>(
                name: "HinhAnhPhu",
                table: "SanPhams",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SKU",
                table: "SanPhams",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HinhAnh",
                table: "KhuyenMais",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TrangThai",
                table: "KhuyenMais",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SoHoaDon",
                table: "HoaDons",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TrangThai",
                table: "DonHangs",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<decimal>(
                name: "TongTien",
                table: "DonHangs",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AddColumn<string>(
                name: "MaGiamGia",
                table: "DonHangs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PhiVanChuyen",
                table: "DonHangs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "HinhAnh",
                table: "DanhMucs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "DanhMucs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DonGia",
                table: "ChiTietGioHangs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "DonGia",
                table: "ChiTietDonHangs",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AddColumn<decimal>(
                name: "GiaKhuyenMai",
                table: "ChiTietDonHangs",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "KichCo",
                table: "ChiTietDonHangs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MauSac",
                table: "ChiTietDonHangs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HinhAnhPhu",
                table: "SanPhams");

            migrationBuilder.DropColumn(
                name: "SKU",
                table: "SanPhams");

            migrationBuilder.DropColumn(
                name: "HinhAnh",
                table: "KhuyenMais");

            migrationBuilder.DropColumn(
                name: "TrangThai",
                table: "KhuyenMais");

            migrationBuilder.DropColumn(
                name: "SoHoaDon",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "MaGiamGia",
                table: "DonHangs");

            migrationBuilder.DropColumn(
                name: "PhiVanChuyen",
                table: "DonHangs");

            migrationBuilder.DropColumn(
                name: "HinhAnh",
                table: "DanhMucs");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "DanhMucs");

            migrationBuilder.DropColumn(
                name: "DonGia",
                table: "ChiTietGioHangs");

            migrationBuilder.DropColumn(
                name: "GiaKhuyenMai",
                table: "ChiTietDonHangs");

            migrationBuilder.DropColumn(
                name: "KichCo",
                table: "ChiTietDonHangs");

            migrationBuilder.DropColumn(
                name: "MauSac",
                table: "ChiTietDonHangs");

            migrationBuilder.AlterColumn<float>(
                name: "GiaBan",
                table: "SanPhams",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<bool>(
                name: "TrangThai",
                table: "DonHangs",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<float>(
                name: "TongTien",
                table: "DonHangs",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<float>(
                name: "DonGia",
                table: "ChiTietDonHangs",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
