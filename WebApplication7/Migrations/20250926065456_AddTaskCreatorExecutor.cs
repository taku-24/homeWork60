using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication7.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskCreatorExecutor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Executor",
                table: "ToDoTasks");

            migrationBuilder.AddColumn<int>(
                name: "CreatorId",
                table: "ToDoTasks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExecutorId",
                table: "ToDoTasks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ToDoTasks_CreatorId",
                table: "ToDoTasks",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_ToDoTasks_ExecutorId",
                table: "ToDoTasks",
                column: "ExecutorId");

            migrationBuilder.AddForeignKey(
                name: "FK_ToDoTasks_AspNetUsers_CreatorId",
                table: "ToDoTasks",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ToDoTasks_AspNetUsers_ExecutorId",
                table: "ToDoTasks",
                column: "ExecutorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToDoTasks_AspNetUsers_CreatorId",
                table: "ToDoTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_ToDoTasks_AspNetUsers_ExecutorId",
                table: "ToDoTasks");

            migrationBuilder.DropIndex(
                name: "IX_ToDoTasks_CreatorId",
                table: "ToDoTasks");

            migrationBuilder.DropIndex(
                name: "IX_ToDoTasks_ExecutorId",
                table: "ToDoTasks");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "ToDoTasks");

            migrationBuilder.DropColumn(
                name: "ExecutorId",
                table: "ToDoTasks");

            migrationBuilder.AddColumn<string>(
                name: "Executor",
                table: "ToDoTasks",
                type: "text",
                nullable: true);
        }
    }
}
