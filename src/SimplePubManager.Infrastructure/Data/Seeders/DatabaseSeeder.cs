using SimplePubManager.Domain.Entities;
using SimplePubManager.Domain.Enums;
using SimplePubManager.Infrastructure.Services;

namespace SimplePubManager.Infrastructure.Data.Seeders
{
    /// <summary>
    /// Utility class for generating test data for database seeding.
    /// </summary>
    public static class DatabaseSeeder
    {
        /// <summary>
        /// Creates a test organization with predefined values.
        /// </summary>
        /// <returns>An Organization entity with test data</returns>
        public static Organization CreateTestOrganization()
        {
            return new Organization
            {
                Id = new Guid("00000000-0000-0000-0000-000000000001"),
                Name = "Test Pub",
                Subdomain = "testpub",
                CreatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Creates a list of test users with predefined values and hashed passwords.
        /// </summary>
        /// <param name="authService">The authentication service used to hash passwords</param>
        /// <returns>A list of User entities with test data</returns>
        public static List<User> CreateTestUsers(AuthService authService)
        {
            if (authService == null)
            {
                throw new ArgumentNullException(nameof(authService));
            }

            var testPassword = "TestPass123!";
            var passwordHash = authService.HashPassword(testPassword);

            return new List<User>
            {
                new User
                {
                    Id = new Guid("00000000-0000-0000-0000-000000000010"),
                    OrganizationId = new Guid("00000000-0000-0000-0000-000000000001"),
                    Name = "Manager",
                    Email = "manager@test.com",
                    PasswordHash = passwordHash,
                    Role = UserRole.Manager,
                    Status = UserStatus.Active,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = new Guid("00000000-0000-0000-0000-000000000011"),
                    OrganizationId = new Guid("00000000-0000-0000-0000-000000000001"),
                    Name = "Supervisor",
                    Email = "supervisor@test.com",
                    PasswordHash = passwordHash,
                    Role = UserRole.Supervisor,
                    Status = UserStatus.Active,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = new Guid("00000000-0000-0000-0000-000000000012"),
                    OrganizationId = new Guid("00000000-0000-0000-0000-000000000001"),
                    Name = "Staff",
                    Email = "staff@test.com",
                    PasswordHash = passwordHash,
                    Role = UserRole.Staff,
                    Status = UserStatus.Active,
                    CreatedAt = DateTime.UtcNow
                }
            };
        }

        /// <summary>
        /// Creates a list of test areas with predefined values.
        /// </summary>
        /// <returns>A list of Area entities with test data</returns>
        public static List<Area> CreateTestAreas()
        {
            return new List<Area>
            {
                new Area
                {
                    Id = new Guid("00000000-0000-0000-0000-000000000020"),
                    OrganizationId = new Guid("00000000-0000-0000-0000-000000000001"),
                    Name = "Kitchen",
                    Description = "Food preparation area",
                    CreatedAt = DateTime.UtcNow
                },
                new Area
                {
                    Id = new Guid("00000000-0000-0000-0000-000000000021"),
                    OrganizationId = new Guid("00000000-0000-0000-0000-000000000001"),
                    Name = "Bar",
                    Description = "Beverage service area",
                    CreatedAt = DateTime.UtcNow
                },
                new Area
                {
                    Id = new Guid("00000000-0000-0000-0000-000000000022"),
                    OrganizationId = new Guid("00000000-0000-0000-0000-000000000001"),
                    Name = "Dining",
                    Description = "Customer seating area",
                    CreatedAt = DateTime.UtcNow
                }
            };
        }

        /// <summary>
        /// Creates a list of test shifts with predefined values.
        /// </summary>
        /// <param name="users">The list of users to assign to shifts</param>
        /// <param name="areas">The list of areas (not directly used in shifts but available for reference)</param>
        /// <returns>A list of Shift entities with test data</returns>
        public static List<Shift> CreateTestShifts(List<User> users, List<Area> areas)
        {
            if (users == null || users.Count == 0)
            {
                throw new ArgumentException("Users list cannot be null or empty", nameof(users));
            }

            if (areas == null)
            {
                throw new ArgumentNullException(nameof(areas));
            }

            var staffUser = users.FirstOrDefault(u => u.Role == UserRole.Staff);
            if (staffUser == null)
            {
                throw new InvalidOperationException("A staff member user is required to create test shifts");
            }

            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            return new List<Shift>
            {
                new Shift
                {
                    Id = new Guid("00000000-0000-0000-0000-000000000030"),
                    OrganizationId = new Guid("00000000-0000-0000-0000-000000000001"),
                    StaffId = staffUser.Id,
                    Type = ShiftType.Planned,
                    StartTime = today.AddHours(10),
                    EndTime = today.AddHours(18),
                    Status = ShiftStatus.Active,
                    CreatedBy = users[0].Id,
                    CreatedAt = DateTime.UtcNow
                },
                new Shift
                {
                    Id = new Guid("00000000-0000-0000-0000-000000000031"),
                    OrganizationId = new Guid("00000000-0000-0000-0000-000000000001"),
                    StaffId = staffUser.Id,
                    Type = ShiftType.Planned,
                    StartTime = tomorrow.AddHours(14),
                    EndTime = tomorrow.AddHours(22),
                    Status = ShiftStatus.Approved,
                    CreatedBy = users[0].Id,
                    CreatedAt = DateTime.UtcNow
                }
            };
        }
    }
}
