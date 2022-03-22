using Microsoft.EntityFrameworkCore.Migrations;

namespace App.Migrations
{
    public partial class Update_AppUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "homeAddress",
                table: "Users",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "homeAddress",
                table: "Users");
        }
    }
}
