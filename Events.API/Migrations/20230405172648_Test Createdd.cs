using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Events.API.Migrations
{
  /// <inheritdoc />
  public partial class TestCreatedd : Migration
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
            Title = table.Column<string>(type: "TEXT", nullable: false),
            Description = table.Column<string>(type: "TEXT", nullable: false),
            StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
            EndDate = table.Column<DateTime>(type: "TEXT", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Events", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "Users",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "TEXT", nullable: false),
            FirstName = table.Column<string>(type: "TEXT", nullable: false),
            LastName = table.Column<string>(type: "TEXT", nullable: false),
            UserName = table.Column<string>(type: "TEXT", nullable: false),
            Email = table.Column<string>(type: "TEXT", nullable: true),
            // EventId = table.Column<Guid>(type: "TEXT", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Users", x => x.Id);
            // table.ForeignKey(
            //     name: "FK_Users_Events_EventId",
            //     column: x => x.EventId,
            //     principalTable: "Events",
            //     principalColumn: "Id");
          });

      migrationBuilder.CreateTable(
          name: "Invitations",
          columns: table => new
          {
            Id = table.Column<Guid>(type: "TEXT", nullable: false),
            EventId = table.Column<Guid>(type: "TEXT", nullable: false),
            EventOwnerId = table.Column<Guid>(type: "TEXT", nullable: false),
            InvitedId = table.Column<Guid>(type: "TEXT", nullable: false),
            InviteState = table.Column<int>(type: "INTEGER", nullable: false)
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
            table.ForeignKey(
                      name: "FK_Invitations_Users_EventOwnerId",
                      column: x => x.EventOwnerId,
                      principalTable: "Users",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Restrict);
            table.ForeignKey(
                      name: "FK_Invitations_Users_InvitedId",
                      column: x => x.InvitedId,
                      principalTable: "Users",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateIndex(
          name: "IX_Events_OwnerId",
          table: "Events",
          column: "OwnerId");

      migrationBuilder.CreateIndex(
          name: "IX_Invitations_EventId",
          table: "Invitations",
          column: "EventId");

      migrationBuilder.CreateIndex(
          name: "IX_Invitations_EventOwnerId",
          table: "Invitations",
          column: "EventOwnerId");

      migrationBuilder.CreateIndex(
          name: "IX_Invitations_InvitedId",
          table: "Invitations",
          column: "InvitedId");

      //   migrationBuilder.CreateIndex(
      //       name: "IX_Users_EventId",
      //       table: "Users",
      //       column: "EventId");

      migrationBuilder.AddForeignKey(
          name: "FK_Events_Users_OwnerId",
          table: "Events",
          column: "OwnerId",
          principalTable: "Users",
          principalColumn: "Id",
          onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropForeignKey(
          name: "FK_Events_Users_OwnerId",
          table: "Events");

      migrationBuilder.DropTable(
          name: "Invitations");

      migrationBuilder.DropTable(
          name: "Users");

      migrationBuilder.DropTable(
          name: "Events");
    }
  }
}
