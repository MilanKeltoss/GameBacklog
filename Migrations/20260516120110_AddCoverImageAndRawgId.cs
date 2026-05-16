using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameBacklog.Migrations
{
    /// <inheritdoc />
    public partial class AddCoverImageAndRawgId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverImageUrl",
                table: "Games",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RawgId",
                table: "Games",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverImageUrl",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "RawgId",
                table: "Games");
        }
    }
}
