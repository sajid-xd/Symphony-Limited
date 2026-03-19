using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SymphonyLimited.Migrations
{
    /// <inheritdoc />
    public partial class AddHallTicketToEnrollment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "ExamEnrollments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "HallTicketNumber",
                table: "ExamEnrollments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ExamEnrollments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ExamEnrollments_CourseId",
                table: "ExamEnrollments",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamEnrollments_Courses_CourseId",
                table: "ExamEnrollments",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamEnrollments_Courses_CourseId",
                table: "ExamEnrollments");

            migrationBuilder.DropIndex(
                name: "IX_ExamEnrollments_CourseId",
                table: "ExamEnrollments");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "ExamEnrollments");

            migrationBuilder.DropColumn(
                name: "HallTicketNumber",
                table: "ExamEnrollments");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ExamEnrollments");
        }
    }
}
