using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BanHang.API.Migrations
{
    /// <inheritdoc />
    public partial class AddTrangThaiSanPhamEnumColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id_Payment = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id_DonHang = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id_User = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PaymentStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrderInfo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ResponseCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ResponseMessage = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id_Payment);
                    table.ForeignKey(
                        name: "FK_Payments_AspNetUsers_Id_User",
                        column: x => x.Id_User,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_DonHangs_Id_DonHang",
                        column: x => x.Id_DonHang,
                        principalTable: "DonHangs",
                        principalColumn: "Id_DonHang",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vouchers",
                columns: table => new
                {
                    Id_Voucher = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaVoucher = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TenVoucher = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LoaiVoucher = table.Column<int>(type: "int", nullable: false),
                    GiaTri = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GiaTriToiDa = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GiaTriDonHangToiThieu = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: true),
                    SoLuongDaSuDung = table.Column<int>(type: "int", nullable: false),
                    ApDungChoDonHangDauTien = table.Column<bool>(type: "bit", nullable: false),
                    ApDungChoTatCaSanPham = table.Column<bool>(type: "bit", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    Id_NguoiTao = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vouchers", x => x.Id_Voucher);
                    table.ForeignKey(
                        name: "FK_Vouchers_AspNetUsers_Id_NguoiTao",
                        column: x => x.Id_NguoiTao,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VoucherDanhMucs",
                columns: table => new
                {
                    Id_Voucher = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id_DanhMuc = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherDanhMucs", x => new { x.Id_Voucher, x.Id_DanhMuc });
                    table.ForeignKey(
                        name: "FK_VoucherDanhMucs_DanhMucs_Id_DanhMuc",
                        column: x => x.Id_DanhMuc,
                        principalTable: "DanhMucs",
                        principalColumn: "Id_DanhMuc",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VoucherDanhMucs_Vouchers_Id_Voucher",
                        column: x => x.Id_Voucher,
                        principalTable: "Vouchers",
                        principalColumn: "Id_Voucher",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VoucherSanPhams",
                columns: table => new
                {
                    Id_Voucher = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id_SanPham = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherSanPhams", x => new { x.Id_Voucher, x.Id_SanPham });
                    table.ForeignKey(
                        name: "FK_VoucherSanPhams_SanPhams_Id_SanPham",
                        column: x => x.Id_SanPham,
                        principalTable: "SanPhams",
                        principalColumn: "Id_SanPham",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VoucherSanPhams_Vouchers_Id_Voucher",
                        column: x => x.Id_Voucher,
                        principalTable: "Vouchers",
                        principalColumn: "Id_Voucher",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VoucherSuDungs",
                columns: table => new
                {
                    Id_VoucherSuDung = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id_Voucher = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id_DonHang = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id_User = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NgaySuDung = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GiaTriApDung = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherSuDungs", x => x.Id_VoucherSuDung);
                    table.ForeignKey(
                        name: "FK_VoucherSuDungs_AspNetUsers_Id_User",
                        column: x => x.Id_User,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VoucherSuDungs_DonHangs_Id_DonHang",
                        column: x => x.Id_DonHang,
                        principalTable: "DonHangs",
                        principalColumn: "Id_DonHang",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VoucherSuDungs_Vouchers_Id_Voucher",
                        column: x => x.Id_Voucher,
                        principalTable: "Vouchers",
                        principalColumn: "Id_Voucher",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Id_DonHang",
                table: "Payments",
                column: "Id_DonHang");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Id_User",
                table: "Payments",
                column: "Id_User");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherDanhMucs_Id_DanhMuc",
                table: "VoucherDanhMucs",
                column: "Id_DanhMuc");

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_Id_NguoiTao",
                table: "Vouchers",
                column: "Id_NguoiTao");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherSanPhams_Id_SanPham",
                table: "VoucherSanPhams",
                column: "Id_SanPham");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherSuDungs_Id_DonHang",
                table: "VoucherSuDungs",
                column: "Id_DonHang");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherSuDungs_Id_User",
                table: "VoucherSuDungs",
                column: "Id_User");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherSuDungs_Id_Voucher",
                table: "VoucherSuDungs",
                column: "Id_Voucher");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "VoucherDanhMucs");

            migrationBuilder.DropTable(
                name: "VoucherSanPhams");

            migrationBuilder.DropTable(
                name: "VoucherSuDungs");

            migrationBuilder.DropTable(
                name: "Vouchers");
        }
    }
}
