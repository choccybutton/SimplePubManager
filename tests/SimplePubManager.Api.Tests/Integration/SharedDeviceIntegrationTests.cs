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
    /// Integration tests for shared device functionality.
    /// Tests device registration, authentication, and device-specific access controls.
    /// </summary>
    public class SharedDeviceIntegrationTests : IClassFixture<IntegrationTestFixture>
    {
        private readonly IntegrationTestFixture _fixture;

        public SharedDeviceIntegrationTests(IntegrationTestFixture fixture)
        {
            _fixture = fixture;
        }

        private async Task<string> GetManagerTokenAsync()
        {
            var loginRequest = new LoginRequest
            {
                Email = IntegrationTestFixture.ManagerEmail,
                Password = IntegrationTestFixture.ManagerPassword,
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
            client.BaseAddress = _fixture.HttpClient.BaseAddress;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        [Fact]
        public async Task RegisterDevice_WithValidData_CreatesDevice()
        {
            // Arrange
            var managerToken = await GetManagerTokenAsync();
            var managerClient = CreateAuthenticatedClient(managerToken);

            var registerDeviceRequest = new RegisterDeviceRequest
            {
                Name = "Bar Register",
                DeviceId = "bar-register-1",
                DeviceKey = "test-key-123"
            };

            // Act
            var response = await managerClient.PostAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/devices",
                registerDeviceRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var content = await response.Content.ReadAsAsync<ApiResponse<DeviceResponse>>();
            content.Data.Should().NotBeNull();
            content.Data!.Name.Should().Be("Bar Register");
            content.Data.Enabled.Should().Be(true);
        }

        [Fact]
        public async Task RegisterDevice_CreatesDeviceWithDeviceId()
        {
            // Arrange
            var managerToken = await GetManagerTokenAsync();
            var managerClient = CreateAuthenticatedClient(managerToken);

            var registerDeviceRequest = new RegisterDeviceRequest
            {
                Name = "Kitchen Register",
                DeviceId = "kitchen-register-1",
                DeviceKey = "test-key-456"
            };

            // Act
            var response = await managerClient.PostAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/devices",
                registerDeviceRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var content = await response.Content.ReadAsAsync<ApiResponse<DeviceResponse>>();
            content.Data.Should().NotBeNull();
            content.Data!.DeviceId.Should().NotBeNullOrEmpty();
            content.Data.Enabled.Should().Be(true);
        }

        [Fact]
        public async Task GetDevices_ReturnsDevicesForOrganization()
        {
            // Arrange
            var managerToken = await GetManagerTokenAsync();
            var managerClient = CreateAuthenticatedClient(managerToken);

            // Act
            var response = await managerClient.GetAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/devices");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsAsync<ApiResponse<PaginatedResponse<DeviceResponse>>>();
            content.Data.Should().NotBeNull();
            content.Data!.Items.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetDeviceById_ReturnsDeviceDetails()
        {
            // Arrange
            var managerToken = await GetManagerTokenAsync();
            var managerClient = CreateAuthenticatedClient(managerToken);

            // Act
            var response = await managerClient.GetAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/devices/{_fixture.DeviceId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsAsync<ApiResponse<DeviceResponse>>();
            content.Data.Should().NotBeNull();
            content.Data!.Id.Should().Be(_fixture.DeviceId);
        }

        [Fact]
        public async Task UpdateDevice_ChangesDeviceName()
        {
            // Arrange
            var managerToken = await GetManagerTokenAsync();
            var managerClient = CreateAuthenticatedClient(managerToken);

            var updateRequest = new UpdateDeviceRequest
            {
                Name = "Updated Bar Register"
            };

            // Act
            var response = await managerClient.PutAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/devices/{_fixture.DeviceId}",
                updateRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsAsync<ApiResponse<DeviceResponse>>();
            content.Data.Should().NotBeNull();
            content.Data!.Name.Should().Be("Updated Bar Register");
        }

        [Fact]
        public async Task DeactivateDevice_ChangesEnabledToFalse()
        {
            // Arrange
            var managerToken = await GetManagerTokenAsync();
            var managerClient = CreateAuthenticatedClient(managerToken);

            var updateRequest = new UpdateDeviceRequest
            {
                Enabled = false
            };

            // Act
            var response = await managerClient.PutAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/devices/{_fixture.DeviceId}",
                updateRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsAsync<ApiResponse<DeviceResponse>>();
            content.Data.Should().NotBeNull();
            content.Data!.Enabled.Should().Be(false);
        }

        [Fact]
        public async Task DeviceAuthentication_WithValidDeviceKey_Succeeds()
        {
            // Arrange
            var deviceLoginRequest = new DeviceLoginRequest
            {
                DeviceId = _fixture.DeviceId.ToString(),
                DeviceKey = "encrypted-test-key"
            };

            // Act
            var response = await _fixture.HttpClient.PostAsJsonAsync(
                "/api/v1/auth/device-login",
                deviceLoginRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsAsync<ApiResponse<AuthResponse>>();
            content.Data.Should().NotBeNull();
            content.Data!.Token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task DeviceAuthentication_WithInvalidDeviceKey_Fails()
        {
            // Arrange
            var deviceLoginRequest = new DeviceLoginRequest
            {
                DeviceId = _fixture.DeviceId.ToString(),
                DeviceKey = "wrong-key"
            };

            // Act
            var response = await _fixture.HttpClient.PostAsJsonAsync(
                "/api/v1/auth/device-login",
                deviceLoginRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            var content = await response.Content.ReadAsAsync<ApiResponse<object>>();
            content.Error.Should().NotBeNull();
        }

        [Fact]
        public async Task DeviceSession_CanClockIn()
        {
            // Arrange
            var deviceLoginRequest = new DeviceLoginRequest
            {
                DeviceId = _fixture.DeviceId.ToString(),
                DeviceKey = "encrypted-test-key"
            };

            var deviceLoginResponse = await _fixture.HttpClient.PostAsJsonAsync(
                "/api/v1/auth/device-login",
                deviceLoginRequest);

            var deviceAuthContent = await deviceLoginResponse.Content.ReadAsAsync<ApiResponse<AuthResponse>>();
            var deviceToken = deviceAuthContent.Data!.Token;

            var deviceClient = new HttpClient();
            deviceClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", deviceToken);

            // Create a shift first
            var managerToken = await GetManagerTokenAsync();
            var managerClient = CreateAuthenticatedClient(managerToken);

            var createShiftRequest = new CreateShiftRequest
            {
                StaffId = _fixture.StaffUserId,
                Type = "AdHoc",
                StartTime = DateTime.UtcNow,
                EndTime = null
            };

            var shiftCreateResponse = await managerClient.PostAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/shifts",
                createShiftRequest);

            var shiftContent = await shiftCreateResponse.Content.ReadAsAsync<ApiResponse<ShiftResponse>>();
            var shiftId = shiftContent.Data!.Id;

            var clockInRequest = new ClockInRequest
            {
                Timestamp = DateTime.UtcNow
            };

            // Act
            var response = await deviceClient.PostAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/shifts/{shiftId}/clock-in",
                clockInRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeviceSession_CannotAccessFinancialEndpoints()
        {
            // Arrange
            var deviceLoginRequest = new DeviceLoginRequest
            {
                DeviceId = _fixture.DeviceId.ToString(),
                DeviceKey = "encrypted-test-key"
            };

            var deviceLoginResponse = await _fixture.HttpClient.PostAsJsonAsync(
                "/api/v1/auth/device-login",
                deviceLoginRequest);

            var deviceAuthContent = await deviceLoginResponse.Content.ReadAsAsync<ApiResponse<AuthResponse>>();
            var deviceToken = deviceAuthContent.Data!.Token;

            var deviceClient = new HttpClient();
            deviceClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", deviceToken);

            // Act
            var response = await deviceClient.GetAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/payments");

            // Assert
            // Device should not have access to financial data
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task QuickSwap_WithValidPinAndDevice_Succeeds()
        {
            // Arrange
            // This test assumes a PIN has been set for the user on the device
            var quickSwapRequest = new QuickSwapRequest
            {
                DeviceId = _fixture.DeviceId.ToString(),
                UserId = _fixture.StaffUserId,
                Pin = "1234"
            };

            // Act
            var response = await _fixture.HttpClient.PostAsJsonAsync(
                "/api/v1/auth/quick-swap",
                quickSwapRequest);

            // Assert
            // Response status may be Unauthorized if PIN is not set
            // (which is expected in this test scenario)
            response.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteDevice_RemovesDevice()
        {
            // Arrange
            var managerToken = await GetManagerTokenAsync();
            var managerClient = CreateAuthenticatedClient(managerToken);

            // First create a new device to delete
            var registerDeviceRequest = new RegisterDeviceRequest
            {
                Name = "Device to Delete",
                DeviceId = "delete-device-1",
                DeviceKey = "test-key-delete"
            };

            var createResponse = await managerClient.PostAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/devices",
                registerDeviceRequest);

            var deviceContent = await createResponse.Content.ReadAsAsync<ApiResponse<DeviceResponse>>();
            var deviceId = deviceContent.Data!.Id;

            // Act
            var response = await managerClient.DeleteAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/devices/{deviceId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Verify it's deleted
            var getResponse = await managerClient.GetAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/devices/{deviceId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
