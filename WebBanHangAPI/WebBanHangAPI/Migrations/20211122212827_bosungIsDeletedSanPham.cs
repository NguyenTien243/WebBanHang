using Microsoft.EntityFrameworkCore.Migrations;

namespace WebBanHangAPI.Migrations
{
    public partial class bosungIsDeletedSanPham : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isDeleted",
                table: "SanPhams",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "SanPhams");
        }
    }
}
