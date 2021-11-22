using Microsoft.EntityFrameworkCore.Migrations;

namespace WebBanHangAPI.Migrations
{
    public partial class bosungNguoiDung : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GioHangs_NguoiDung_NguoiDungId",
                table: "GioHangs");

            migrationBuilder.DropForeignKey(
                name: "FK_GioHangs_SanPhams_SanPhamId",
                table: "GioHangs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GioHangs",
                table: "GioHangs");

            migrationBuilder.RenameTable(
                name: "GioHangs",
                newName: "GioHang");

            migrationBuilder.RenameIndex(
                name: "IX_GioHangs_SanPhamId",
                table: "GioHang",
                newName: "IX_GioHang_SanPhamId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GioHang",
                table: "GioHang",
                columns: new[] { "NguoiDungId", "SanPhamId" });

            migrationBuilder.AddForeignKey(
                name: "FK_GioHang_NguoiDung_NguoiDungId",
                table: "GioHang",
                column: "NguoiDungId",
                principalTable: "NguoiDung",
                principalColumn: "NguoiDungId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GioHang_SanPhams_SanPhamId",
                table: "GioHang",
                column: "SanPhamId",
                principalTable: "SanPhams",
                principalColumn: "SanPhamId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GioHang_NguoiDung_NguoiDungId",
                table: "GioHang");

            migrationBuilder.DropForeignKey(
                name: "FK_GioHang_SanPhams_SanPhamId",
                table: "GioHang");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GioHang",
                table: "GioHang");

            migrationBuilder.RenameTable(
                name: "GioHang",
                newName: "GioHangs");

            migrationBuilder.RenameIndex(
                name: "IX_GioHang_SanPhamId",
                table: "GioHangs",
                newName: "IX_GioHangs_SanPhamId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GioHangs",
                table: "GioHangs",
                columns: new[] { "NguoiDungId", "SanPhamId" });

            migrationBuilder.AddForeignKey(
                name: "FK_GioHangs_NguoiDung_NguoiDungId",
                table: "GioHangs",
                column: "NguoiDungId",
                principalTable: "NguoiDung",
                principalColumn: "NguoiDungId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GioHangs_SanPhams_SanPhamId",
                table: "GioHangs",
                column: "SanPhamId",
                principalTable: "SanPhams",
                principalColumn: "SanPhamId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
