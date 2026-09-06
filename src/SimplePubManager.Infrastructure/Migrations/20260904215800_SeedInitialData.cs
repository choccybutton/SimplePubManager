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

            // Insert more staff members for testing
            migrationBuilder.Sql(
                $@"INSERT INTO ""Users"" (""Id"", ""OrganizationId"", ""Name"", ""Email"", ""PasswordHash"", ""Role"", ""Status"", ""CreatedAt"")
                  VALUES
                  ('00000000-0000-0000-0000-000000000013', '00000000-0000-0000-0000-000000000001', 'Alice Staff', 'alice@test.com', '{passwordHash}', 2, 0, '2026-09-04T00:00:00Z'),
                  ('00000000-0000-0000-0000-000000000014', '00000000-0000-0000-0000-000000000001', 'Charlie Staff', 'charlie@test.com', '{passwordHash}', 2, 0, '2026-09-04T00:00:00Z');");

            // Insert Shifts - Various statuses for complete testing
            // Status: 0=Scheduled, 1=InProgress, 2=Completed
            migrationBuilder.Sql(
                @"INSERT INTO ""Shifts"" (""Id"", ""OrganizationId"", ""StaffId"", ""Type"", ""StartTime"", ""EndTime"", ""Status"", ""CreatedBy"", ""CreatedAt"")
                  VALUES
                  -- Scheduled shifts (future)
                  ('00000000-0000-0000-0000-000000000030', '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000012', 0, '2026-09-06T09:00:00Z', '2026-09-06T17:00:00Z', 0, '00000000-0000-0000-0000-000000000010', '2026-09-04T00:00:00Z'),
                  ('00000000-0000-0000-0000-000000000031', '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000013', 0, '2026-09-06T17:00:00Z', '2026-09-07T01:00:00Z', 0, '00000000-0000-0000-0000-000000000010', '2026-09-04T00:00:00Z'),
                  ('00000000-0000-0000-0000-000000000032', '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000014', 0, '2026-09-07T09:00:00Z', '2026-09-07T17:00:00Z', 0, '00000000-0000-0000-0000-000000000010', '2026-09-04T00:00:00Z'),

                  -- In Progress shifts (current/recent)
                  ('00000000-0000-0000-0000-000000000033', '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000012', 0, '2026-09-05T14:00:00Z', '2026-09-05T22:00:00Z', 1, '00000000-0000-0000-0000-000000000010', '2026-09-04T00:00:00Z'),
                  ('00000000-0000-0000-0000-000000000034', '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000013', 0, '2026-09-05T09:00:00Z', '2026-09-05T17:00:00Z', 1, '00000000-0000-0000-0000-000000000010', '2026-09-04T00:00:00Z'),

                  -- Completed shifts (past)
                  ('00000000-0000-0000-0000-000000000035', '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000014', 0, '2026-09-03T09:00:00Z', '2026-09-03T17:00:00Z', 2, '00000000-0000-0000-0000-000000000010', '2026-09-03T00:00:00Z'),
                  ('00000000-0000-0000-0000-000000000036', '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000012', 0, '2026-09-02T17:00:00Z', '2026-09-03T01:00:00Z', 2, '00000000-0000-0000-0000-000000000010', '2026-09-02T00:00:00Z');");

            // Insert ShiftArea relationships (assign areas to shifts)
            migrationBuilder.Sql(
                @"INSERT INTO ""ShiftAreas"" (""ShiftId"", ""AreaId"")
                  VALUES
                  -- Shift 30 (Bob, Kitchen + Bar)
                  ('00000000-0000-0000-0000-000000000030', '00000000-0000-0000-0000-000000000020'),
                  ('00000000-0000-0000-0000-000000000030', '00000000-0000-0000-0000-000000000021'),
                  -- Shift 31 (Alice, Bar)
                  ('00000000-0000-0000-0000-000000000031', '00000000-0000-0000-0000-000000000021'),
                  -- Shift 32 (Charlie, Kitchen)
                  ('00000000-0000-0000-0000-000000000032', '00000000-0000-0000-0000-000000000020'),
                  -- Shift 33 (Bob, Dining)
                  ('00000000-0000-0000-0000-000000000033', '00000000-0000-0000-0000-000000000022'),
                  -- Shift 34 (Alice, Kitchen + Dining)
                  ('00000000-0000-0000-0000-000000000034', '00000000-0000-0000-0000-000000000020'),
                  ('00000000-0000-0000-0000-000000000034', '00000000-0000-0000-0000-000000000022'),
                  -- Shift 35 (Charlie, Bar)
                  ('00000000-0000-0000-0000-000000000035', '00000000-0000-0000-0000-000000000021'),
                  -- Shift 36 (Bob, Kitchen)
                  ('00000000-0000-0000-0000-000000000036', '00000000-0000-0000-0000-000000000020');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Delete ShiftAreas first (foreign key to Shifts)
            migrationBuilder.Sql(
                @"DELETE FROM ""ShiftAreas"" WHERE ""ShiftId"" IN ('00000000-0000-0000-0000-000000000030', '00000000-0000-0000-0000-000000000031', '00000000-0000-0000-0000-000000000032', '00000000-0000-0000-0000-000000000033', '00000000-0000-0000-0000-000000000034', '00000000-0000-0000-0000-000000000035', '00000000-0000-0000-0000-000000000036');");

            // Delete Shifts second (depends on ShiftAreas being deleted)
            migrationBuilder.Sql(
                @"DELETE FROM ""Shifts"" WHERE ""Id"" IN ('00000000-0000-0000-0000-000000000030', '00000000-0000-0000-0000-000000000031', '00000000-0000-0000-0000-000000000032', '00000000-0000-0000-0000-000000000033', '00000000-0000-0000-0000-000000000034', '00000000-0000-0000-0000-000000000035', '00000000-0000-0000-0000-000000000036');");

            // Delete Areas third (no dependencies after shifts are deleted)
            migrationBuilder.Sql(
                @"DELETE FROM ""Areas"" WHERE ""Id"" IN ('00000000-0000-0000-0000-000000000020', '00000000-0000-0000-0000-000000000021', '00000000-0000-0000-0000-000000000022');");

            // Delete Users fourth (no dependencies after shifts/areas are deleted)
            migrationBuilder.Sql(
                @"DELETE FROM ""Users"" WHERE ""Id"" IN ('00000000-0000-0000-0000-000000000010', '00000000-0000-0000-0000-000000000011', '00000000-0000-0000-0000-000000000012', '00000000-0000-0000-0000-000000000013', '00000000-0000-0000-0000-000000000014');");

            // Delete Organization last
            migrationBuilder.Sql(
                @"DELETE FROM ""Organizations"" WHERE ""Id"" = '00000000-0000-0000-0000-000000000001';");
        }
    }
}
