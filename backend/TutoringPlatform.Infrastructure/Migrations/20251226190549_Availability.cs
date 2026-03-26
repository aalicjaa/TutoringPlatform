using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutoringPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Availability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AvailabilitySlots_TutorProfiles_TutorProfileId",
                table: "AvailabilitySlots");

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
                name: "IX_Bookings_LessonOfferId",
                table: "Bookings");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_LessonOfferId_StartUtc_EndUtc",
                table: "Bookings",
                columns: new[] { "LessonOfferId", "StartUtc", "EndUtc" });

            migrationBuilder.AddForeignKey(
                name: "FK_AvailabilitySlots_TutorProfiles_TutorProfileId",
                table: "AvailabilitySlots",
                column: "TutorProfileId",
                principalTable: "TutorProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonOffers_TutorProfiles_TutorProfileId",
                table: "LessonOffers",
                column: "TutorProfileId",
                principalTable: "TutorProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Bookings_BookingId",
                table: "Payments",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TutorProfiles_AspNetUsers_UserId",
                table: "TutorProfiles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AvailabilitySlots_TutorProfiles_TutorProfileId",
                table: "AvailabilitySlots");

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
                name: "IX_Bookings_LessonOfferId_StartUtc_EndUtc",
                table: "Bookings");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_LessonOfferId",
                table: "Bookings",
                column: "LessonOfferId");

            migrationBuilder.AddForeignKey(
                name: "FK_AvailabilitySlots_TutorProfiles_TutorProfileId",
                table: "AvailabilitySlots",
                column: "TutorProfileId",
                principalTable: "TutorProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
    }
}
