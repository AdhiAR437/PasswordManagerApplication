using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PasswordManagerApplication.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users_tb",
                columns: table => new
                {
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SecurityQuestion = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    SecurityQAns = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users_tb", x => x.Email);
                });

            migrationBuilder.CreateTable(
                name: "PasswordEntries_tb",
                columns: table => new
                {
                    PasswordEntryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Website = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordEntries_tb", x => x.PasswordEntryId);
                    table.ForeignKey(
                        name: "FK_PasswordEntries_tb_Users_tb_UserId",
                        column: x => x.UserId,
                        principalTable: "Users_tb",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PasswordEntries_tb_UserId",
                table: "PasswordEntries_tb",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_tb_Username",
                table: "Users_tb",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PasswordEntries_tb");

            migrationBuilder.DropTable(
                name: "Users_tb");
        }
    }
}
