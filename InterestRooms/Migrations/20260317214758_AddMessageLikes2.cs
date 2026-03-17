using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterestRooms.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageLikes2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageLike_Messages_MessageId",
                table: "MessageLike");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageLike_Users_UserId",
                table: "MessageLike");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MessageLike",
                table: "MessageLike");

            migrationBuilder.RenameTable(
                name: "MessageLike",
                newName: "MessageLikes");

            migrationBuilder.RenameIndex(
                name: "IX_MessageLike_UserId",
                table: "MessageLikes",
                newName: "IX_MessageLikes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_MessageLike_MessageId_UserId",
                table: "MessageLikes",
                newName: "IX_MessageLikes_MessageId_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MessageLikes",
                table: "MessageLikes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageLikes_Messages_MessageId",
                table: "MessageLikes",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageLikes_Users_UserId",
                table: "MessageLikes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageLikes_Messages_MessageId",
                table: "MessageLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageLikes_Users_UserId",
                table: "MessageLikes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MessageLikes",
                table: "MessageLikes");

            migrationBuilder.RenameTable(
                name: "MessageLikes",
                newName: "MessageLike");

            migrationBuilder.RenameIndex(
                name: "IX_MessageLikes_UserId",
                table: "MessageLike",
                newName: "IX_MessageLike_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_MessageLikes_MessageId_UserId",
                table: "MessageLike",
                newName: "IX_MessageLike_MessageId_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MessageLike",
                table: "MessageLike",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageLike_Messages_MessageId",
                table: "MessageLike",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageLike_Users_UserId",
                table: "MessageLike",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
