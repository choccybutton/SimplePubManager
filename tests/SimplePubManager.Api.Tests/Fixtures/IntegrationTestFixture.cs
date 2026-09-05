using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimplePubManager.Domain.Entities;
using SimplePubManager.Domain.Enums;
using SimplePubManager.Infrastructure.Data;
using SimplePubManager.Infrastructure.Services;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace SimplePubManager.Api.Tests.Fixtures
{
    /// <summary>
    /// Integration test fixture that provides a test web application factory
    /// with an in-memory database and seeded test data.
    /// </summary>
    public class IntegrationTestFixture : IAsyncLifetime
    {
        private WebApplicationFactory<Program> _factory = null!;
        private AppDbContext _dbContext = null!;
        public HttpClient HttpClient = null!;

        // Test data IDs for consistent reference
        public Guid TestOrganizationId { get; private set; } = Guid.NewGuid();
        public Guid ManagerUserId { get; private set; } = Guid.NewGuid();
        public Guid SupervisorUserId { get; private set; } = Guid.NewGuid();
        public Guid StaffUserId { get; private set; } = Guid.NewGuid();
        public Guid StaffUser2Id { get; private set; } = Guid.NewGuid();
        public Guid KitchenAreaId { get; private set; } = Guid.NewGuid();
        public Guid BarAreaId { get; private set; } = Guid.NewGuid();
        public Guid FrontOfHouseAreaId { get; private set; } = Guid.NewGuid();
        public Guid DeviceId { get; private set; } = Guid.NewGuid();

        // Test credentials
        public const string ManagerEmail = "manager@test.com";
        public const string ManagerPassword = "TestPass123!";
        public const string SupervisorEmail = "supervisor@test.com";
        public const string SupervisorPassword = "TestPass123!";
        public const string StaffEmail = "staff@test.com";
        public const string StaffPassword = "TestPass123!";
        public const string StaffEmail2 = "staff2@test.com";
        public const string StaffPassword2 = "TestPass123!";

        /// <summary>
        /// Initializes the test fixture with a web application factory and in-memory database.
        /// Seeds test data for use in integration tests.
        /// </summary>
        public async Task InitializeAsync()
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        // Remove the existing DbContext registration
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                        if (descriptor != null)
                        {
                            services.Remove(descriptor);
                        }

                        // Add in-memory database for testing
                        services.AddDbContext<AppDbContext>(options =>
                            options.UseInMemoryDatabase("TestDatabase"));
                    });
                });

            // Create HTTP client
            HttpClient = _factory.CreateClient();

            // Initialize database
            using (var scope = _factory.Services.CreateScope())
            {
                _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await _dbContext.Database.EnsureCreatedAsync();
                await SeedTestDataAsync(scope.ServiceProvider);
            }
        }

        /// <summary>
        /// Seeds test data into the database for use in tests.
        /// </summary>
        private async Task SeedTestDataAsync(IServiceProvider serviceProvider)
        {
            // Create organization
            var organization = new Organization
            {
                Id = TestOrganizationId,
                Name = "Test Pub",
                Subdomain = "testpub",
                CreatedAt = DateTime.UtcNow
            };

            await _dbContext.Organizations.AddAsync(organization);

            // Create areas
            var kitchenArea = new Area
            {
                Id = KitchenAreaId,
                OrganizationId = TestOrganizationId,
                Name = "Kitchen",
                Description = "Kitchen prep area",
                CreatedAt = DateTime.UtcNow
            };

            var barArea = new Area
            {
                Id = BarAreaId,
                OrganizationId = TestOrganizationId,
                Name = "Bar",
                Description = "Bar service area",
                CreatedAt = DateTime.UtcNow
            };

            var fohArea = new Area
            {
                Id = FrontOfHouseAreaId,
                OrganizationId = TestOrganizationId,
                Name = "Front of House",
                Description = "Customer-facing area",
                CreatedAt = DateTime.UtcNow
            };

            await _dbContext.Areas.AddRangeAsync(kitchenArea, barArea, fohArea);

            // Create users with hashed passwords
            var passwordService = serviceProvider.GetRequiredService<PasswordHashService>();

            var managerUser = new User
            {
                Id = ManagerUserId,
                OrganizationId = TestOrganizationId,
                Name = "Manager User",
                Email = ManagerEmail,
                PasswordHash = passwordService.HashPassword(ManagerPassword),
                Role = UserRole.Manager,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            var supervisorUser = new User
            {
                Id = SupervisorUserId,
                OrganizationId = TestOrganizationId,
                Name = "Supervisor User",
                Email = SupervisorEmail,
                PasswordHash = passwordService.HashPassword(SupervisorPassword),
                Role = UserRole.Supervisor,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            var staffUser = new User
            {
                Id = StaffUserId,
                OrganizationId = TestOrganizationId,
                Name = "Staff User",
                Email = StaffEmail,
                PasswordHash = passwordService.HashPassword(StaffPassword),
                Role = UserRole.Staff,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            var staffUser2 = new User
            {
                Id = StaffUser2Id,
                OrganizationId = TestOrganizationId,
                Name = "Staff User 2",
                Email = StaffEmail2,
                PasswordHash = passwordService.HashPassword(StaffPassword2),
                Role = UserRole.Staff,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            await _dbContext.Users.AddRangeAsync(managerUser, supervisorUser, staffUser, staffUser2);

            // Create a device
            var device = new Device
            {
                Id = DeviceId,
                OrganizationId = TestOrganizationId,
                Name = "Test Register",
                DeviceId = "test-device-1",
                DeviceKeyHash = "hashed-test-key",
                Enabled = true,
                CreatedAt = DateTime.UtcNow
            };

            await _dbContext.Devices.AddAsync(device);

            // Save all changes
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Cleans up resources after tests complete.
        /// </summary>
        public async Task DisposeAsync()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await dbContext.Database.EnsureDeletedAsync();
            }

            _factory?.Dispose();
            HttpClient?.Dispose();
        }

        /// <summary>
        /// Gets the database context for direct testing if needed.
        /// </summary>
        public AppDbContext GetDbContext()
        {
            return _dbContext;
        }

        /// <summary>
        /// Creates a new scope with a fresh DbContext.
        /// </summary>
        public IServiceScope CreateScope()
        {
            return _factory.Services.CreateScope();
        }
    }
}

/// <summary>
/// Extension methods for HttpContent to deserialize JSON responses.
/// </summary>
public static class HttpContentExtensions
{
    /// <summary>
    /// Reads the HttpContent as JSON and deserializes it to the specified type.
    /// </summary>
    public static async Task<T?> ReadAsAsync<T>(this HttpContent content)
    {
        var json = await content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
}
