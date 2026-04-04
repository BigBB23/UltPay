using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UltPay.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBeneficiaryForMobileMoney : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MobileMoneyProvider",
                table: "Beneficiaries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MobileMoneyProvider",
                table: "Beneficiaries");
        }
    }
}
