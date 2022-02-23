using Microsoft.EntityFrameworkCore.Migrations;

namespace Scanner.Data.Migrations
{
    public partial class FileIsChecked : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Checked",
                table: "FileDatas",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Checked",
                table: "FileDatas");
        }
    }
}
