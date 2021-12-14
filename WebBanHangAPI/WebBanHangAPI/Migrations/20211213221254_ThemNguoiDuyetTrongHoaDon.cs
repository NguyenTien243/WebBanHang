using Microsoft.EntityFrameworkCore.Migrations;

namespace WebBanHangAPI.Migrations
{
    public partial class ThemNguoiDuyetTrongHoaDon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NhanVienId",
                table: "HoaDons",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_NhanVienId",
                table: "HoaDons",
                column: "NhanVienId");

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDons_NguoiDungs_NhanVienId",
                table: "HoaDons",
                column: "NhanVienId",
                principalTable: "NguoiDungs",
                principalColumn: "NguoiDungId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HoaDons_NguoiDungs_NhanVienId",
                table: "HoaDons");

            migrationBuilder.DropIndex(
                name: "IX_HoaDons_NhanVienId",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "NhanVienId",
                table: "HoaDons");
        }
    }
}
