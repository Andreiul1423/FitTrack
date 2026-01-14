using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitTrack.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkoutDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Workouts",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Workouts");
        }
    }
}
