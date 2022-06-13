using Microsoft.EntityFrameworkCore.Migrations;

namespace WebBanHangAPI.Migrations
{
    public partial class ThemDanhGiaSP2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DanhGiaDuocDuyet",
                table: "ChiTietHDs");

            migrationBuilder.AddColumn<int>(
                name: "TrangThaiDanhGia",
                table: "ChiTietHDs",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrangThaiDanhGia",
                table: "ChiTietHDs");

            migrationBuilder.AddColumn<bool>(
                name: "DanhGiaDuocDuyet",
                table: "ChiTietHDs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
