using System.Net;
using System.Net.Http.Json;
using JobTracker.Application.DTOs.AuthDtos.LoginDtos;
using JobTracker.Application.DTOs.AuthDtos.RegisterDtos;
using JobTracker.Application.DTOs.CompanyDtos;
using JobTracker.Application.DTOs.JobApplicationDtos;
using JobTracker.Application.DTOs.JobCategoryDtos;
using JobTracker.Domain.Enums;

namespace JobTracker.Tests.IntegrationTests
{
    public class JobTrackerApiTests
        : IClassFixture<JobTrackerWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public JobTrackerApiTests(
            JobTrackerWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Register_AndLogin_ReturnsAccessToken()
        {
            var registerDto = new RegisterDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = $"test{Guid.NewGuid()}@example.com",
                Password = "Test123!"
            };

            var registerResponse =
                await _client.PostAsJsonAsync(
                    "/api/Auth/register",
                    registerDto);

            Assert.True(
                registerResponse.IsSuccessStatusCode);

            var loginDto = new LoginDto
            {
                Email = registerDto.Email,
                Password = registerDto.Password
            };

            var loginResponse =
                await _client.PostAsJsonAsync(
                    "/api/Auth/login",
                    loginDto);

            Assert.Equal(
                HttpStatusCode.OK,
                loginResponse.StatusCode);

            var loginResult =
                await loginResponse.Content
                    .ReadFromJsonAsync<LoginResponseDto>();

            Assert.NotNull(loginResult);
            Assert.False(
                string.IsNullOrWhiteSpace(
                    loginResult!.AccessToken));
            Assert.False(
                string.IsNullOrWhiteSpace(
                    loginResult.RefreshToken));
        }

        [Fact]
        public async Task AuthenticatedRequest_WithValidToken_ReturnsSuccess()
        {
            var registerDto = new RegisterDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = $"test{Guid.NewGuid()}@example.com",
                Password = "Test123!"
            };

            await _client.PostAsJsonAsync(
                "/api/Auth/register",
                registerDto);

            var loginDto = new LoginDto
            {
                Email = registerDto.Email,
                Password = registerDto.Password
            };

            var loginResponse =
                await _client.PostAsJsonAsync(
                    "/api/Auth/login",
                    loginDto);

            var loginResult =
                await loginResponse.Content
                    .ReadFromJsonAsync<LoginResponseDto>();

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    loginResult!.AccessToken);

            var response =
                await _client.GetAsync(
                    "/api/JobApplication");

            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
        }

        [Fact]
        public async Task CreateJobApplication_WithValidToken_ReturnsCreated()
        {
            var registerDto = new RegisterDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = $"test{Guid.NewGuid()}@example.com",
                Password = "Test123!"
            };

            await _client.PostAsJsonAsync(
                "/api/Auth/register",
                registerDto);

            var loginResponse =
                await _client.PostAsJsonAsync(
                    "/api/Auth/login",
                    new LoginDto
                    {
                        Email = registerDto.Email,
                        Password = registerDto.Password
                    });

            var loginResult =
                await loginResponse.Content
                    .ReadFromJsonAsync<LoginResponseDto>();

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    loginResult!.AccessToken);

            var companyResponse =
                await _client.PostAsJsonAsync(
                    "/api/Company",
                    new CreateCompanyDto
                    {
                        Name = $"Test Company {Guid.NewGuid()}",
                        Website = $"https://example-{Guid.NewGuid()}.com",
                        Location = "Dublin"
                    });

            Assert.Equal(
                HttpStatusCode.Created,
                companyResponse.StatusCode);

            var company =
                await companyResponse.Content
                    .ReadFromJsonAsync<CompanyDto>();

            Assert.NotNull(company);

            var categoryResponse =
                await _client.PostAsJsonAsync(
                    "/api/JobCategory",
                    new CreateJobCategoryDto
                    {
                        Name = $"Backend Development {Guid.NewGuid()}",
                        Description = "Backend development jobs"
                    });

            Assert.Equal(
                HttpStatusCode.Created,
                categoryResponse.StatusCode);

            var category =
                await categoryResponse.Content
                    .ReadFromJsonAsync<JobCategoryDto>();

            Assert.NotNull(category);

            var jobApplicationDto =
                new CreateJobApplicationDto
                {
                    Location = "Dublin",
                    Position = "Junior .NET Developer",
                    Salary = 45000,
                    CompanyId = company!.Id,
                    JobCategoryId = category!.Id,
                    WorkPlace = WorkPlace.Hybrid,
                    JobUrl = "https://example.com/job",
                    ApplicationDate = DateTime.UtcNow,
                    Status = JobApplicationStatus.Applied,
                    Description = "Integration test application"
                };

            var jobApplicationResponse =
                await _client.PostAsJsonAsync(
                    "/api/JobApplication",
                    jobApplicationDto);

            Assert.Equal(
                HttpStatusCode.Created,
                jobApplicationResponse.StatusCode);

            var jobApplication =
                await jobApplicationResponse.Content
                    .ReadFromJsonAsync<JobApplicationDto>();

            Assert.NotNull(jobApplication);
            Assert.True(jobApplication!.Id > 0);
            Assert.Equal(
                jobApplicationDto.Position,
                jobApplication.Position);
            Assert.Equal(
                company.Id,
                jobApplication.CompanyId);
            Assert.Equal(
                category.Id,
                jobApplication.JobCategoryId);
        }

        [Fact]
        public async Task GetJobApplications_ReturnsCreatedApplication()
        {
            var registerDto = new RegisterDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = $"test{Guid.NewGuid()}@example.com",
                Password = "Test123!"
            };

            await _client.PostAsJsonAsync(
                "/api/Auth/register",
                registerDto);

            var loginResponse =
                await _client.PostAsJsonAsync(
                    "/api/Auth/login",
                    new LoginDto
                    {
                        Email = registerDto.Email,
                        Password = registerDto.Password
                    });

            var loginResult =
                await loginResponse.Content
                    .ReadFromJsonAsync<LoginResponseDto>();

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    loginResult!.AccessToken);

            var companyResponse =
                await _client.PostAsJsonAsync(
                    "/api/Company",
                    new CreateCompanyDto
                    {
                        Name = $"Test Company {Guid.NewGuid()}",
                        Website = $"https://example-{Guid.NewGuid()}.com",
                        Location = "Dublin"
                    });

            var company =
                await companyResponse.Content
                    .ReadFromJsonAsync<CompanyDto>();

            var categoryResponse =
                await _client.PostAsJsonAsync(
                    "/api/JobCategory",
                    new CreateJobCategoryDto
                    {
                        Name = $"Backend {Guid.NewGuid()}",
                        Description = "Backend development"
                    });

            var category =
                await categoryResponse.Content
                    .ReadFromJsonAsync<JobCategoryDto>();

            var createResponse =
                await _client.PostAsJsonAsync(
                    "/api/JobApplication",
                    new CreateJobApplicationDto
                    {
                        Location = "Dublin",
                        Position = "Junior .NET Developer",
                        Salary = 45000,
                        CompanyId = company!.Id,
                        JobCategoryId = category!.Id,
                        WorkPlace = WorkPlace.Hybrid,
                        JobUrl = "https://example.com/job",
                        ApplicationDate = DateTime.UtcNow,
                        Status = JobApplicationStatus.Applied,
                        Description = "Integration test"
                    });

            var createdApplication =
                await createResponse.Content
                    .ReadFromJsonAsync<JobApplicationDto>();

            var getResponse =
                await _client.GetAsync(
                    "/api/JobApplication");

            Assert.Equal(
                HttpStatusCode.OK,
                getResponse.StatusCode);

            var applications =
                await getResponse.Content
                    .ReadFromJsonAsync<List<JobApplicationDto>>();

            Assert.NotNull(applications);

            Assert.Contains(
                applications!,
                x => x.Id == createdApplication!.Id);
        }

        [Fact]
        public async Task GetJobApplication_ByAnotherUser_ReturnsNotFound()
        {
            // Arrange - User A: Register and Login
            var userADto = new RegisterDto
            {
                FirstName = "User",
                LastName = "A",
                Email = $"usera{Guid.NewGuid()}@example.com",
                Password = "Test123!"
            };

            await _client.PostAsJsonAsync(
                "/api/Auth/register",
                userADto);

            var loginAResponse =
                await _client.PostAsJsonAsync(
                    "/api/Auth/login",
                    new LoginDto
                    {
                        Email = userADto.Email,
                        Password = userADto.Password
                    });

            var loginA =
                await loginAResponse.Content
                    .ReadFromJsonAsync<LoginResponseDto>();

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    loginA!.AccessToken);

            // Create Company
            var companyResponse =
                await _client.PostAsJsonAsync(
                    "/api/Company",
                    new CreateCompanyDto
                    {
                        Name = $"Company {Guid.NewGuid()}",
                        Website = $"https://company-{Guid.NewGuid()}.com",
                        Location = "Dublin"
                    });

            Assert.Equal(
                HttpStatusCode.Created,
                companyResponse.StatusCode);

            var company =
                await companyResponse.Content
                    .ReadFromJsonAsync<CompanyDto>();

            // Create JobCategory
            var categoryResponse =
                await _client.PostAsJsonAsync(
                    "/api/JobCategory",
                    new CreateJobCategoryDto
                    {
                        Name = $"Backend {Guid.NewGuid()}",
                        Description = "Backend development"
                    });

            Assert.Equal(
                HttpStatusCode.Created,
                categoryResponse.StatusCode);

            var category =
                await categoryResponse.Content
                    .ReadFromJsonAsync<JobCategoryDto>();

            // User A creates JobApplication
            var createApplicationResponse =
                await _client.PostAsJsonAsync(
                    "/api/JobApplication",
                    new CreateJobApplicationDto
                    {
                        Location = "Dublin",
                        Position = "Junior .NET Developer",
                        Salary = 45000,
                        CompanyId = company!.Id,
                        JobCategoryId = category!.Id,
                        WorkPlace = WorkPlace.Hybrid,
                        JobUrl = "https://example.com/job",
                        ApplicationDate = DateTime.UtcNow,
                        Status = JobApplicationStatus.Applied,
                        Description = "Created by User A"
                    });

            Assert.Equal(
                HttpStatusCode.Created,
                createApplicationResponse.StatusCode);

            var createdApplication =
                await createApplicationResponse.Content
                    .ReadFromJsonAsync<JobApplicationDto>();

            // Arrange - User B: Register and Login
            var userBDto = new RegisterDto
            {
                FirstName = "User",
                LastName = "B",
                Email = $"userb{Guid.NewGuid()}@example.com",
                Password = "Test123!"
            };

            await _client.PostAsJsonAsync(
                "/api/Auth/register",
                userBDto);

            var loginBResponse =
                await _client.PostAsJsonAsync(
                    "/api/Auth/login",
                    new LoginDto
                    {
                        Email = userBDto.Email,
                        Password = userBDto.Password
                    });

            var loginB =
                await loginBResponse.Content
                    .ReadFromJsonAsync<LoginResponseDto>();

            // Replace User A token with User B token
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    loginB!.AccessToken);

            // Act - User B tries to access User A's application
            var response =
                await _client.GetAsync(
                    $"/api/JobApplication/{createdApplication!.Id}");

            // Assert
            Assert.Equal(
                HttpStatusCode.NotFound,
                response.StatusCode);
        }

        [Fact]
        public async Task UpdateJobApplication_WithValidToken_UpdatesApplication()
        {
            var registerDto = new RegisterDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = $"test{Guid.NewGuid()}@example.com",
                Password = "Test123!"
            };

            await _client.PostAsJsonAsync(
                "/api/Auth/register",
                registerDto);

            var loginResponse = await _client.PostAsJsonAsync(
                "/api/Auth/login",
                new LoginDto
                {
                    Email = registerDto.Email,
                    Password = registerDto.Password
                });

            var loginResult =
                await loginResponse.Content
                    .ReadFromJsonAsync<LoginResponseDto>();

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    loginResult!.AccessToken);

            var companyResponse = await _client.PostAsJsonAsync(
                "/api/Company",
                new CreateCompanyDto
                {
                    Name = $"Company {Guid.NewGuid()}",
                    Website = $"https://company-{Guid.NewGuid()}.com",
                    Location = "Dublin"
                });

            var company =
                await companyResponse.Content
                    .ReadFromJsonAsync<CompanyDto>();

            var categoryResponse = await _client.PostAsJsonAsync(
                "/api/JobCategory",
                new CreateJobCategoryDto
                {
                    Name = $"Backend {Guid.NewGuid()}",
                    Description = "Backend development"
                });

            var category =
                await categoryResponse.Content
                    .ReadFromJsonAsync<JobCategoryDto>();

            var createResponse = await _client.PostAsJsonAsync(
                "/api/JobApplication",
                new CreateJobApplicationDto
                {
                    Location = "Dublin",
                    Position = "Junior .NET Developer",
                    Salary = 45000,
                    CompanyId = company!.Id,
                    JobCategoryId = category!.Id,
                    WorkPlace = WorkPlace.Hybrid,
                    JobUrl = "https://example.com/job",
                    ApplicationDate = DateTime.UtcNow,
                    Status = JobApplicationStatus.Applied,
                    Description = "Original description"
                });

            Assert.Equal(
                HttpStatusCode.Created,
                createResponse.StatusCode);

            var createdApplication =
                await createResponse.Content
                    .ReadFromJsonAsync<JobApplicationDto>();

            var updateDto = new UpdateJobApplicationDto
            {
                Location = "Cork",
                Position = "Mid-level .NET Developer",
                Salary = 60000,
                CompanyId = company.Id,
                JobCategoryId = category.Id,
                WorkPlace = WorkPlace.Remote,
                JobUrl = "https://example.com/updated-job",
                ApplicationDate = DateTime.UtcNow,
                Status = JobApplicationStatus.TechnicalInterview,
                Description = "Updated description"
            };

            var updateResponse = await _client.PutAsJsonAsync(
                $"/api/JobApplication/{createdApplication!.Id}",
                updateDto);

            Assert.Equal(
                HttpStatusCode.NoContent,
                updateResponse.StatusCode);

            var getResponse = await _client.GetAsync(
                $"/api/JobApplication/{createdApplication.Id}");

            Assert.Equal(
                HttpStatusCode.OK,
                getResponse.StatusCode);

            var updatedApplication =
                await getResponse.Content
                    .ReadFromJsonAsync<JobApplicationDto>();

            Assert.NotNull(updatedApplication);
            Assert.Equal(
                updateDto.Position,
                updatedApplication!.Position);
            Assert.Equal(
                updateDto.Location,
                updatedApplication.Location);
            Assert.Equal(
                updateDto.Salary,
                updatedApplication.Salary);
            Assert.Equal(
                updateDto.WorkPlace,
                updatedApplication.WorkPlace);
            Assert.Equal(
                updateDto.Status,
                updatedApplication.Status);
            Assert.Equal(
                updateDto.Description,
                updatedApplication.Description);
        }

        [Fact]
        public async Task DeleteJobApplication_WithValidToken_DeletesApplication()
        {
            var registerDto = new RegisterDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = $"test{Guid.NewGuid()}@example.com",
                Password = "Test123!"
            };

            await _client.PostAsJsonAsync(
                "/api/Auth/register",
                registerDto);

            var loginResponse = await _client.PostAsJsonAsync(
                "/api/Auth/login",
                new LoginDto
                {
                    Email = registerDto.Email,
                    Password = registerDto.Password
                });

            var loginResult =
                await loginResponse.Content
                    .ReadFromJsonAsync<LoginResponseDto>();

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    loginResult!.AccessToken);

            var companyResponse = await _client.PostAsJsonAsync(
                "/api/Company",
                new CreateCompanyDto
                {
                    Name = $"Company {Guid.NewGuid()}",
                    Website = $"https://company-{Guid.NewGuid()}.com",
                    Location = "Dublin"
                });

            var company =
                await companyResponse.Content
                    .ReadFromJsonAsync<CompanyDto>();

            var categoryResponse = await _client.PostAsJsonAsync(
                "/api/JobCategory",
                new CreateJobCategoryDto
                {
                    Name = $"Backend {Guid.NewGuid()}",
                    Description = "Backend development"
                });

            var category =
                await categoryResponse.Content
                    .ReadFromJsonAsync<JobCategoryDto>();

            var createResponse = await _client.PostAsJsonAsync(
                "/api/JobApplication",
                new CreateJobApplicationDto
                {
                    Location = "Dublin",
                    Position = "Junior .NET Developer",
                    Salary = 45000,
                    CompanyId = company!.Id,
                    JobCategoryId = category!.Id,
                    WorkPlace = WorkPlace.Hybrid,
                    JobUrl = "https://example.com/job",
                    ApplicationDate = DateTime.UtcNow,
                    Status = JobApplicationStatus.Applied,
                    Description = "Application to be deleted"
                });

            Assert.Equal(
                HttpStatusCode.Created,
                createResponse.StatusCode);

            var createdApplication =
                await createResponse.Content
                    .ReadFromJsonAsync<JobApplicationDto>();

            var deleteResponse = await _client.DeleteAsync(
                $"/api/JobApplication/{createdApplication!.Id}");

            Assert.Equal(
                HttpStatusCode.NoContent,
                deleteResponse.StatusCode);

            var getResponse = await _client.GetAsync(
                $"/api/JobApplication/{createdApplication.Id}");

            Assert.Equal(
                HttpStatusCode.NotFound,
                getResponse.StatusCode);
        }


    }
}