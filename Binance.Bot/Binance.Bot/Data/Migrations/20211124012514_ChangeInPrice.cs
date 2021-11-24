using Microsoft.EntityFrameworkCore.Migrations;

namespace Binance.Bot.Data.Migrations
{
    public partial class ChangeInPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ChangeInPrice",
                table: "Bots",
                newName: "ChangeInPriceUp");

            migrationBuilder.AddColumn<decimal>(
                name: "ChangeInPriceDown",
                table: "Bots",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangeInPriceDown",
                table: "Bots");

            migrationBuilder.RenameColumn(
                name: "ChangeInPriceUp",
                table: "Bots",
                newName: "ChangeInPrice");
        }
    }
}
