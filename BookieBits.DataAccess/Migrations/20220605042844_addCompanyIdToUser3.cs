using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookieBits.DataAccess.Migrations
{
    public partial class addCompanyIdToUser3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Companies_CompanyObjID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Company",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "CompanyObjID",
                table: "AspNetUsers",
                newName: "CompanyID");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_CompanyObjID",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_CompanyID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Companies_CompanyID",
                table: "AspNetUsers",
                column: "CompanyID",
                principalTable: "Companies",
                principalColumn: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Companies_CompanyID",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "CompanyID",
                table: "AspNetUsers",
                newName: "CompanyObjID");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_CompanyID",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_CompanyObjID");

            migrationBuilder.AddColumn<int>(
                name: "Company",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Companies_CompanyObjID",
                table: "AspNetUsers",
                column: "CompanyObjID",
                principalTable: "Companies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
