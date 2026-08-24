using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SimplePubManager.Shared.Dto;
using SimplePubManager.Shared.Dto.Request;
using SimplePubManager.Shared.Dto.Response;
using SimplePubManager.Api.Tests.Fixtures;
using Xunit;

namespace SimplePubManager.Api.Tests.Integration
{
    /// <summary>
    /// Integration tests for permission and role-based access control.
    /// Tests that different user roles have appropriate access levels.
    /// </summary>
    public class PermissionIntegrationTests : IClassFixture<IntegrationTestFixture>
    {
        private readonly IntegrationTestFixture _fixture;

        public PermissionIntegrationTests(IntegrationTestFixture fixture)
        {
            _fixture = fixture;
        }

        private async Task<string> GetAuthTokenAsync(string email, string password)
        {
            var loginRequest = new LoginRequest
            {
                Email = email,
                Password = password,
                OrganizationId = _fixture.TestOrganizationId
            };

            var response = await _fixture.HttpClient.PostAsJsonAsync(
                "/api/v1/auth/login",
                loginRequest);

            var content = await response.Content.ReadAsAsync<ApiResponse<AuthResponse>>();
            return content.Data!.Token;
        }

        private HttpClient CreateAuthenticatedClient(string token)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        [Fact]
        public async Task Manager_CanApproveShifts()
        {
            // Arrange
            var managerToken = await GetAuthTokenAsync(IntegrationTestFixture.ManagerEmail, IntegrationTestFixture.ManagerPassword);
            var managerClient = CreateAuthenticatedClient(managerToken);

            // Create a shift first
            var createShiftRequest = new CreateShiftRequest
            {
                StaffId = _fixture.StaffUserId,
                Type = "AdHoc",
                StartTime = DateTime.UtcNow.AddHours(-2),
                EndTime = DateTime.UtcNow.AddHours(-1)
            };

            var shiftCreateResponse = await _fixture.HttpClient.PostAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/shifts",
                createShiftRequest);

            var shiftContent = await shiftCreateResponse.Content.ReadAsAsync<ApiResponse<ShiftResponse>>();
            var shiftId = shiftContent.Data!.Id;

            var updateRequest = new UpdateShiftRequest
            {
                Status = "Approved"
            };

            // Act
            var response = await managerClient.PutAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/shifts/{shiftId}",
                updateRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsAsync<ApiResponse<ShiftResponse>>();
            content.Data.Should().NotBeNull();
            content.Data!.Status.Should().Be("Approved");
        }

        [Fact]
        public async Task Supervisor_CanApproveShifts()
        {
            // Arrange
            var supervisorToken = await GetAuthTokenAsync(IntegrationTestFixture.SupervisorEmail, IntegrationTestFixture.SupervisorPassword);
            var supervisorClient = CreateAuthenticatedClient(supervisorToken);

            // Create a shift first
            var createShiftRequest = new CreateShiftRequest
            {
                StaffId = _fixture.StaffUserId,
                Type = "AdHoc",
                StartTime = DateTime.UtcNow.AddHours(-2),
                EndTime = DateTime.UtcNow.AddHours(-1)
            };

            var shiftCreateResponse = await _fixture.HttpClient.PostAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/shifts",
                createShiftRequest);

            var shiftContent = await shiftCreateResponse.Content.ReadAsAsync<ApiResponse<ShiftResponse>>();
            var shiftId = shiftContent.Data!.Id;

            var updateRequest = new UpdateShiftRequest
            {
                Status = "Approved"
            };

            // Act
            var response = await supervisorClient.PutAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/shifts/{shiftId}",
                updateRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Staff_CannotApproveShifts()
        {
            // Arrange
            var staffToken = await GetAuthTokenAsync(IntegrationTestFixture.StaffEmail, IntegrationTestFixture.StaffPassword);
            var staffClient = CreateAuthenticatedClient(staffToken);

            // Create a shift first
            var createShiftRequest = new CreateShiftRequest
            {
                StaffId = _fixture.StaffUserId,
                Type = "AdHoc",
                StartTime = DateTime.UtcNow.AddHours(-2),
                EndTime = DateTime.UtcNow.AddHours(-1)
            };

            var shiftCreateResponse = await _fixture.HttpClient.PostAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/shifts",
                createShiftRequest);

            var shiftContent = await shiftCreateResponse.Content.ReadAsAsync<ApiResponse<ShiftResponse>>();
            var shiftId = shiftContent.Data!.Id;

            var updateRequest = new UpdateShiftRequest
            {
                Status = "Approved"
            };

            // Act
            var response = await staffClient.PutAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/shifts/{shiftId}",
                updateRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Manager_CanDeleteUsers()
        {
            // Arrange
            var managerToken = await GetAuthTokenAsync(IntegrationTestFixture.ManagerEmail, IntegrationTestFixture.ManagerPassword);
            var managerClient = CreateAuthenticatedClient(managerToken);

            // Act
            var response = await managerClient.DeleteAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/staff/{_fixture.StaffUser2Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Supervisor_CannotDeleteUsers()
        {
            // Arrange
            var supervisorToken = await GetAuthTokenAsync(IntegrationTestFixture.SupervisorEmail, IntegrationTestFixture.SupervisorPassword);
            var supervisorClient = CreateAuthenticatedClient(supervisorToken);

            // Act
            var response = await supervisorClient.DeleteAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/staff/{_fixture.StaffUser2Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Staff_CannotViewOtherStaffData()
        {
            // Arrange
            var staffToken = await GetAuthTokenAsync(IntegrationTestFixture.StaffEmail, IntegrationTestFixture.StaffPassword);
            var staffClient = CreateAuthenticatedClient(staffToken);

            // Act
            var response = await staffClient.GetAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/staff/{_fixture.StaffUser2Id}");

            // Assert
            // Staff should not be able to view other staff members' data
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Manager_CanViewAllStaffData()
        {
            // Arrange
            var managerToken = await GetAuthTokenAsync(IntegrationTestFixture.ManagerEmail, IntegrationTestFixture.ManagerPassword);
            var managerClient = CreateAuthenticatedClient(managerToken);

            // Act
            var response = await managerClient.GetAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/staff");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsAsync<ApiResponse<PaginatedResponse<StaffResponse>>>();
            content.Data.Should().NotBeNull();
            content.Data!.Items.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Staff_CanRequestHolidays()
        {
            // Arrange
            var staffToken = await GetAuthTokenAsync(IntegrationTestFixture.StaffEmail, IntegrationTestFixture.StaffPassword);
            var staffClient = CreateAuthenticatedClient(staffToken);

            var createHolidayRequest = new CreateHolidayRequest
            {
                StartDate = DateTime.UtcNow.AddDays(7),
                EndDate = DateTime.UtcNow.AddDays(10),
                Reason = "Personal holiday"
            };

            // Act
            var response = await staffClient.PostAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/holidays",
                createHolidayRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var content = await response.Content.ReadAsAsync<ApiResponse<HolidayResponse>>();
            content.Data.Should().NotBeNull();
            content.Data!.Status.Should().Be("Pending");
        }

        [Fact]
        public async Task Manager_CanApproveHolidays()
        {
            // Arrange
            // Create a holiday request first as staff
            var staffToken = await GetAuthTokenAsync(IntegrationTestFixture.StaffEmail, IntegrationTestFixture.StaffPassword);
            var staffClient = CreateAuthenticatedClient(staffToken);

            var createHolidayRequest = new CreateHolidayRequest
            {
                StartDate = DateTime.UtcNow.AddDays(7),
                EndDate = DateTime.UtcNow.AddDays(10),
                Reason = "Personal holiday"
            };

            var holidayResponse = await staffClient.PostAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/holidays",
                createHolidayRequest);

            var holidayContent = await holidayResponse.Content.ReadAsAsync<ApiResponse<HolidayResponse>>();
            var holidayId = holidayContent.Data!.Id;

            // Act - Approve as manager
            var managerToken = await GetAuthTokenAsync(IntegrationTestFixture.ManagerEmail, IntegrationTestFixture.ManagerPassword);
            var managerClient = CreateAuthenticatedClient(managerToken);

            var approveRequest = new { Status = "Approved" };
            var approveResponse = await managerClient.PutAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/holidays/{holidayId}",
                approveRequest);

            // Assert
            approveResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await approveResponse.Content.ReadAsAsync<ApiResponse<HolidayResponse>>();
            content.Data.Should().NotBeNull();
            content.Data!.Status.Should().Be("Approved");
        }

        [Fact]
        public async Task Staff_CannotRegisterDevices()
        {
            // Arrange
            var staffToken = await GetAuthTokenAsync(IntegrationTestFixture.StaffEmail, IntegrationTestFixture.StaffPassword);
            var staffClient = CreateAuthenticatedClient(staffToken);

            var registerDeviceRequest = new RegisterDeviceRequest
            {
                Name = "Staff Device",
                DeviceId = "device-85fc99a5", DeviceKey = "test-key"
            };

            // Act
            var response = await staffClient.PostAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/devices",
                registerDeviceRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Manager_CanRegisterDevices()
        {
            // Arrange
            var managerToken = await GetAuthTokenAsync(IntegrationTestFixture.ManagerEmail, IntegrationTestFixture.ManagerPassword);
            var managerClient = CreateAuthenticatedClient(managerToken);

            var registerDeviceRequest = new RegisterDeviceRequest
            {
                Name = "Manager Device",
                DeviceId = "device-85fc99a5", DeviceKey = "test-key"
            };

            // Act
            var response = await managerClient.PostAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/devices",
                registerDeviceRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var content = await response.Content.ReadAsAsync<ApiResponse<DeviceResponse>>();
            content.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task Unauthenticated_CannotAccessProtectedEndpoints()
        {
            // Act
            var response = await _fixture.HttpClient.GetAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/shifts");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
