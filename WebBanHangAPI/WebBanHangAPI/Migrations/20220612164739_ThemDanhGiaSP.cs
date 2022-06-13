using Microsoft.EntityFrameworkCore.Migrations;

namespace WebBanHangAPI.Migrations
{
    public partial class ThemDanhGiaSP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BinhLuanDanhGia",
                table: "ChiTietHDs",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DanhGiaDuocDuyet",
                table: "ChiTietHDs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SoSao",
                table: "ChiTietHDs",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BinhLuanDanhGia",
                table: "ChiTietHDs");

            migrationBuilder.DropColumn(
                name: "DanhGiaDuocDuyet",
                table: "ChiTietHDs");

            migrationBuilder.DropColumn(
                name: "SoSao",
                table: "ChiTietHDs");
        }
    }
}
