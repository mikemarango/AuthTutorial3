using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Auth.Migrations.Identity
{
  public partial class IdentityMigration : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "Users",
          columns: table => new
          {
            Id = table.Column<Guid>(nullable: false),
            Subject = table.Column<string>(maxLength: 200, nullable: false),
            UserName = table.Column<string>(maxLength: 200, nullable: true),
            Password = table.Column<string>(maxLength: 200, nullable: true),
            IsActive = table.Column<bool>(nullable: false),
            ConcurrencyStamp = table.Column<string>(nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Users", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "UserClaims",
          columns: table => new
          {
            Id = table.Column<Guid>(nullable: false),
            Type = table.Column<string>(maxLength: 250, nullable: false),
            Value = table.Column<string>(maxLength: 250, nullable: false),
            ConcurrencyStamp = table.Column<string>(nullable: true),
            UserId = table.Column<Guid>(nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_UserClaims", x => x.Id);
            table.ForeignKey(
                      name: "FK_UserClaims_Users_UserId",
                      column: x => x.UserId,
                      principalTable: "Users",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.InsertData(
          table: "Users",
          columns: new[] { "Id", "ConcurrencyStamp", "IsActive", "Password", "Subject", "UserName" },
          values: new object[] { new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "0192b6e7-8546-4139-a40d-c432fecfccba", true, "password", "d860efca-22d9-47fd-8249-791ba61b07c7", "Frank" });

      migrationBuilder.InsertData(
          table: "Users",
          columns: new[] { "Id", "ConcurrencyStamp", "IsActive", "Password", "Subject", "UserName" },
          values: new object[] { new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"), "ba79a534-6279-423d-8b38-cad7c98b1a05", true, "password", "b7539694-97e7-4dfe-84da-b4256e1ff5c7", "Claire" });

      migrationBuilder.InsertData(
          table: "UserClaims",
          columns: new[] { "Id", "ConcurrencyStamp", "Type", "UserId", "Value" },
          values: new object[,]
          {
                    { new Guid("798e8dd0-191f-4bb4-a1d9-52f413230e9d"), "f570279d-7aea-4fde-ba4d-965caab10fba", "given_name", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "Frank" },
                    { new Guid("120c0830-2f68-4265-ab50-8d7658ec80c4"), "5a93a52f-30e9-42dc-89cc-117fcd09cff9", "family_name", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "Underwood" },
                    { new Guid("ffcebc5b-cbce-4be2-b823-fe5ea5d53832"), "8a5f2ad9-4964-408b-a696-78c046031554", "email", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "frank@email.com" },
                    { new Guid("b373fa50-67bd-426c-8522-5264fd0d00be"), "54cb4ace-657b-4710-89c4-ea2372479f5d", "address", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "Main Road 1" },
                    { new Guid("7f0b65b8-62d7-4fdc-8531-1b7f5099e870"), "3b50482a-2990-4e0d-8730-8c4c1bc3ce34", "subscriptionlevel", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "FreeUser" },
                    { new Guid("c78998da-03e9-4dac-8b3e-af9868f66a40"), "373f1362-7b1d-4f43-94db-6fa6f2d00bf1", "country", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "nl" },
                    { new Guid("ae817744-cc0a-4370-8027-3d104a3ccef0"), "863253fb-40f0-42d6-a143-288de5d96895", "email", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "claire@email.com" },
                    { new Guid("c2faa3b9-d352-451c-ac10-84d22b82bd3b"), "7a72cf90-d472-4d20-9d0b-c8e5df109512", "given_name", new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"), "Claire" },
                    { new Guid("9cc43d6e-2b23-4618-b8c5-cb7d152a7923"), "3b3ec571-5d8c-4345-88b5-9dce5e046b34", "family_name", new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"), "Underwood" },
                    { new Guid("a0940dcf-aa3d-46f4-9e8e-b06db1af0929"), "ba3de0d9-3a72-4796-b9d2-ece8b97d89ab", "address", new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"), "Big Street 2" },
                    { new Guid("d4948a87-2820-4218-89b5-4196c658470f"), "8f2bae8a-6f7f-4d30-b93f-75d2ac470b3a", "subscriptionlevel", new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"), "PayingUser" },
                    { new Guid("8a5b121e-d745-4687-82b0-665ad5d24994"), "61ee05ff-9d22-40b5-8a55-8a124c3ca941", "country", new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"), "be" }
          });

      migrationBuilder.CreateIndex(
          name: "IX_UserClaims_UserId",
          table: "UserClaims",
          column: "UserId");

      migrationBuilder.CreateIndex(
          name: "IX_Users_Subject",
          table: "Users",
          column: "Subject",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_Users_UserName",
          table: "Users",
          column: "UserName",
          unique: true,
          filter: "[UserName] IS NOT NULL");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "UserClaims");

      migrationBuilder.DropTable(
          name: "Users");
    }
  }
}
