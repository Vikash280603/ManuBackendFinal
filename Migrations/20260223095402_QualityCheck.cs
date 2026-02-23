using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManuBackend.Migrations
{
    /// <inheritdoc />
    public partial class QualityCheck : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QualityChecks",
                columns: table => new
                {
                    QcId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WorkOrderId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    InspectionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalQty = table.Column<int>(type: "int", nullable: false),
                    AcceptedQty = table.Column<int>(type: "int", nullable: false),
                    RejectedQty = table.Column<int>(type: "int", nullable: false),
                    SuccessRate = table.Column<int>(type: "int", nullable: false),
                    Result = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualityChecks", x => x.QcId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QualityChecks");
        }
    }
}
