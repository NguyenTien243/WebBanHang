using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebBanHangAPI.Migrations
{
    public partial class WebBanHang : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MaGiamGias",
                columns: table => new
                {
                    MaGiamGiaId = table.Column<string>(nullable: false),
                    TenMaGiamGia = table.Column<string>(nullable: true),
                    NoiDungChiTiet = table.Column<string>(nullable: true),
                    NgayHetHang = table.Column<DateTime>(nullable: true),
                    NgayBatDau = table.Column<DateTime>(nullable: true),
                    GiamToiDa = table.Column<double>(nullable: false),
                    DonToiThieu = table.Column<double>(nullable: false),
                    KieuGiam = table.Column<int>(nullable: false),
                    GiamGia = table.Column<int>(nullable: false),
                    SoLuongSuDUng = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaGiamGias", x => x.MaGiamGiaId);
                });

            migrationBuilder.CreateTable(
                name: "MaGiamGiaCuaNgDungs",
                columns: table => new
                {
                    NguoiDungId = table.Column<string>(nullable: false),
                    MaGiamGiaId = table.Column<string>(nullable: false),
                    DaSuDung = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaGiamGiaCuaNgDungs", x => new { x.MaGiamGiaId, x.NguoiDungId });
                    table.ForeignKey(
                        name: "FK_MaGiamGiaCuaNgDungs_MaGiamGias_MaGiamGiaId",
                        column: x => x.MaGiamGiaId,
                        principalTable: "MaGiamGias",
                        principalColumn: "MaGiamGiaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaGiamGiaCuaNgDungs_NguoiDungs_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "NguoiDungs",
                        principalColumn: "NguoiDungId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaGiamGiaCuaNgDungs_NguoiDungId",
                table: "MaGiamGiaCuaNgDungs",
                column: "NguoiDungId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaGiamGiaCuaNgDungs");

            migrationBuilder.DropTable(
                name: "MaGiamGias");
        }
    }
}
