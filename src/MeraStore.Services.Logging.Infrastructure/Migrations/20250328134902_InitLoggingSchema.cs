using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeraStore.Services.Logging.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitLoggingSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(26)", maxLength: 26, nullable: false),
                    HttpMethod = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Payload = table.Column<byte[]>(type: "VARBINARY(MAX)", nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CorrelationId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Responses",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(26)", maxLength: 26, nullable: false),
                    StatusCode = table.Column<int>(type: "int", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Payload = table.Column<byte[]>(type: "VARBINARY(MAX)", nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CorrelationId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Responses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Requests_CorrelationId",
                table: "Requests",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_HttpMethod_Timestamp",
                table: "Requests",
                columns: new[] { "HttpMethod", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_Requests_Timestamp",
                table: "Requests",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_Responses_CorrelationId",
                table: "Responses",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_Responses_RequestId_Timestamp",
                table: "Responses",
                columns: new[] { "RequestId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_Responses_StatusCode",
                table: "Responses",
                column: "StatusCode");

            migrationBuilder.CreateIndex(
                name: "IX_Responses_Timestamp",
                table: "Responses",
                column: "Timestamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Requests");

            migrationBuilder.DropTable(
                name: "Responses");
        }
    }
}
