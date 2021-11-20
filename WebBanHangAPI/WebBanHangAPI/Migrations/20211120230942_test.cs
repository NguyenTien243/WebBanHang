using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebBanHangAPI.Migrations
{
    public partial class test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrangThaiGiaoHangs",
                columns: table => new
                {
                    TrangThaiGiaoHangId = table.Column<string>(nullable: false),
                    tenTrangThai = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrangThaiGiaoHangs", x => x.TrangThaiGiaoHangId);
                });

            migrationBuilder.CreateTable(
                name: "HoaDons",
                columns: table => new
                {
                    HoaDonId = table.Column<string>(nullable: false),
                    KhachHangId = table.Column<string>(nullable: true),
                    NhanVienId = table.Column<string>(nullable: true),
                    tongHoaDon = table.Column<double>(nullable: false),
                    ngayXuatDon = table.Column<DateTime>(nullable: false),
                    diaChiGiaoHang = table.Column<string>(nullable: true),
                    TrangThaiGiaoHangId = table.Column<string>(nullable: true),
                    thanhToanOnline = table.Column<bool>(nullable: false),
                    daThanhToan = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDons", x => x.HoaDonId);
                    table.ForeignKey(
                        name: "FK_HoaDons_TrangThaiGiaoHangs_TrangThaiGiaoHangId",
                        column: x => x.TrangThaiGiaoHangId,
                        principalTable: "TrangThaiGiaoHangs",
                        principalColumn: "TrangThaiGiaoHangId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietHDs",
                columns: table => new
                {
                    HoaDonId = table.Column<string>(nullable: false),
                    SanPhamId = table.Column<string>(nullable: false),
                    soLuong = table.Column<int>(nullable: false),
                    giamGia = table.Column<int>(nullable: false),
                    giaTien = table.Column<int>(nullable: false),
                    tongTien = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietHDs", x => new { x.HoaDonId, x.SanPhamId });
                    table.ForeignKey(
                        name: "FK_ChiTietHDs_HoaDons_HoaDonId",
                        column: x => x.HoaDonId,
                        principalTable: "HoaDons",
                        principalColumn: "HoaDonId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietHDs_SanPhams_SanPhamId",
                        column: x => x.SanPhamId,
                        principalTable: "SanPhams",
                        principalColumn: "SanPhamId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietHDs_SanPhamId",
                table: "ChiTietHDs",
                column: "SanPhamId");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_TrangThaiGiaoHangId",
                table: "HoaDons",
                column: "TrangThaiGiaoHangId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietHDs");

            migrationBuilder.DropTable(
                name: "HoaDons");

            migrationBuilder.DropTable(
                name: "TrangThaiGiaoHangs");
        }
    }
}
