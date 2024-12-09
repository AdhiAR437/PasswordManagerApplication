using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PasswordManagerApplication.Migrations
{
    /// <inheritdoc />
    public partial class SecondCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PasswordEntries_tb_Title",
                table: "PasswordEntries_tb",
                column: "Title",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PasswordEntries_tb_Title",
                table: "PasswordEntries_tb");
        }
    }
}
