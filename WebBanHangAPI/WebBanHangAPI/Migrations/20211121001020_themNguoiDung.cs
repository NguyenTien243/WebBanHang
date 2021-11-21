using Microsoft.EntityFrameworkCore.Migrations;

namespace WebBanHangAPI.Migrations
{
    public partial class themNguoiDung : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KhachHangId",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "NhanVienId",
                table: "HoaDons");

            migrationBuilder.AddColumn<string>(
                name: "NguoiDungId",
                table: "HoaDons",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VaiTros",
                columns: table => new
                {
                    VaiTroId = table.Column<string>(nullable: false),
                    tenVaiTro = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaiTros", x => x.VaiTroId);
                });

            migrationBuilder.CreateTable(
                name: "NguoiDung",
                columns: table => new
                {
                    NguoiDungId = table.Column<string>(nullable: false),
                    tenNguoiDung = table.Column<string>(nullable: true),
                    email = table.Column<string>(nullable: true),
                    sDT = table.Column<string>(nullable: true),
                    diaChi = table.Column<string>(nullable: true),
                    gioiTinh = table.Column<bool>(nullable: false),
                    tenDangNhap = table.Column<string>(nullable: true),
                    matKhau = table.Column<string>(nullable: true),
                    VaiTroId = table.Column<string>(nullable: true),
                    conHoatDong = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDung", x => x.NguoiDungId);
                    table.ForeignKey(
                        name: "FK_NguoiDung_VaiTros_VaiTroId",
                        column: x => x.VaiTroId,
                        principalTable: "VaiTros",
                        principalColumn: "VaiTroId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GioHangs",
                columns: table => new
                {
                    NguoiDungId = table.Column<string>(nullable: false),
                    SanPhamId = table.Column<string>(nullable: false),
                    soLuong = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GioHangs", x => new { x.NguoiDungId, x.SanPhamId });
                    table.ForeignKey(
                        name: "FK_GioHangs_NguoiDung_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "NguoiDung",
                        principalColumn: "NguoiDungId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GioHangs_SanPhams_SanPhamId",
                        column: x => x.SanPhamId,
                        principalTable: "SanPhams",
                        principalColumn: "SanPhamId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_NguoiDungId",
                table: "HoaDons",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_GioHangs_SanPhamId",
                table: "GioHangs",
                column: "SanPhamId");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_VaiTroId",
                table: "NguoiDung",
                column: "VaiTroId");

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDons_NguoiDung_NguoiDungId",
                table: "HoaDons",
                column: "NguoiDungId",
                principalTable: "NguoiDung",
                principalColumn: "NguoiDungId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HoaDons_NguoiDung_NguoiDungId",
                table: "HoaDons");

            migrationBuilder.DropTable(
                name: "GioHangs");

            migrationBuilder.DropTable(
                name: "NguoiDung");

            migrationBuilder.DropTable(
                name: "VaiTros");

            migrationBuilder.DropIndex(
                name: "IX_HoaDons_NguoiDungId",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "NguoiDungId",
                table: "HoaDons");

            migrationBuilder.AddColumn<string>(
                name: "KhachHangId",
                table: "HoaDons",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NhanVienId",
                table: "HoaDons",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
