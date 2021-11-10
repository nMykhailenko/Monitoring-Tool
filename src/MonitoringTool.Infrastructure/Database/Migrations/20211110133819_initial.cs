using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MonitoringTool.Infrastructure.Database.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConnectedClients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConnectedClients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConnectedService",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    BaseUrl = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ConnectedClientId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConnectedService", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConnectedService_ConnectedClients_ConnectedClientId",
                        column: x => x.ConnectedClientId,
                        principalTable: "ConnectedClients",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConnectedService_ConnectedClientId",
                table: "ConnectedService",
                column: "ConnectedClientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConnectedService");

            migrationBuilder.DropTable(
                name: "ConnectedClients");
        }
    }
}
