using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PasswordManagerApplication.Migrations
{
    /// <inheritdoc />
    public partial class ThirdMigrate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PasswordEntries_tb_Title",
                table: "PasswordEntries_tb");

            migrationBuilder.DropIndex(
                name: "IX_PasswordEntries_tb_UserId",
                table: "PasswordEntries_tb");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordEntries_tb_UserId_Title",
                table: "PasswordEntries_tb",
                columns: new[] { "UserId", "Title" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PasswordEntries_tb_UserId_Title",
                table: "PasswordEntries_tb");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordEntries_tb_Title",
                table: "PasswordEntries_tb",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PasswordEntries_tb_UserId",
                table: "PasswordEntries_tb",
                column: "UserId");
        }
    }
}
