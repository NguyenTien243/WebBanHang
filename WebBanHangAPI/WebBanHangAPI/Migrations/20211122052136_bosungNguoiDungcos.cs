using Microsoft.EntityFrameworkCore.Migrations;

namespace WebBanHangAPI.Migrations
{
    public partial class bosungNguoiDungcos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GioHang_NguoiDung_NguoiDungId",
                table: "GioHang");

            migrationBuilder.DropForeignKey(
                name: "FK_GioHang_SanPhams_SanPhamId",
                table: "GioHang");

            migrationBuilder.DropForeignKey(
                name: "FK_HoaDons_NguoiDung_NguoiDungId",
                table: "HoaDons");

            migrationBuilder.DropForeignKey(
                name: "FK_NguoiDung_VaiTros_VaiTroId",
                table: "NguoiDung");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NguoiDung",
                table: "NguoiDung");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GioHang",
                table: "GioHang");

            migrationBuilder.RenameTable(
                name: "NguoiDung",
                newName: "NguoiDungs");

            migrationBuilder.RenameTable(
                name: "GioHang",
                newName: "GioHangs");

            migrationBuilder.RenameIndex(
                name: "IX_NguoiDung_VaiTroId",
                table: "NguoiDungs",
                newName: "IX_NguoiDungs_VaiTroId");

            migrationBuilder.RenameIndex(
                name: "IX_GioHang_SanPhamId",
                table: "GioHangs",
                newName: "IX_GioHangs_SanPhamId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NguoiDungs",
                table: "NguoiDungs",
                column: "NguoiDungId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GioHangs",
                table: "GioHangs",
                columns: new[] { "NguoiDungId", "SanPhamId" });

            migrationBuilder.AddForeignKey(
                name: "FK_GioHangs_NguoiDungs_NguoiDungId",
                table: "GioHangs",
                column: "NguoiDungId",
                principalTable: "NguoiDungs",
                principalColumn: "NguoiDungId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GioHangs_SanPhams_SanPhamId",
                table: "GioHangs",
                column: "SanPhamId",
                principalTable: "SanPhams",
                principalColumn: "SanPhamId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDons_NguoiDungs_NguoiDungId",
                table: "HoaDons",
                column: "NguoiDungId",
                principalTable: "NguoiDungs",
                principalColumn: "NguoiDungId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NguoiDungs_VaiTros_VaiTroId",
                table: "NguoiDungs",
                column: "VaiTroId",
                principalTable: "VaiTros",
                principalColumn: "VaiTroId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GioHangs_NguoiDungs_NguoiDungId",
                table: "GioHangs");

            migrationBuilder.DropForeignKey(
                name: "FK_GioHangs_SanPhams_SanPhamId",
                table: "GioHangs");

            migrationBuilder.DropForeignKey(
                name: "FK_HoaDons_NguoiDungs_NguoiDungId",
                table: "HoaDons");

            migrationBuilder.DropForeignKey(
                name: "FK_NguoiDungs_VaiTros_VaiTroId",
                table: "NguoiDungs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NguoiDungs",
                table: "NguoiDungs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GioHangs",
                table: "GioHangs");

            migrationBuilder.RenameTable(
                name: "NguoiDungs",
                newName: "NguoiDung");

            migrationBuilder.RenameTable(
                name: "GioHangs",
                newName: "GioHang");

            migrationBuilder.RenameIndex(
                name: "IX_NguoiDungs_VaiTroId",
                table: "NguoiDung",
                newName: "IX_NguoiDung_VaiTroId");

            migrationBuilder.RenameIndex(
                name: "IX_GioHangs_SanPhamId",
                table: "GioHang",
                newName: "IX_GioHang_SanPhamId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NguoiDung",
                table: "NguoiDung",
                column: "NguoiDungId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDons_NguoiDung_NguoiDungId",
                table: "HoaDons",
                column: "NguoiDungId",
                principalTable: "NguoiDung",
                principalColumn: "NguoiDungId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NguoiDung_VaiTros_VaiTroId",
                table: "NguoiDung",
                column: "VaiTroId",
                principalTable: "VaiTros",
                principalColumn: "VaiTroId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
