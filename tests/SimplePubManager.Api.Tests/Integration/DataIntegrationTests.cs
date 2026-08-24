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
    /// Integration tests for data persistence and complex workflows.
    /// Tests that data is correctly stored and retrieved from the database.
    /// </summary>
    public class DataIntegrationTests : IClassFixture<IntegrationTestFixture>
    {
        private readonly IntegrationTestFixture _fixture;

        public DataIntegrationTests(IntegrationTestFixture fixture)
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
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        [Fact]
        public async Task CreateShift_WithMultipleAreas_PersistsToDatabase()
        {
            // Arrange
            var managerToken = await GetManagerTokenAsync();
            var managerClient = CreateAuthenticatedClient(managerToken);

            var createShiftRequest = new CreateShiftRequest
            {
                StaffId = _fixture.StaffUserId,
                Type = "AdHoc",
                StartTime = DateTime.UtcNow,
                EndTime = null,
                AreaIds = new List<Guid> { _fixture.KitchenAreaId, _fixture.BarAreaId }
            };

            // Act
            var response = await managerClient.PostAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/shifts",
                createShiftRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var content = await response.Content.ReadAsAsync<ApiResponse<ShiftResponse>>();
            var shiftId = content.Data!.Id;

            // Verify by retrieving from database
            var getResponse = await managerClient.GetAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/shifts/{shiftId}");

            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var retrievedContent = await getResponse.Content.ReadAsAsync<ApiResponse<ShiftResponse>>();
            retrievedContent.Data.Should().NotBeNull();
            retrievedContent.Data!.AreaIds.Should().HaveCount(2);
            retrievedContent.Data.AreaIds.Should().Contain(_fixture.KitchenAreaId);
            retrievedContent.Data.AreaIds.Should().Contain(_fixture.BarAreaId);
        }

        [Fact]
        public async Task CreateTask_InArea_CanBeAssignedToStaff()
        {
            // Arrange
            var managerToken = await GetManagerTokenAsync();
            var managerClient = CreateAuthenticatedClient(managerToken);

            var createTaskRequest = new CreateTaskRequest
            {
                Title = "Clean Bar",
                Description = "Clean the bar thoroughly",
                AreaId = _fixture.BarAreaId,
                AssignedToId = _fixture.StaffUserId
            };

            // Act
            var response = await managerClient.PostAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/tasks",
                createTaskRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var content = await response.Content.ReadAsAsync<ApiResponse<TaskResponse>>();
            content.Data.Should().NotBeNull();
            content.Data!.Title.Should().Be("Clean Bar");
            content.Data.AreaId.Should().Be(_fixture.BarAreaId);
            content.Data.AssignedToId.Should().Be(_fixture.StaffUserId);
        }

        [Fact]
        public async Task RequestHoliday_PersistsToDatabase()
        {
            // Arrange
            var managerToken = await GetManagerTokenAsync();
            var managerClient = CreateAuthenticatedClient(managerToken);

            var startDate = DateTime.UtcNow.AddDays(7);
            var endDate = DateTime.UtcNow.AddDays(10);

            var createHolidayRequest = new CreateHolidayRequest
            {
                StartDate = startDate,
                EndDate = endDate,
                Reason = "Summer vacation"
            };

            // Act
            var response = await managerClient.PostAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/holidays",
                createHolidayRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var content = await response.Content.ReadAsAsync<ApiResponse<HolidayResponse>>();
            var holidayId = content.Data!.Id;
            content.Data.StartDate.Date.Should().Be(startDate.Date);
            content.Data.EndDate.Date.Should().Be(endDate.Date);
            content.Data.Status.Should().Be("Pending");

            // Verify by retrieving
            var getResponse = await managerClient.GetAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/holidays/{holidayId}");

            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var retrievedContent = await getResponse.Content.ReadAsAsync<ApiResponse<HolidayResponse>>();
            retrievedContent.Data.Should().NotBeNull();
            retrievedContent.Data!.StartDate.Date.Should().Be(startDate.Date);
        }

        [Fact]
        public async Task ApproveHoliday_UpdatesDatabase()
        {
            // Arrange
            var managerToken = await GetManagerTokenAsync();
            var managerClient = CreateAuthenticatedClient(managerToken);

            // Create a holiday request
            var createHolidayRequest = new CreateHolidayRequest
            {
                StartDate = DateTime.UtcNow.AddDays(7),
                EndDate = DateTime.UtcNow.AddDays(10),
                Reason = "Summer vacation"
            };

            var createResponse = await managerClient.PostAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/holidays",
                createHolidayRequest);

            var content = await createResponse.Content.ReadAsAsync<ApiResponse<HolidayResponse>>();
            var holidayId = content.Data!.Id;

            var approveRequest = new { Status = "Approved" };

            // Act
            var response = await managerClient.PutAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/holidays/{holidayId}",
                approveRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var approveContent = await response.Content.ReadAsAsync<ApiResponse<HolidayResponse>>();
            approveContent.Data.Should().NotBeNull();
            approveContent.Data!.Status.Should().Be("Approved");

            // Verify by retrieving
            var getResponse = await managerClient.GetAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/holidays/{holidayId}");

            var retrievedContent = await getResponse.Content.ReadAsAsync<ApiResponse<HolidayResponse>>();
            retrievedContent.Data!.Status.Should().Be("Approved");
        }

        [Fact]
        public async Task CreatePayment_RecordsToDatabase()
        {
            // Arrange
            var managerToken = await GetManagerTokenAsync();
            var managerClient = CreateAuthenticatedClient(managerToken);

            var createPaymentRequest = new CreatePaymentRequest
            {
                Amount = 150.00m,
                Type = "Salary",
                RelatedShiftId = null
            };

            // Act
            var response = await managerClient.PostAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/payments",
                createPaymentRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var content = await response.Content.ReadAsAsync<ApiResponse<PaymentResponse>>();
            content.Data.Should().NotBeNull();
            content.Data!.Amount.Should().Be(150.00m);
            content.Data.StaffId.Should().Be(_fixture.StaffUserId);
            var paymentId = content.Data.Id;

            // Verify by retrieving
            var getResponse = await managerClient.GetAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/payments/{paymentId}");

            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var retrievedContent = await getResponse.Content.ReadAsAsync<ApiResponse<PaymentResponse>>();
            retrievedContent.Data.Should().NotBeNull();
            retrievedContent.Data!.Amount.Should().Be(150.00m);
        }

        [Fact]
        public async Task ListPayments_ReturnsAllPaymentsForOrganization()
        {
            // Arrange
            var managerToken = await GetManagerTokenAsync();
            var managerClient = CreateAuthenticatedClient(managerToken);

            // Create multiple payments
            for (int i = 0; i < 3; i++)
            {
                var createPaymentRequest = new CreatePaymentRequest
                {
                    StaffId = _fixture.StaffUserId,
                    Amount = (100.00m + i * 50),
                    Type = "Salary",
                    RelatedShiftId = null
                };

                await managerClient.PostAsJsonAsync(
                    $"/api/v1/organizations/{_fixture.TestOrganizationId}/payments",
                    createPaymentRequest);
            }

            // Act
            var response = await managerClient.GetAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/payments");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsAsync<ApiResponse<PaginatedResponse<PaymentResponse>>>();
            content.Data.Should().NotBeNull();
            content.Data!.Items.Count().Should().BeGreaterThanOrEqualTo(3);
        }

        [Fact]
        public async Task FilterShiftsByStaff_ReturnsOnlyStaffShifts()
        {
            // Arrange
            var managerToken = await GetManagerTokenAsync();
            var managerClient = CreateAuthenticatedClient(managerToken);

            // Create shifts for different staff
            for (int i = 0; i < 2; i++)
            {
                var staffId = i == 0 ? _fixture.StaffUserId : _fixture.StaffUser2Id;
                var createShiftRequest = new CreateShiftRequest
                {
                    StaffId = staffId,
                    Type = "AdHoc",
                    StartTime = DateTime.UtcNow.AddHours(i),
                    EndTime = null
                };

                await managerClient.PostAsJsonAsync(
                    $"/api/v1/organizations/{_fixture.TestOrganizationId}/shifts",
                    createShiftRequest);
            }

            // Act
            var response = await managerClient.GetAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/shifts?staffId={_fixture.StaffUserId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsAsync<ApiResponse<PaginatedResponse<ShiftResponse>>>();
            content.Data.Should().NotBeNull();
            content.Data!.Items.Should().AllSatisfy(s => s.StaffId.Should().Be(_fixture.StaffUserId));
        }

        [Fact]
        public async Task GetAreas_ReturnsAllAreasForOrganization()
        {
            // Arrange
            var managerToken = await GetManagerTokenAsync();
            var managerClient = CreateAuthenticatedClient(managerToken);

            // Act
            var response = await managerClient.GetAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/areas");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsAsync<ApiResponse<PaginatedResponse<AreaResponse>>>();
            content.Data.Should().NotBeNull();
            content.Data!.Items.Should().HaveCount(3); // Kitchen, Bar, Front of House
            content.Data.Items.Select(a => a.Name)
                .Should().Contain(new[] { "Kitchen", "Bar", "Front of House" });
        }

        [Fact]
        public async Task CreateArea_PersistsToDatabase()
        {
            // Arrange
            var managerToken = await GetManagerTokenAsync();
            var managerClient = CreateAuthenticatedClient(managerToken);

            var createAreaRequest = new CreateAreaRequest
            {
                Name = "Storage",
                Description = "Storage room"
            };

            // Act
            var response = await managerClient.PostAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/areas",
                createAreaRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var content = await response.Content.ReadAsAsync<ApiResponse<AreaResponse>>();
            content.Data.Should().NotBeNull();
            content.Data!.Name.Should().Be("Storage");
            var areaId = content.Data.Id;

            // Verify by retrieving
            var getResponse = await managerClient.GetAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/areas/{areaId}");

            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var retrievedContent = await getResponse.Content.ReadAsAsync<ApiResponse<AreaResponse>>();
            retrievedContent.Data.Should().NotBeNull();
            retrievedContent.Data!.Name.Should().Be("Storage");
        }

        [Fact]
        public async Task UpdateArea_PersistsChangesToDatabase()
        {
            // Arrange
            var managerToken = await GetManagerTokenAsync();
            var managerClient = CreateAuthenticatedClient(managerToken);

            var updateRequest = new UpdateAreaRequest
            {
                Name = "Updated Bar",
                Description = "Updated bar description"
            };

            // Act
            var response = await managerClient.PutAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/areas/{_fixture.BarAreaId}",
                updateRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsAsync<ApiResponse<AreaResponse>>();
            content.Data.Should().NotBeNull();
            content.Data!.Name.Should().Be("Updated Bar");

            // Verify by retrieving
            var getResponse = await managerClient.GetAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/areas/{_fixture.BarAreaId}");

            var retrievedContent = await getResponse.Content.ReadAsAsync<ApiResponse<AreaResponse>>();
            retrievedContent.Data!.Name.Should().Be("Updated Bar");
        }

        [Fact]
        public async Task ConcurrentShiftCreation_BothSucceed()
        {
            // Arrange
            var managerToken = await GetManagerTokenAsync();
            var managerClient = CreateAuthenticatedClient(managerToken);

            var createShiftRequest1 = new CreateShiftRequest
            {
                StaffId = _fixture.StaffUserId,
                Type = "AdHoc",
                StartTime = DateTime.UtcNow,
                EndTime = null
            };

            var createShiftRequest2 = new CreateShiftRequest
            {
                StaffId = _fixture.StaffUser2Id,
                Type = "AdHoc",
                StartTime = DateTime.UtcNow,
                EndTime = null
            };

            // Act
            var task1 = managerClient.PostAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/shifts",
                createShiftRequest1);

            var task2 = managerClient.PostAsJsonAsync(
                $"/api/v1/organizations/{_fixture.TestOrganizationId}/shifts",
                createShiftRequest2);

            await Task.WhenAll(task1, task2);

            // Assert
            task1.Result.StatusCode.Should().Be(HttpStatusCode.Created);
            task2.Result.StatusCode.Should().Be(HttpStatusCode.Created);
        }
    }
}
