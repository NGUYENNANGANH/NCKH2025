using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BanHang.API.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreFieldsToSanPham_KhuyenMai : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GhiChu",
                table: "SanPham_KhuyenMais",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Id_NguoiCapNhat",
                table: "SanPham_KhuyenMais",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Id_NguoiTao",
                table: "SanPham_KhuyenMais",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MucGiamGiaRieng",
                table: "SanPham_KhuyenMais",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayCapNhat",
                table: "SanPham_KhuyenMais",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayKetThuc",
                table: "SanPham_KhuyenMais",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayTao",
                table: "SanPham_KhuyenMais",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "SoLuongGioiHan",
                table: "SanPham_KhuyenMais",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ThuTuUuTien",
                table: "SanPham_KhuyenMais",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "TrangThai",
                table: "SanPham_KhuyenMais",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_SanPham_KhuyenMais_Id_NguoiCapNhat",
                table: "SanPham_KhuyenMais",
                column: "Id_NguoiCapNhat");

            migrationBuilder.CreateIndex(
                name: "IX_SanPham_KhuyenMais_Id_NguoiTao",
                table: "SanPham_KhuyenMais",
                column: "Id_NguoiTao");

            migrationBuilder.AddForeignKey(
                name: "FK_SanPham_KhuyenMais_AspNetUsers_Id_NguoiCapNhat",
                table: "SanPham_KhuyenMais",
                column: "Id_NguoiCapNhat",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SanPham_KhuyenMais_AspNetUsers_Id_NguoiTao",
                table: "SanPham_KhuyenMais",
                column: "Id_NguoiTao",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SanPham_KhuyenMais_AspNetUsers_Id_NguoiCapNhat",
                table: "SanPham_KhuyenMais");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPham_KhuyenMais_AspNetUsers_Id_NguoiTao",
                table: "SanPham_KhuyenMais");

            migrationBuilder.DropIndex(
                name: "IX_SanPham_KhuyenMais_Id_NguoiCapNhat",
                table: "SanPham_KhuyenMais");

            migrationBuilder.DropIndex(
                name: "IX_SanPham_KhuyenMais_Id_NguoiTao",
                table: "SanPham_KhuyenMais");

            migrationBuilder.DropColumn(
                name: "GhiChu",
                table: "SanPham_KhuyenMais");

            migrationBuilder.DropColumn(
                name: "Id_NguoiCapNhat",
                table: "SanPham_KhuyenMais");

            migrationBuilder.DropColumn(
                name: "Id_NguoiTao",
                table: "SanPham_KhuyenMais");

            migrationBuilder.DropColumn(
                name: "MucGiamGiaRieng",
                table: "SanPham_KhuyenMais");

            migrationBuilder.DropColumn(
                name: "NgayCapNhat",
                table: "SanPham_KhuyenMais");

            migrationBuilder.DropColumn(
                name: "NgayKetThuc",
                table: "SanPham_KhuyenMais");

            migrationBuilder.DropColumn(
                name: "NgayTao",
                table: "SanPham_KhuyenMais");

            migrationBuilder.DropColumn(
                name: "SoLuongGioiHan",
                table: "SanPham_KhuyenMais");

            migrationBuilder.DropColumn(
                name: "ThuTuUuTien",
                table: "SanPham_KhuyenMais");

            migrationBuilder.DropColumn(
                name: "TrangThai",
                table: "SanPham_KhuyenMais");
        }
    }
}
