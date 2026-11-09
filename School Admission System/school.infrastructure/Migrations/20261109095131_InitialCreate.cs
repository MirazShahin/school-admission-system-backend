using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace school.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "classes",
                columns: table => new
                {
                    class_id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    class_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    total_seats = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_classes", x => x.class_id);
                    table.CheckConstraint("classes_total_seats_ck", "total_seats >= 1");
                });

            migrationBuilder.CreateTable(
                name: "students",
                columns: table => new
                {
                    student_id = table.Column<string>(type: "character varying(17)", maxLength: 17, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_students", x => x.student_id);
                    table.CheckConstraint("students_student_id_ck", "student_id ~ '^[0-9]{17}$'");
                });

            migrationBuilder.CreateTable(
                name: "holds",
                columns: table => new
                {
                    hold_id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    class_id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    student_id = table.Column<string>(type: "character varying(17)", maxLength: 17, nullable: false),
                    status = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_holds", x => x.hold_id);
                    table.CheckConstraint("holds_status_ck", "status IN ('HELD','CONFIRMED','EXPIRED','CANCELLED')");
                    table.ForeignKey(
                        name: "FK_holds_classes_class_id",
                        column: x => x.class_id,
                        principalTable: "classes",
                        principalColumn: "class_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_holds_students_student_id",
                        column: x => x.student_id,
                        principalTable: "students",
                        principalColumn: "student_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "holds_class_status_idx",
                table: "holds",
                columns: new[] { "class_id", "status" });

            migrationBuilder.CreateIndex(
                name: "holds_one_live_per_student_class_uq",
                table: "holds",
                columns: new[] { "class_id", "student_id" },
                unique: true,
                filter: "status IN ('HELD','CONFIRMED')");

            migrationBuilder.CreateIndex(
                name: "holds_student_created_idx",
                table: "holds",
                columns: new[] { "student_id", "created_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "holds");

            migrationBuilder.DropTable(
                name: "classes");

            migrationBuilder.DropTable(
                name: "students");
        }
    }
}
