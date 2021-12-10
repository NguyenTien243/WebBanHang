using Microsoft.EntityFrameworkCore.Migrations;

namespace WebBanHangAPI.Migrations
{
    public partial class CapNhatChiTietHD : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "soLuong",
                table: "ChiTietHDs");

            migrationBuilder.AddColumn<string>(
                name: "sdtNguoiNhan",
                table: "HoaDons",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "tongTien",
                table: "ChiTietHDs",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "hinhAnh",
                table: "ChiTietHDs",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "soLuongDat",
                table: "ChiTietHDs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "tenSP",
                table: "ChiTietHDs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sdtNguoiNhan",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "hinhAnh",
                table: "ChiTietHDs");

            migrationBuilder.DropColumn(
                name: "soLuongDat",
                table: "ChiTietHDs");

            migrationBuilder.DropColumn(
                name: "tenSP",
                table: "ChiTietHDs");

            migrationBuilder.AlterColumn<int>(
                name: "tongTien",
                table: "ChiTietHDs",
                type: "int",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AddColumn<int>(
                name: "soLuong",
                table: "ChiTietHDs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
