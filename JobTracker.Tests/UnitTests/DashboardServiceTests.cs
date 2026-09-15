using JobTracker.Application.DTOs.DashboardDtos;
using JobTracker.Application.Interfaces.Repository;
using JobTracker.Application.Interfaces.Service;
using JobTracker.Application.Services;
using Moq;

namespace JobTracker.Tests.UnitTests
{
    public class DashboardServiceTests
    {
        private readonly Mock<IJobApplicationRepository> _repositoryMock;
        private readonly Mock<IUserContext> _userContextMock;
        private readonly DashboardService _service;

        public DashboardServiceTests()
        {
            _repositoryMock = new Mock<IJobApplicationRepository>();
            _userContextMock = new Mock<IUserContext>();

            _service = new DashboardService(
                _repositoryMock.Object,
                _userContextMock.Object);
        }

        [Fact]
        public async Task GetDashboardAsync_ReturnsUserDashboard()
        {
            // Arrange
            var userId = 1L;

            var expectedDashboard = new DashboardDto
            {
                TotalApplications = 10,
                Saved = 2,
                Applied = 3,
                Interviews = 2,
                Offers = 1,
                Rejected = 1,
                Withdrawn = 1,
                Remote = 4,
                Hybrid = 3,
                OnSite = 3
            };

            _userContextMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _repositoryMock
                .Setup(x => x.GetDashboardAsync(userId))
                .ReturnsAsync(expectedDashboard);

            // Act
            var result = await _service.GetDashboardAsync();

            // Assert
            Assert.NotNull(result);

            Assert.Equal(
                expectedDashboard.TotalApplications,
                result.TotalApplications);

            Assert.Equal(
                expectedDashboard.Saved,
                result.Saved);

            Assert.Equal(
                expectedDashboard.Applied,
                result.Applied);

            Assert.Equal(
                expectedDashboard.Interviews,
                result.Interviews);

            Assert.Equal(
                expectedDashboard.Offers,
                result.Offers);

            Assert.Equal(
                expectedDashboard.Rejected,
                result.Rejected);

            Assert.Equal(
                expectedDashboard.Withdrawn,
                result.Withdrawn);

            Assert.Equal(
                expectedDashboard.Remote,
                result.Remote);

            Assert.Equal(
                expectedDashboard.Hybrid,
                result.Hybrid);

            Assert.Equal(
                expectedDashboard.OnSite,
                result.OnSite);

            _repositoryMock.Verify(
                x => x.GetDashboardAsync(userId),
                Times.Once);
        }
    }
}