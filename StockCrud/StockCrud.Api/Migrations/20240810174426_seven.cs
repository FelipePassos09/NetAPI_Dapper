using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockCrud.Api.Migrations
{
    /// <inheritdoc />
    public partial class seven : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "telephone",
                table: "suppliers",
                type: "bigint",
                maxLength: 11,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldMaxLength: 11);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "telephone",
                table: "suppliers",
                type: "integer",
                maxLength: 11,
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldMaxLength: 11);
        }
    }
}
