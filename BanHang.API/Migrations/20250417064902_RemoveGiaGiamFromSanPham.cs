using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BanHang.API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveGiaGiamFromSanPham : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GiaGiam",
                table: "SanPhams");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "GiaGiam",
                table: "SanPhams",
                type: "real",
                nullable: true);
        }
    }
}
