using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutoringPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCoreRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "TutorProfiles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Subjects",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Payments",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Provider",
                table: "Payments",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                table: "Payments",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Payments",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "LessonOffers",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Mode",
                table: "LessonOffers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Bookings",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_TutorProfiles_UserId",
                table: "TutorProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_Name",
                table: "Subjects",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BookingId",
                table: "Payments",
                column: "BookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LessonOffers_SubjectId",
                table: "LessonOffers",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonOffers_TutorProfileId_SubjectId",
                table: "LessonOffers",
                columns: new[] { "TutorProfileId", "SubjectId" });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_LessonOfferId",
                table: "Bookings",
                column: "LessonOfferId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_StudentUserId_StartUtc",
                table: "Bookings",
                columns: new[] { "StudentUserId", "StartUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_AvailabilitySlots_TutorProfileId_StartUtc_EndUtc",
                table: "AvailabilitySlots",
                columns: new[] { "TutorProfileId", "StartUtc", "EndUtc" });

            migrationBuilder.AddForeignKey(
                name: "FK_AvailabilitySlots_TutorProfiles_TutorProfileId",
                table: "AvailabilitySlots",
                column: "TutorProfileId",
                principalTable: "TutorProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_AspNetUsers_StudentUserId",
                table: "Bookings",
                column: "StudentUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_LessonOffers_LessonOfferId",
                table: "Bookings",
                column: "LessonOfferId",
                principalTable: "LessonOffers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonOffers_Subjects_SubjectId",
                table: "LessonOffers",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonOffers_TutorProfiles_TutorProfileId",
                table: "LessonOffers",
                column: "TutorProfileId",
                principalTable: "TutorProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Bookings_BookingId",
                table: "Payments",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TutorProfiles_AspNetUsers_UserId",
                table: "TutorProfiles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AvailabilitySlots_TutorProfiles_TutorProfileId",
                table: "AvailabilitySlots");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_AspNetUsers_StudentUserId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_LessonOffers_LessonOfferId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonOffers_Subjects_SubjectId",
                table: "LessonOffers");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonOffers_TutorProfiles_TutorProfileId",
                table: "LessonOffers");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Bookings_BookingId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_TutorProfiles_AspNetUsers_UserId",
                table: "TutorProfiles");

            migrationBuilder.DropIndex(
                name: "IX_TutorProfiles_UserId",
                table: "TutorProfiles");

            migrationBuilder.DropIndex(
                name: "IX_Subjects_Name",
                table: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_Payments_BookingId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_LessonOffers_SubjectId",
                table: "LessonOffers");

            migrationBuilder.DropIndex(
                name: "IX_LessonOffers_TutorProfileId_SubjectId",
                table: "LessonOffers");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_LessonOfferId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_StudentUserId_StartUtc",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_AvailabilitySlots_TutorProfileId_StartUtc_EndUtc",
                table: "AvailabilitySlots");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TutorProfiles");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Subjects",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "Provider",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(3)",
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Payments",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "LessonOffers",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Mode",
                table: "LessonOffers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);
        }
    }
}
