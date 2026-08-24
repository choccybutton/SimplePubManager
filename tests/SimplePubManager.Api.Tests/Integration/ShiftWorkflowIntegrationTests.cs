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
    /// Integration tests for complete shift workflows.
    /// Tests creating shifts, clocking in/out, and payment calculations.
    /// </summary>
    public class ShiftWorkflowIntegrationTests : IClassFixture<IntegrationTestFixture>
    {
        private readonly IntegrationTestFixture _fixture;

        public ShiftWorkflowIntegrationTests(IntegrationTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task CreateShift_WithValidData_CreatesShift()
        {
            // Arrange
            var createShiftRequest = new CreateShiftRequest
            {
                StaffId = _fixture.StaffUserId,
                Type = "AdHoc",
                StartTime = DateTime.UtcNow,
                EndTime = null,
                AreaIds = new List<Guid> { _fixture.BarAreaId }
            };

            // Act
            var response = await _fixture.HttpClient.PostAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/shifts",
                createShiftRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var content = await response.Content.ReadAsAsync<ApiResponse<ShiftResponse>>();
            content.Data.Should().NotBeNull();
            content.Data!.StaffId.Should().Be(_fixture.StaffUserId);
            content.Data.Status.Should().Be("Pending");
        }

        [Fact]
        public async Task CreateShift_WithMultipleAreas_CreatesShiftWithAreas()
        {
            // Arrange
            var createShiftRequest = new CreateShiftRequest
            {
                StaffId = _fixture.StaffUserId,
                Type = "AdHoc",
                StartTime = DateTime.UtcNow,
                EndTime = null,
                AreaIds = new List<Guid> { _fixture.KitchenAreaId, _fixture.BarAreaId, _fixture.FrontOfHouseAreaId }
            };

            // Act
            var response = await _fixture.HttpClient.PostAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/shifts",
                createShiftRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var content = await response.Content.ReadAsAsync<ApiResponse<ShiftResponse>>();
            content.Data.Should().NotBeNull();
            content.Data!.StaffId.Should().Be(_fixture.StaffUserId);
            content.Data.AreaIds.Should().HaveCount(3);
        }

        [Fact]
        public async Task CreateShift_WithoutAuthentication_ReturnsUnauthorized()
        {
            // Arrange
            var unauthenticatedClient = new HttpClient();
            var createShiftRequest = new CreateShiftRequest
            {
                StaffId = _fixture.StaffUserId,
                Type = "AdHoc",
                StartTime = DateTime.UtcNow,
                EndTime = null
            };

            // Act
            var response = await unauthenticatedClient.PostAsJsonAsync(
                $"http://localhost/api/v1/organizations/{_fixture.TestOrganizationId}/shifts",
                createShiftRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetShifts_ReturnsShiftsForOrganization()
        {
            // Arrange
            // Create a shift first
            var createShiftRequest = new CreateShiftRequest
            {
                StaffId = _fixture.StaffUserId,
                Type = "AdHoc",
                StartTime = DateTime.UtcNow,
                EndTime = null
            };

            await _fixture.HttpClient.PostAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/shifts",
                createShiftRequest);

            // Act
            var response = await _fixture.HttpClient.GetAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/shifts");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsAsync<ApiResponse<PaginatedResponse<ShiftResponse>>>();
            content.Data.Should().NotBeNull();
            content.Data!.Items.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetShifts_WithStatusFilter_ReturnsFilteredShifts()
        {
            // Arrange
            var createShiftRequest = new CreateShiftRequest
            {
                StaffId = _fixture.StaffUserId,
                Type = "AdHoc",
                StartTime = DateTime.UtcNow,
                EndTime = null
            };

            await _fixture.HttpClient.PostAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/shifts",
                createShiftRequest);

            // Act
            var response = await _fixture.HttpClient.GetAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/shifts?status=Pending");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsAsync<ApiResponse<PaginatedResponse<ShiftResponse>>>();
            content.Data.Should().NotBeNull();
            content.Data!.Items.Should().AllSatisfy(s => s.Status.Should().Be("Pending"));
        }

        [Fact]
        public async Task ClockIn_CreatesShiftLog()
        {
            // Arrange
            // Create a shift first
            var createShiftRequest = new CreateShiftRequest
            {
                StaffId = _fixture.StaffUserId,
                Type = "AdHoc",
                StartTime = DateTime.UtcNow,
                EndTime = null
            };

            var shiftCreateResponse = await _fixture.HttpClient.PostAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/shifts",
                createShiftRequest);

            var shiftContent = await shiftCreateResponse.Content.ReadAsAsync<ApiResponse<ShiftResponse>>();
            var shiftId = shiftContent.Data!.Id;

            var clockInRequest = new ClockInRequest
            {
                Timestamp = DateTime.UtcNow
            };

            // Act
            var response = await _fixture.HttpClient.PostAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/shifts/{shiftId}/clock-in",
                clockInRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsAsync<ApiResponse<ShiftResponse>>();
            content.Data.Should().NotBeNull();
            content.Data!.Status.Should().Be("InProgress");
        }

        [Fact]
        public async Task ClockOut_UpdatesShiftStatus()
        {
            // Arrange
            // Create and clock in to a shift first
            var createShiftRequest = new CreateShiftRequest
            {
                StaffId = _fixture.StaffUserId,
                Type = "AdHoc",
                StartTime = DateTime.UtcNow.AddHours(-1),
                EndTime = null
            };

            var shiftCreateResponse = await _fixture.HttpClient.PostAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/shifts",
                createShiftRequest);

            var shiftContent = await shiftCreateResponse.Content.ReadAsAsync<ApiResponse<ShiftResponse>>();
            var shiftId = shiftContent.Data!.Id;

            // Clock in
            var clockInRequest = new ClockInRequest
            {
                Timestamp = DateTime.UtcNow.AddHours(-1)
            };

            await _fixture.HttpClient.PostAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/shifts/{shiftId}/clock-in",
                clockInRequest);

            // Act - Clock out
            var clockOutRequest = new ClockInRequest
            {
                Timestamp = DateTime.UtcNow
            };

            var response = await _fixture.HttpClient.PostAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/shifts/{shiftId}/clock-out",
                clockOutRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsAsync<ApiResponse<ShiftResponse>>();
            content.Data.Should().NotBeNull();
            content.Data!.Status.Should().Be("Completed");
        }

        [Fact]
        public async Task ApproveShift_ChangesStatusToApproved()
        {
            // Arrange
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
            var response = await _fixture.HttpClient.PutAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/shifts/{shiftId}",
                updateRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsAsync<ApiResponse<ShiftResponse>>();
            content.Data.Should().NotBeNull();
            content.Data!.Status.Should().Be("Approved");
        }

        [Fact]
        public async Task RejectShift_ChangesStatusToRejected()
        {
            // Arrange
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
                Status = "Rejected"
            };

            // Act
            var response = await _fixture.HttpClient.PutAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/shifts/{shiftId}",
                updateRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsAsync<ApiResponse<ShiftResponse>>();
            content.Data.Should().NotBeNull();
            content.Data!.Status.Should().Be("Rejected");
        }

        [Fact]
        public async Task DeleteShift_RemovesShift()
        {
            // Arrange
            var createShiftRequest = new CreateShiftRequest
            {
                StaffId = _fixture.StaffUserId,
                Type = "AdHoc",
                StartTime = DateTime.UtcNow,
                EndTime = null
            };

            var shiftCreateResponse = await _fixture.HttpClient.PostAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/shifts",
                createShiftRequest);

            var shiftContent = await shiftCreateResponse.Content.ReadAsAsync<ApiResponse<ShiftResponse>>();
            var shiftId = shiftContent.Data!.Id;

            // Act
            var response = await _fixture.HttpClient.DeleteAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/shifts/{shiftId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Verify it's deleted
            var getResponse = await _fixture.HttpClient.GetAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/shifts/{shiftId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetShiftById_ReturnsShiftDetails()
        {
            // Arrange
            var createShiftRequest = new CreateShiftRequest
            {
                StaffId = _fixture.StaffUserId,
                Type = "AdHoc",
                StartTime = DateTime.UtcNow,
                EndTime = null
            };

            var shiftCreateResponse = await _fixture.HttpClient.PostAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/shifts",
                createShiftRequest);

            var shiftContent = await shiftCreateResponse.Content.ReadAsAsync<ApiResponse<ShiftResponse>>();
            var shiftId = shiftContent.Data!.Id;

            // Act
            var response = await _fixture.HttpClient.GetAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/shifts/{shiftId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsAsync<ApiResponse<ShiftResponse>>();
            content.Data.Should().NotBeNull();
            content.Data!.Id.Should().Be(shiftId);
            content.Data.StaffId.Should().Be(_fixture.StaffUserId);
        }
    }
}
