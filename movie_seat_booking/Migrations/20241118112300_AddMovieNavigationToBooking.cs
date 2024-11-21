using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace movie_seat_booking.Migrations
{
    public partial class AddMovieNavigationToBooking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Bookings_MovieId",
                table: "Bookings",
                column: "MovieId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Movies_MovieId",
                table: "Bookings",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "MovieId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Movies_MovieId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_MovieId",
                table: "Bookings");
        }
    }
}
