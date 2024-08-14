using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockCrud.Api.Migrations
{
    /// <inheritdoc />
    public partial class fifthy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "uf",
                table: "suppliers",
                type: "character varying(2)",
                maxLength: 2,
                nullable: false,
                oldClrType: typeof(char),
                oldType: "character(1)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<char>(
                name: "uf",
                table: "suppliers",
                type: "character(1)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2)",
                oldMaxLength: 2);
        }
    }
}
