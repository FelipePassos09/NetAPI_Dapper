using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockCrud.Api.Migrations
{
    /// <inheritdoc />
    public partial class eleven : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "productid",
                table: "suppliers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "productid",
                table: "suppliers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
