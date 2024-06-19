using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SongViewLibrary.Migrations
{
    /// <inheritdoc />
    public partial class Changeviewscounttolong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "ViewsCount",
                table: "SongViews",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ViewsCount",
                table: "SongViews",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
