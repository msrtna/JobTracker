using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using JobTracker.Application.DTOs.AuthDtos.LoginDtos;
using JobTracker.Application.DTOs.AuthDtos.RegisterDtos;
using JobTracker.Application.DTOs.Common;
using JobTracker.Application.DTOs.CompanyDtos;
using JobTracker.Application.DTOs.InterviewDtos;
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
                    .ReadFromJsonAsync<PagedResultDto<JobApplicationDto>>();

            Assert.NotNull(applications);

            Assert.Contains(
                applications!.Items,
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

        [Fact]
        public async Task UpdateJobApplication_ByAnotherUser_ReturnsNotFound()
        {
            // User A: Register
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

            // User A: Login
            var loginAResponse = await _client.PostAsJsonAsync(
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
                new AuthenticationHeaderValue(
                    "Bearer",
                    loginA!.AccessToken);

            // Create Company
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

            // Create JobCategory
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

            // User A: Create JobApplication
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
                    Description = "Created by User A"
                });

            var application =
                await createResponse.Content
                    .ReadFromJsonAsync<JobApplicationDto>();

            // User B: Register
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

            // User B: Login
            var loginBResponse = await _client.PostAsJsonAsync(
                "/api/Auth/login",
                new LoginDto
                {
                    Email = userBDto.Email,
                    Password = userBDto.Password
                });

            var loginB =
                await loginBResponse.Content
                    .ReadFromJsonAsync<LoginResponseDto>();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    loginB!.AccessToken);

            // User B: Try to update User A's application
            var updateDto = new UpdateJobApplicationDto
            {
                Location = "Cork",
                Position = "Senior .NET Developer",
                Salary = 60000,
                CompanyId = company.Id,
                JobCategoryId = category.Id,
                WorkPlace = WorkPlace.Remote,
                JobUrl = "https://example.com/updated-job",
                ApplicationDate = DateTime.UtcNow,
                Status = JobApplicationStatus.TechnicalInterview,
                Description = "Updated by User B"
            };

            var updateResponse = await _client.PutAsJsonAsync(
                $"/api/JobApplication/{application!.Id}",
                updateDto);

            Assert.Equal(
                HttpStatusCode.NotFound,
                updateResponse.StatusCode);
        }

        [Fact]
        public async Task DeleteJobApplication_ByAnotherUser_ReturnsNotFound()
        {
            // User A: Register
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

            // User A: Login
            var loginAResponse = await _client.PostAsJsonAsync(
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
                new AuthenticationHeaderValue(
                    "Bearer",
                    loginA!.AccessToken);

            // Create Company
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

            // Create JobCategory
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

            // User A: Create JobApplication
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
                    Description = "Created by User A"
                });

            var application =
                await createResponse.Content
                    .ReadFromJsonAsync<JobApplicationDto>();

            // User B: Register
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

            // User B: Login
            var loginBResponse = await _client.PostAsJsonAsync(
                "/api/Auth/login",
                new LoginDto
                {
                    Email = userBDto.Email,
                    Password = userBDto.Password
                });

            var loginB =
                await loginBResponse.Content
                    .ReadFromJsonAsync<LoginResponseDto>();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    loginB!.AccessToken);

            // User B: Try to delete User A's application
            var deleteResponse = await _client.DeleteAsync(
                $"/api/JobApplication/{application!.Id}");

            Assert.Equal(
                HttpStatusCode.NotFound,
                deleteResponse.StatusCode);
        }

        [Fact]
        public async Task GetJobApplications_WithoutAuthentication_ReturnsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.GetAsync(
                "/api/JobApplication");

            Assert.Equal(
                HttpStatusCode.Unauthorized,
                response.StatusCode);
        }

        [Fact]
        public async Task GetCompanies_WithoutAuthentication_ReturnsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.GetAsync(
                "/api/Company");

            Assert.Equal(
                HttpStatusCode.Unauthorized,
                response.StatusCode);
        }

        [Fact]
        public async Task GetJobCategories_WithoutAuthentication_ReturnsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.GetAsync(
                "/api/JobCategory");

            Assert.Equal(
                HttpStatusCode.Unauthorized,
                response.StatusCode);
        }

        [Fact]
        public async Task CreateInterview_WithValidToken_ReturnsCreated()
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
                new AuthenticationHeaderValue(
                    "Bearer",
                    loginResult!.AccessToken);

            var companyResponse =
                await _client.PostAsJsonAsync(
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

            var jobApplicationResponse =
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

            Assert.Equal(
                HttpStatusCode.Created,
                jobApplicationResponse.StatusCode);

            var jobApplication =
                await jobApplicationResponse.Content
                    .ReadFromJsonAsync<JobApplicationDto>();

            var interviewDto = new CreateInterviewDto
            {
                JobApplicationId = jobApplication!.Id,
                InterviewDate = DateTime.UtcNow.AddDays(2),
                InterviewType = InterviewType.TechnicalInterview,
                Notes = "Technical interview"
            };

            var response =
                await _client.PostAsJsonAsync(
                    "/api/Interview",
                    interviewDto);

            Assert.Equal(
                HttpStatusCode.Created,
                response.StatusCode);

            var interview =
                await response.Content
                    .ReadFromJsonAsync<InterviewDto>();

            Assert.NotNull(interview);
            Assert.True(interview!.Id > 0);
            Assert.Equal(
                interviewDto.JobApplicationId,
                interview.JobApplicationId);
            Assert.Equal(
                interviewDto.InterviewType,
                interview.InterviewType);
            Assert.Equal(
                interviewDto.Notes,
                interview.Notes);
        }

        [Fact]
        public async Task GetInterviews_ReturnsCreatedInterview()
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
                new AuthenticationHeaderValue(
                    "Bearer",
                    loginResult!.AccessToken);

            var companyResponse =
                await _client.PostAsJsonAsync(
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

            var jobApplicationResponse =
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

            var jobApplication =
                await jobApplicationResponse.Content
                    .ReadFromJsonAsync<JobApplicationDto>();

            var createResponse =
                await _client.PostAsJsonAsync(
                    "/api/Interview",
                    new CreateInterviewDto
                    {
                        JobApplicationId = jobApplication!.Id,
                        InterviewDate = DateTime.UtcNow.AddDays(2),
                        InterviewType = InterviewType.HRInterview,
                        Notes = "HR interview"
                    });

            Assert.Equal(
                HttpStatusCode.Created,
                createResponse.StatusCode);

            var createdInterview =
                await createResponse.Content
                    .ReadFromJsonAsync<InterviewDto>();

            var getResponse =
                await _client.GetAsync(
                    "/api/Interview");

            Assert.Equal(
                HttpStatusCode.OK,
                getResponse.StatusCode);

            var interviews =
                await getResponse.Content
                    .ReadFromJsonAsync<List<InterviewDto>>();

            Assert.NotNull(interviews);

            Assert.Contains(
                interviews!,
                x => x.Id == createdInterview!.Id);
        }

        [Fact]
        public async Task GetInterview_ByAnotherUser_ReturnsNotFound()
        {
            // User A: Register and Login
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
                new AuthenticationHeaderValue(
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

            var category =
                await categoryResponse.Content
                    .ReadFromJsonAsync<JobCategoryDto>();

            // User A: Create JobApplication
            var applicationResponse =
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

            var application =
                await applicationResponse.Content
                    .ReadFromJsonAsync<JobApplicationDto>();

            // User A: Create Interview
            var interviewResponse =
                await _client.PostAsJsonAsync(
                    "/api/Interview",
                    new CreateInterviewDto
                    {
                        JobApplicationId = application!.Id,
                        InterviewDate = DateTime.UtcNow.AddDays(2),
                        InterviewType = InterviewType.TechnicalInterview,
                        Notes = "Technical interview"
                    });

            var interview =
                await interviewResponse.Content
                    .ReadFromJsonAsync<InterviewDto>();

            // User B: Register and Login
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

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    loginB!.AccessToken);

            // User B tries to access User A's Interview
            var response =
                await _client.GetAsync(
                    $"/api/Interview/{interview!.Id}");

            Assert.Equal(
                HttpStatusCode.NotFound,
                response.StatusCode);
        }

        [Fact]
        public async Task UpdateInterview_WithValidToken_UpdatesInterview()
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
                new AuthenticationHeaderValue(
                    "Bearer",
                    loginResult!.AccessToken);

            var companyResponse =
                await _client.PostAsJsonAsync(
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

            var applicationResponse =
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

            var application =
                await applicationResponse.Content
                    .ReadFromJsonAsync<JobApplicationDto>();

            var createResponse =
                await _client.PostAsJsonAsync(
                    "/api/Interview",
                    new CreateInterviewDto
                    {
                        JobApplicationId = application!.Id,
                        InterviewDate = DateTime.UtcNow.AddDays(2),
                        InterviewType = InterviewType.HRInterview,
                        Notes = "Original notes"
                    });

            var createdInterview =
                await createResponse.Content
                    .ReadFromJsonAsync<InterviewDto>();

            var updateDto = new UpdateInterviewDto
            {
                JobApplicationId = application.Id,
                InterviewDate = DateTime.UtcNow.AddDays(5),
                InterviewType = InterviewType.FinalInterview,
                Notes = "Updated notes"
            };

            var updateResponse =
                await _client.PutAsJsonAsync(
                    $"/api/Interview/{createdInterview!.Id}",
                    updateDto);

            Assert.Equal(
                HttpStatusCode.NoContent,
                updateResponse.StatusCode);

            var getResponse =
                await _client.GetAsync(
                    $"/api/Interview/{createdInterview.Id}");

            Assert.Equal(
                HttpStatusCode.OK,
                getResponse.StatusCode);

            var updatedInterview =
                await getResponse.Content
                    .ReadFromJsonAsync<InterviewDto>();

            Assert.NotNull(updatedInterview);

            Assert.Equal(
                updateDto.JobApplicationId,
                updatedInterview!.JobApplicationId);

            Assert.Equal(
                updateDto.InterviewType,
                updatedInterview.InterviewType);

            Assert.Equal(
                updateDto.Notes,
                updatedInterview.Notes);
        }

        [Fact]
        public async Task DeleteInterview_WithValidToken_DeletesInterview()
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
                new AuthenticationHeaderValue(
                    "Bearer",
                    loginResult!.AccessToken);

            var companyResponse =
                await _client.PostAsJsonAsync(
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

            var applicationResponse =
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

            var application =
                await applicationResponse.Content
                    .ReadFromJsonAsync<JobApplicationDto>();

            var createResponse =
                await _client.PostAsJsonAsync(
                    "/api/Interview",
                    new CreateInterviewDto
                    {
                        JobApplicationId = application!.Id,
                        InterviewDate = DateTime.UtcNow.AddDays(2),
                        InterviewType = InterviewType.TechnicalInterview,
                        Notes = "Interview to be deleted"
                    });

            var createdInterview =
                await createResponse.Content
                    .ReadFromJsonAsync<InterviewDto>();

            var deleteResponse =
                await _client.DeleteAsync(
                    $"/api/Interview/{createdInterview!.Id}");

            Assert.Equal(
                HttpStatusCode.NoContent,
                deleteResponse.StatusCode);

            var getResponse =
                await _client.GetAsync(
                    $"/api/Interview/{createdInterview.Id}");

            Assert.Equal(
                HttpStatusCode.NotFound,
                getResponse.StatusCode);
        }

        [Fact]
        public async Task GetInterviews_WithoutAuthentication_ReturnsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var response =
                await _client.GetAsync(
                    "/api/Interview");

            Assert.Equal(
                HttpStatusCode.Unauthorized,
                response.StatusCode);
        }

        [Fact]
        public async Task CreateInterview_WithAnotherUsersJobApplication_ReturnsNotFound()
        {
            // User A: Register and Login
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
                new AuthenticationHeaderValue(
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

            var category =
                await categoryResponse.Content
                    .ReadFromJsonAsync<JobCategoryDto>();

            // User A: Create JobApplication
            var applicationResponse =
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

            var application =
                await applicationResponse.Content
                    .ReadFromJsonAsync<JobApplicationDto>();

            // User B: Register and Login
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

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    loginB!.AccessToken);

            // User B: Try to create Interview for User A's JobApplication
            var response =
                await _client.PostAsJsonAsync(
                    "/api/Interview",
                    new CreateInterviewDto
                    {
                        JobApplicationId = application!.Id,
                        InterviewDate = DateTime.UtcNow.AddDays(2),
                        InterviewType = InterviewType.TechnicalInterview,
                        Notes = "Should not be created"
                    });

            Assert.Equal(
                HttpStatusCode.NotFound,
                response.StatusCode);
        }

        [Fact]
        public async Task UpdateInterview_WithAnotherUsersJobApplication_ReturnsNotFound()
        {
            // User A: Register and Login
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
                new AuthenticationHeaderValue(
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

            var category =
                await categoryResponse.Content
                    .ReadFromJsonAsync<JobCategoryDto>();

            // User A: Create first JobApplication
            var applicationAResponse =
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
                        JobUrl = "https://example.com/job-a",
                        ApplicationDate = DateTime.UtcNow,
                        Status = JobApplicationStatus.Applied,
                        Description = "User A application"
                    });

            var applicationA =
                await applicationAResponse.Content
                    .ReadFromJsonAsync<JobApplicationDto>();

            // User A: Create Interview
            var interviewResponse =
                await _client.PostAsJsonAsync(
                    "/api/Interview",
                    new CreateInterviewDto
                    {
                        JobApplicationId = applicationA!.Id,
                        InterviewDate = DateTime.UtcNow.AddDays(2),
                        InterviewType = InterviewType.HRInterview,
                        Notes = "Original interview"
                    });

            Assert.Equal(
                HttpStatusCode.Created,
                interviewResponse.StatusCode);

            var interview =
                await interviewResponse.Content
                    .ReadFromJsonAsync<InterviewDto>();

            // User B: Register and Login
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

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    loginB!.AccessToken);

            // User B: Create own JobApplication
            var applicationBResponse =
                await _client.PostAsJsonAsync(
                    "/api/JobApplication",
                    new CreateJobApplicationDto
                    {
                        Location = "Cork",
                        Position = "Backend Developer",
                        Salary = 55000,
                        CompanyId = company.Id,
                        JobCategoryId = category.Id,
                        WorkPlace = WorkPlace.Remote,
                        JobUrl = "https://example.com/job-b",
                        ApplicationDate = DateTime.UtcNow,
                        Status = JobApplicationStatus.Applied,
                        Description = "User B application"
                    });

            var applicationB =
                await applicationBResponse.Content
                    .ReadFromJsonAsync<JobApplicationDto>();

            // User B: Try to update User A's Interview
            // and assign it to User B's JobApplication
            var updateDto = new UpdateInterviewDto
            {
                JobApplicationId = applicationB!.Id,
                InterviewDate = DateTime.UtcNow.AddDays(5),
                InterviewType = InterviewType.FinalInterview,
                Notes = "Should not be allowed"
            };

            var response =
                await _client.PutAsJsonAsync(
                    $"/api/Interview/{interview!.Id}",
                    updateDto);

            Assert.Equal(
                HttpStatusCode.NotFound,
                response.StatusCode);
        }

        [Fact]
        public async Task GetJobApplications_WithPaginationAndSort_ReturnsExpectedResult()
        {
            // Arrange
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
                new AuthenticationHeaderValue(
                    "Bearer",
                    loginResult!.AccessToken);

            // Create Company
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

            // Create Job Category
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

            // Create first application
            await _client.PostAsJsonAsync(
                "/api/JobApplication",
                new CreateJobApplicationDto
                {
                    Location = "Dublin",
                    Position = "First Application",
                    Salary = 40000,
                    CompanyId = company!.Id,
                    JobCategoryId = category!.Id,
                    WorkPlace = WorkPlace.Hybrid,
                    JobUrl = "https://example.com/job1",
                    ApplicationDate = new DateTime(2026, 1, 10),
                    Status = JobApplicationStatus.Applied
                });

            // Create second application
            await _client.PostAsJsonAsync(
                "/api/JobApplication",
                new CreateJobApplicationDto
                {
                    Location = "Dublin",
                    Position = "Second Application",
                    Salary = 45000,
                    CompanyId = company.Id,
                    JobCategoryId = category.Id,
                    WorkPlace = WorkPlace.Remote,
                    JobUrl = "https://example.com/job2",
                    ApplicationDate = new DateTime(2026, 2, 10),
                    Status = JobApplicationStatus.Applied
                });

            // Create third application
            await _client.PostAsJsonAsync(
                "/api/JobApplication",
                new CreateJobApplicationDto
                {
                    Location = "Dublin",
                    Position = "Third Application",
                    Salary = 50000,
                    CompanyId = company.Id,
                    JobCategoryId = category.Id,
                    WorkPlace = WorkPlace.OnSite,
                    JobUrl = "https://example.com/job3",
                    ApplicationDate = new DateTime(2026, 3, 10),
                    Status = JobApplicationStatus.Applied
                });

            // Act
            var response = await _client.GetAsync(
                "/api/JobApplication" +
                "?page=1" +
                "&pageSize=2" +
                "&sortBy=applicationdate" +
                "&sortDescending=true");

            // Assert
            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);

            var result =
                await response.Content
                    .ReadFromJsonAsync<PagedResultDto<JobApplicationDto>>();

            Assert.NotNull(result);

            Assert.Equal(1, result!.PageNumber);
            Assert.Equal(2, result.PageSize);
            Assert.Equal(3, result.TotalCount);
            Assert.Equal(2, result.TotalPages);

            Assert.True(result.HasNext);
            Assert.False(result.HasPrevious);

            Assert.Equal(2, result.Items.Count);

            Assert.Equal(
                "Third Application",
                result.Items[0].Position);

            Assert.Equal(
                "Second Application",
                result.Items[1].Position);
        }

        [Fact]
        public async Task GetJobApplications_WithSearch_ReturnsMatchingApplications()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                FirstName = "Search",
                LastName = "Test",
                Email = $"search{Guid.NewGuid()}@example.com",
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
                new AuthenticationHeaderValue(
                    "Bearer",
                    loginResult!.AccessToken);

            // Company 1
            var company1Response =
                await _client.PostAsJsonAsync(
                    "/api/Company",
                    new CreateCompanyDto
                    {
                        Name = $"Backend Company {Guid.NewGuid()}",
                        Website = $"https://backend-{Guid.NewGuid()}.com",
                        Location = "Dublin"
                    });

            var company1 =
                await company1Response.Content
                    .ReadFromJsonAsync<CompanyDto>();

            Assert.NotNull(company1);

            // Company 2
            var company2Response =
                await _client.PostAsJsonAsync(
                    "/api/Company",
                    new CreateCompanyDto
                    {
                        Name = $"Frontend Company {Guid.NewGuid()}",
                        Website = $"https://frontend-{Guid.NewGuid()}.com",
                        Location = "Dublin"
                    });

            var company2 =
                await company2Response.Content
                    .ReadFromJsonAsync<CompanyDto>();

            Assert.NotNull(company2);

            // Category 1
            var category1Response =
                await _client.PostAsJsonAsync(
                    "/api/JobCategory",
                    new CreateJobCategoryDto
                    {
                        Name = $"Backend Development {Guid.NewGuid()}",
                        Description = "Backend jobs"
                    });

            var category1 =
                await category1Response.Content
                    .ReadFromJsonAsync<JobCategoryDto>();

            Assert.NotNull(category1);

            // Category 2
            var category2Response =
                await _client.PostAsJsonAsync(
                    "/api/JobCategory",
                    new CreateJobCategoryDto
                    {
                        Name = $"Frontend Development {Guid.NewGuid()}",
                        Description = "Frontend jobs"
                    });

            var category2 =
                await category2Response.Content
                    .ReadFromJsonAsync<JobCategoryDto>();

            Assert.NotNull(category2);

            // Application 1
            // Search should match Position
            await _client.PostAsJsonAsync(
                "/api/JobApplication",
                new CreateJobApplicationDto
                {
                    Location = "Dublin",
                    Position = "Backend Developer",
                    Salary = 45000,
                    CompanyId = company2!.Id,
                    JobCategoryId = category2!.Id,
                    WorkPlace = WorkPlace.Hybrid,
                    ApplicationDate = DateTime.UtcNow,
                    Status = JobApplicationStatus.Applied
                });

            // Application 2
            // Search should match Company.Name
            await _client.PostAsJsonAsync(
                "/api/JobApplication",
                new CreateJobApplicationDto
                {
                    Location = "Dublin",
                    Position = "Software Developer",
                    Salary = 46000,
                    CompanyId = company1.Id,
                    JobCategoryId = category2.Id,
                    WorkPlace = WorkPlace.Remote,
                    ApplicationDate = DateTime.UtcNow,
                    Status = JobApplicationStatus.Applied
                });

            // Application 3
            // Search should match JobCategory.Name
            await _client.PostAsJsonAsync(
                "/api/JobApplication",
                new CreateJobApplicationDto
                {
                    Location = "Dublin",
                    Position = "Software Engineer",
                    Salary = 47000,
                    CompanyId = company2.Id,
                    JobCategoryId = category1.Id,
                    WorkPlace = WorkPlace.OnSite,
                    ApplicationDate = DateTime.UtcNow,
                    Status = JobApplicationStatus.Applied
                });

            // Act
            var response = await _client.GetAsync(
                "/api/JobApplication?search=Backend");

            // Assert
            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);

            var result =
                await response.Content
                    .ReadFromJsonAsync<PagedResultDto<JobApplicationDto>>();

            Assert.NotNull(result);

            Assert.Equal(3, result!.TotalCount);
            Assert.Equal(3, result.Items.Count);

            Assert.Contains(
                result.Items,
                x => x.Position == "Backend Developer");

            Assert.Contains(
                result.Items,
                x => x.Position == "Software Developer");

            Assert.Contains(
                result.Items,
                x => x.Position == "Software Engineer");
        }

        [Fact]
        public async Task GetJobApplications_WithFilters_ReturnsMatchingApplications()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                FirstName = "Filter",
                LastName = "Test",
                Email = $"filter{Guid.NewGuid()}@example.com",
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
                new AuthenticationHeaderValue(
                    "Bearer",
                    loginResult!.AccessToken);

            // Company 1
            var company1Response =
                await _client.PostAsJsonAsync(
                    "/api/Company",
                    new CreateCompanyDto
                    {
                        Name = $"Filter Company 1 {Guid.NewGuid()}",
                        Website = $"https://filter1-{Guid.NewGuid()}.com",
                        Location = "Dublin"
                    });

            var company1 =
                await company1Response.Content
                    .ReadFromJsonAsync<CompanyDto>();

            Assert.NotNull(company1);

            // Company 2
            var company2Response =
                await _client.PostAsJsonAsync(
                    "/api/Company",
                    new CreateCompanyDto
                    {
                        Name = $"Filter Company 2 {Guid.NewGuid()}",
                        Website = $"https://filter2-{Guid.NewGuid()}.com",
                        Location = "Cork"
                    });

            var company2 =
                await company2Response.Content
                    .ReadFromJsonAsync<CompanyDto>();

            Assert.NotNull(company2);

            // Category 1
            var category1Response =
                await _client.PostAsJsonAsync(
                    "/api/JobCategory",
                    new CreateJobCategoryDto
                    {
                        Name = $"Filter Category 1 {Guid.NewGuid()}",
                        Description = "Backend jobs"
                    });

            var category1 =
                await category1Response.Content
                    .ReadFromJsonAsync<JobCategoryDto>();

            Assert.NotNull(category1);

            // Category 2
            var category2Response =
                await _client.PostAsJsonAsync(
                    "/api/JobCategory",
                    new CreateJobCategoryDto
                    {
                        Name = $"Filter Category 2 {Guid.NewGuid()}",
                        Description = "Frontend jobs"
                    });

            var category2 =
                await category2Response.Content
                    .ReadFromJsonAsync<JobCategoryDto>();

            Assert.NotNull(category2);

            // Application 1
            await _client.PostAsJsonAsync(
                "/api/JobApplication",
                new CreateJobApplicationDto
                {
                    Location = "Dublin",
                    Position = "Backend Developer",
                    Salary = 45000,
                    CompanyId = company1!.Id,
                    JobCategoryId = category1!.Id,
                    WorkPlace = WorkPlace.Remote,
                    ApplicationDate = new DateTime(2026, 1, 10),
                    Status = JobApplicationStatus.Applied
                });

            // Application 2
            await _client.PostAsJsonAsync(
                "/api/JobApplication",
                new CreateJobApplicationDto
                {
                    Location = "Cork",
                    Position = "Software Developer",
                    Salary = 50000,
                    CompanyId = company1.Id,
                    JobCategoryId = category2!.Id,
                    WorkPlace = WorkPlace.Hybrid,
                    ApplicationDate = new DateTime(2026, 2, 10),
                    Status = JobApplicationStatus.HRScreening
                });

            // Application 3
            await _client.PostAsJsonAsync(
                "/api/JobApplication",
                new CreateJobApplicationDto
                {
                    Location = "Dublin",
                    Position = "Senior Developer",
                    Salary = 60000,
                    CompanyId = company2!.Id,
                    JobCategoryId = category1.Id,
                    WorkPlace = WorkPlace.OnSite,
                    ApplicationDate = new DateTime(2026, 3, 10),
                    Status = JobApplicationStatus.Applied
                });

            // --------------------------------------------------
            // Filter by Status
            // --------------------------------------------------

            var statusResponse =
                await _client.GetAsync(
                    "/api/JobApplication?status=2");

            Assert.Equal(
                HttpStatusCode.OK,
                statusResponse.StatusCode);

            var statusResult =
                await statusResponse.Content
                    .ReadFromJsonAsync<PagedResultDto<JobApplicationDto>>();

            Assert.NotNull(statusResult);
            Assert.Equal(2, statusResult!.TotalCount);
            Assert.All(
                statusResult.Items,
                x => Assert.Equal(
                    JobApplicationStatus.Applied,
                    x.Status));

            // --------------------------------------------------
            // Filter by WorkPlace
            // --------------------------------------------------

            var workPlaceResponse =
                await _client.GetAsync(
                    "/api/JobApplication?workPlace=3");

            Assert.Equal(
                HttpStatusCode.OK,
                workPlaceResponse.StatusCode);

            var workPlaceResult =
                await workPlaceResponse.Content
                    .ReadFromJsonAsync<PagedResultDto<JobApplicationDto>>();

            Assert.NotNull(workPlaceResult);
            Assert.Equal(1, workPlaceResult!.TotalCount);
            Assert.All(
                workPlaceResult.Items,
                x => Assert.Equal(
                    WorkPlace.Remote,
                    x.WorkPlace));

            // --------------------------------------------------
            // Filter by CompanyId
            // --------------------------------------------------

            var companyResponse =
                await _client.GetAsync(
                    $"/api/JobApplication?companyId={company1.Id}");

            Assert.Equal(
                HttpStatusCode.OK,
                companyResponse.StatusCode);

            var companyResult =
                await companyResponse.Content
                    .ReadFromJsonAsync<PagedResultDto<JobApplicationDto>>();

            Assert.NotNull(companyResult);
            Assert.Equal(2, companyResult!.TotalCount);
            Assert.All(
                companyResult.Items,
                x => Assert.Equal(company1.Id, x.CompanyId));

            // --------------------------------------------------
            // Filter by JobCategoryId
            // --------------------------------------------------

            var categoryResponse =
                await _client.GetAsync(
                    $"/api/JobApplication?jobCategoryId={category1.Id}");

            Assert.Equal(
                HttpStatusCode.OK,
                categoryResponse.StatusCode);

            var categoryResult =
                await categoryResponse.Content
                    .ReadFromJsonAsync<PagedResultDto<JobApplicationDto>>();

            Assert.NotNull(categoryResult);
            Assert.Equal(2, categoryResult!.TotalCount);
            Assert.All(
                categoryResult.Items,
                x => Assert.Equal(category1.Id, x.JobCategoryId));

            // --------------------------------------------------
            // Filter by ApplicationDateFrom
            // --------------------------------------------------

            var dateFromResponse =
                await _client.GetAsync(
                    "/api/JobApplication?applicationDateFrom=2026-02-01");

            Assert.Equal(
                HttpStatusCode.OK,
                dateFromResponse.StatusCode);

            var dateFromResult =
                await dateFromResponse.Content
                    .ReadFromJsonAsync<PagedResultDto<JobApplicationDto>>();

            Assert.NotNull(dateFromResult);
            Assert.Equal(2, dateFromResult!.TotalCount);

            // --------------------------------------------------
            // Filter by ApplicationDateTo
            // --------------------------------------------------

            var dateToResponse =
                await _client.GetAsync(
                    "/api/JobApplication?applicationDateTo=2026-02-15");

            Assert.Equal(
                HttpStatusCode.OK,
                dateToResponse.StatusCode);

            var dateToResult =
                await dateToResponse.Content
                    .ReadFromJsonAsync<PagedResultDto<JobApplicationDto>>();

            Assert.NotNull(dateToResult);
            Assert.Equal(2, dateToResult!.TotalCount);
        }

        [Fact]
        public async Task GetJobApplications_WithCombinedQuery_ReturnsExpectedResult()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                FirstName = "Combined",
                LastName = "Test",
                Email = $"combined{Guid.NewGuid()}@example.com",
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
                new AuthenticationHeaderValue(
                    "Bearer",
                    loginResult!.AccessToken);

            var companyResponse =
                await _client.PostAsJsonAsync(
                    "/api/Company",
                    new CreateCompanyDto
                    {
                        Name = $"Combined Company {Guid.NewGuid()}",
                        Website = $"https://combined-{Guid.NewGuid()}.com",
                        Location = "Dublin"
                    });

            var company =
                await companyResponse.Content
                    .ReadFromJsonAsync<CompanyDto>();

            Assert.NotNull(company);

            var categoryResponse =
                await _client.PostAsJsonAsync(
                    "/api/JobCategory",
                    new CreateJobCategoryDto
                    {
                        Name = $"Combined Category {Guid.NewGuid()}",
                        Description = "Backend development"
                    });

            var category =
                await categoryResponse.Content
                    .ReadFromJsonAsync<JobCategoryDto>();

            Assert.NotNull(category);

            // Application 1 - should match
            await _client.PostAsJsonAsync(
                "/api/JobApplication",
                new CreateJobApplicationDto
                {
                    Location = "Dublin",
                    Position = "Junior Backend Developer",
                    Salary = 45000,
                    CompanyId = company!.Id,
                    JobCategoryId = category!.Id,
                    WorkPlace = WorkPlace.Remote,
                    ApplicationDate = new DateTime(2026, 1, 10),
                    Status = JobApplicationStatus.Applied
                });

            // Application 2 - should match
            await _client.PostAsJsonAsync(
                "/api/JobApplication",
                new CreateJobApplicationDto
                {
                    Location = "Dublin",
                    Position = "Senior Backend Developer",
                    Salary = 65000,
                    CompanyId = company.Id,
                    JobCategoryId = category.Id,
                    WorkPlace = WorkPlace.Hybrid,
                    ApplicationDate = new DateTime(2026, 2, 10),
                    Status = JobApplicationStatus.Applied
                });

            // Application 3 - should NOT match because status is HRScreening
            await _client.PostAsJsonAsync(
                "/api/JobApplication",
                new CreateJobApplicationDto
                {
                    Location = "Dublin",
                    Position = "Backend Developer",
                    Salary = 55000,
                    CompanyId = company.Id,
                    JobCategoryId = category.Id,
                    WorkPlace = WorkPlace.Remote,
                    ApplicationDate = new DateTime(2026, 3, 10),
                    Status = JobApplicationStatus.HRScreening
                });

            // Application 4 - should NOT match because Position does not contain Backend
            await _client.PostAsJsonAsync(
                "/api/JobApplication",
                new CreateJobApplicationDto
                {
                    Location = "Dublin",
                    Position = "Junior Frontend Developer",
                    Salary = 40000,
                    CompanyId = company.Id,
                    JobCategoryId = category.Id,
                    WorkPlace = WorkPlace.Remote,
                    ApplicationDate = new DateTime(2026, 4, 10),
                    Status = JobApplicationStatus.Applied
                });

            // Act
            var response =
                await _client.GetAsync(
                    "/api/JobApplication" +
                    "?search=Backend" +
                    "&status=2" +
                    "&sortBy=salary" +
                    "&sortDescending=true" +
                    "&page=1" +
                    "&pageSize=1");

            // Assert
            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);

            var result =
                await response.Content
                    .ReadFromJsonAsync<PagedResultDto<JobApplicationDto>>();

            Assert.NotNull(result);

            Assert.Equal(1, result!.PageNumber);
            Assert.Equal(1, result.PageSize);

            // Only Application 1 and 2 match all conditions.
            Assert.Equal(2, result.TotalCount);

            Assert.Equal(2, result.TotalPages);

            Assert.True(result.HasNext);
            Assert.False(result.HasPrevious);

            // Salary DESC => Application 2 must be first.
            Assert.Single(result.Items);

            Assert.Equal(
                "Senior Backend Developer",
                result.Items[0].Position);

            Assert.Equal(
                65000,
                result.Items[0].Salary);

            Assert.Equal(
                JobApplicationStatus.Applied,
                result.Items[0].Status);
        }
    }
}