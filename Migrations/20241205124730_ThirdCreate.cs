using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PasswordManagerApplication.Migrations
{
    /// <inheritdoc />
    public partial class ThirdCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "PasswordEntries_tb",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_PasswordEntries_tb_UserId_Website_Username",
                table: "PasswordEntries_tb",
                columns: new[] { "UserId", "Website", "Username" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PasswordEntries_tb_UserId_Website_Username",
                table: "PasswordEntries_tb");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "PasswordEntries_tb",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
