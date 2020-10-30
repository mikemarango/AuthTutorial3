using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Api.Data.Migrations
{
  public partial class Initial : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "Images",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
            FileName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
            OwnerId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Images", x => x.Id);
          });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "Images");
    }
  }
}
