using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BanHang.API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTrangThaiFromSanPham : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrangThai",
                table: "SanPhams");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TrangThai",
                table: "SanPhams",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
} 