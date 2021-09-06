using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Worker.Balance.Migrations
{
    public partial class AddKafkaOffsetsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KafkaOffsets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LastProcessedMessageId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KafkaOffsets", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "KafkaOffsets",
                columns: new[] { "Id", "LastProcessedMessageId" },
                values: new object[] { 1, -1L });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KafkaOffsets");
        }
    }
}
