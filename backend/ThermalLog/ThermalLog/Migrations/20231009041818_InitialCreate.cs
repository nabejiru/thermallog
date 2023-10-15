using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThermalLog.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "thermals",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    measured_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    host_name = table.Column<string>(type: "nvarchar(128)", nullable: true),
                    temperature = table.Column<double>(type: "float", nullable: true),
                    humidity = table.Column<double>(type: "float", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_thermals", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "thermals");
        }
    }
}
