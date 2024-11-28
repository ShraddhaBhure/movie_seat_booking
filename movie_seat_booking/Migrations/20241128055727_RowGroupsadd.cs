using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace movie_seat_booking.Migrations
{
    public partial class RowGroupsadd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RowGroupId",
                table: "Seat",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "RowGroups",
                columns: table => new
                {
                    RowGroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MovieId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RowGroups", x => x.RowGroupId);
                    table.ForeignKey(
                        name: "FK_RowGroups_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "MovieId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Seat_RowGroupId",
                table: "Seat",
                column: "RowGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_RowGroups_MovieId",
                table: "RowGroups",
                column: "MovieId");

            migrationBuilder.AddForeignKey(
                name: "FK_Seat_RowGroups_RowGroupId",
                table: "Seat",
                column: "RowGroupId",
                principalTable: "RowGroups",
                principalColumn: "RowGroupId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Seat_RowGroups_RowGroupId",
                table: "Seat");

            migrationBuilder.DropTable(
                name: "RowGroups");

            migrationBuilder.DropIndex(
                name: "IX_Seat_RowGroupId",
                table: "Seat");

            migrationBuilder.DropColumn(
                name: "RowGroupId",
                table: "Seat");
        }
    }
}
