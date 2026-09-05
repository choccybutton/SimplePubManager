using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimplePubManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Generate password hash dynamically for "TestPass123!" with work factor 10
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("TestPass123!", 10);

            // Insert Organization
            migrationBuilder.Sql(
                @"INSERT INTO ""Organizations"" (""Id"", ""Name"", ""Subdomain"", ""CreatedAt"")
                  VALUES ('00000000-0000-0000-0000-000000000001', 'TestPub', 'testpub', '2026-09-04T00:00:00Z');");

            // Insert Users (Manager, Supervisor, Staff) with dynamically generated hash
            migrationBuilder.Sql(
                $@"INSERT INTO ""Users"" (""Id"", ""OrganizationId"", ""Name"", ""Email"", ""PasswordHash"", ""Role"", ""Status"", ""CreatedAt"")
                  VALUES
                  ('00000000-0000-0000-0000-000000000010', '00000000-0000-0000-0000-000000000001', 'John Manager', 'manager@test.com', '{passwordHash}', 0, 0, '2026-09-04T00:00:00Z'),
                  ('00000000-0000-0000-0000-000000000011', '00000000-0000-0000-0000-000000000001', 'Jane Supervisor', 'supervisor@test.com', '{passwordHash}', 1, 0, '2026-09-04T00:00:00Z'),
                  ('00000000-0000-0000-0000-000000000012', '00000000-0000-0000-0000-000000000001', 'Bob Staff', 'staff@test.com', '{passwordHash}', 2, 0, '2026-09-04T00:00:00Z');");

            // Insert Areas (Kitchen, Bar, Dining)
            migrationBuilder.Sql(
                @"INSERT INTO ""Areas"" (""Id"", ""OrganizationId"", ""Name"", ""Description"", ""CreatedAt"")
                  VALUES
                  ('00000000-0000-0000-0000-000000000020', '00000000-0000-0000-0000-000000000001', 'Kitchen', 'Food preparation area', '2026-09-04T00:00:00Z'),
                  ('00000000-0000-0000-0000-000000000021', '00000000-0000-0000-0000-000000000001', 'Bar', 'Beverage service area', '2026-09-04T00:00:00Z'),
                  ('00000000-0000-0000-0000-000000000022', '00000000-0000-0000-0000-000000000001', 'Dining', 'Customer seating area', '2026-09-04T00:00:00Z');");

            // Insert Shifts (Today and Tomorrow for Staff member)
            migrationBuilder.Sql(
                @"INSERT INTO ""Shifts"" (""Id"", ""OrganizationId"", ""StaffId"", ""Type"", ""StartTime"", ""EndTime"", ""Status"", ""CreatedBy"", ""CreatedAt"")
                  VALUES
                  ('00000000-0000-0000-0000-000000000030', '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000012', 0, '2026-09-04T09:00:00Z', '2026-09-04T17:00:00Z', 0, '00000000-0000-0000-0000-000000000010', '2026-09-04T00:00:00Z'),
                  ('00000000-0000-0000-0000-000000000031', '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000012', 0, '2026-09-05T14:00:00Z', '2026-09-05T22:00:00Z', 0, '00000000-0000-0000-0000-000000000010', '2026-09-04T00:00:00Z');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Delete Shifts first (no dependencies)
            migrationBuilder.Sql(
                @"DELETE FROM ""Shifts"" WHERE ""Id"" IN ('00000000-0000-0000-0000-000000000030', '00000000-0000-0000-0000-000000000031');");

            // Delete Areas second (no dependencies after shifts are deleted)
            migrationBuilder.Sql(
                @"DELETE FROM ""Areas"" WHERE ""Id"" IN ('00000000-0000-0000-0000-000000000020', '00000000-0000-0000-0000-000000000021', '00000000-0000-0000-0000-000000000022');");

            // Delete Users third (no dependencies after shifts/areas are deleted)
            migrationBuilder.Sql(
                @"DELETE FROM ""Users"" WHERE ""Id"" IN ('00000000-0000-0000-0000-000000000010', '00000000-0000-0000-0000-000000000011', '00000000-0000-0000-0000-000000000012');");

            // Delete Organization last
            migrationBuilder.Sql(
                @"DELETE FROM ""Organizations"" WHERE ""Id"" = '00000000-0000-0000-0000-000000000001';");
        }
    }
}
