using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ManuBackend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "Name", "PasswordHash", "Role" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "product@company.com", "Product Manager", "$2a$11$dh4LkrvYdzPTtSFSAqqAf.RcWMkx5.lgSJfiG6k2VerH1SJpKIlae", "product_bom_manager" },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "inventory@company.com", "Inventory Manager", "$2a$11$ij4US8J/zT/.4uWeXUMlKuDJeGVxE6r7Dp9dGdcengdbVsN.TLWH2", "inventory_manager" },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "quality@company.com", "Quality Control", "$2a$11$lVL0DGUiFjDkUj.0hMrkgeW4JJT7qHzRUOH9Rx1DEasWWZ6jiXedO", "qc_manager" },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "scheduler@company.com", "Production Scheduler", "$2a$11$2i9gK8y3rfdEHto0QxL2yOoKLHAHe2QIzGp6aMairHIK82zDsvIcK", "production_scheduler" },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "quality1@company.com", "Quality Control 2", "$2a$11$aJ/tAXik2ZtoGxmHtRktwe4ICUOfkwKwy.M.ZEx565qvHOh1.C/ve", "qc_manager" },
                    { 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@company.com", "Admin", "$2a$11$pN8p4VghaXXrrc.Ym3zAPOBq4IwmrwL.PDPDSfGaF04esozxrTcEa", "admin" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
