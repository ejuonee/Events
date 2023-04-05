using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Events.API.Migrations
{
  /// <inheritdoc />
  public partial class InitialCreate : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "Events",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "TEXT", nullable: false),
            OwnerId = table.Column<Guid>(type: "TEXT", nullable: false),
            Title = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
            Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
            StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
            EndDate = table.Column<DateTime>(type: "TEXT", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Events", x => x.Id);
            table.ForeignKey(
                      name: "FK_Events_Users_ID",
                      column: x => x.OwnerId,
                      principalTable: "Users",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);

          });

      migrationBuilder.CreateTable(
          name: "Invitations",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "TEXT", nullable: false),
            EventId = table.Column<Guid>(type: "TEXT", nullable: false),
            InviterId = table.Column<Guid>(type: "TEXT", nullable: false),
            InviteeId = table.Column<Guid>(type: "TEXT", nullable: false),
            InviteState = table.Column<string>(type: "TEXT", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Invitations", x => x.Id);
            table.ForeignKey(
                      name: "FK_Invitations_Events_EventId",
                      column: x => x.EventId,
                      principalTable: "Events",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "Users",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "TEXT", nullable: false),
            FirstName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
            LastName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
            UserName = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
            Email = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Users", x => x.Id);
          });

      migrationBuilder.CreateIndex(
          name: "IX_Invitations_EventId",
          table: "Invitations",
          column: "EventId");

    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "Invitations");

      migrationBuilder.DropTable(
          name: "Users");

      migrationBuilder.DropTable(
          name: "Events");
    }
  }
}
