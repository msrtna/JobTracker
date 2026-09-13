using AutoMapper;
using JobTracker.Application.DTOs.JobApplicationDtos;
using JobTracker.Application.Exceptions;
using JobTracker.Application.Interfaces.Repository;
using JobTracker.Application.Interfaces.Service;
using JobTracker.Application.Services;
using JobTracker.Domain.Entities;
using JobTracker.Domain.Enums;
using Moq;

namespace JobTracker.Tests.UnitTests
{
    public class JobApplicationServiceTests
    {
        private readonly Mock<IJobApplicationRepository> _repositoryMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ICompanyRepository> _companyRepositoryMock = new();
        private readonly Mock<IJobCategoryRepository> _jobCategoryRepositoryMock = new();
        private readonly Mock<IUserContext> _userContextMock = new();

        [Fact]
        public async Task GetAllAsync_ReturnsCurrentUserJobApplications()
        {
            // Arrange
            var userId = 1L;

            var applications = new List<JobApplication>
            {
                new JobApplication
                {
                    Id = 1,
                    UserId = userId,
                    Position = "Backend Developer"
                },
                new JobApplication
                {
                    Id = 2,
                    UserId = userId,
                    Position = "C# Developer"
                }
            };

            _userContextMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _repositoryMock
                .Setup(x => x.GetAllByUserIdAsync(userId))
                .ReturnsAsync(applications);

            var dtoList = new List<JobApplicationDto>
            {
                new JobApplicationDto
                {
                    Id = 1,
                    UserId = userId,
                    Position = "Backend Developer"
                },
                new JobApplicationDto
                {
                    Id = 2,
                    UserId = userId,
                    Position = "C# Developer"
                }
            };

            _mapperMock
                .Setup(x => x.Map<List<JobApplicationDto>>(applications))
                .Returns(dtoList);

            var service = new JobApplicationService(
                _repositoryMock.Object,
                _mapperMock.Object,
                _companyRepositoryMock.Object,
                _jobCategoryRepositoryMock.Object,
                _userContextMock.Object);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, x => Assert.Equal(userId, x.UserId));

            _repositoryMock.Verify(
                x => x.GetAllByUserIdAsync(userId),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<List<JobApplicationDto>>(applications),
                Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WhenApplicationBelongsToCurrentUser_ReturnsApplication()
        {
            // Arrange
            var userId = 1L;
            var applicationId = 10L;

            var application = new JobApplication
            {
                Id = applicationId,
                UserId = userId,
                Position = "Backend Developer"
            };

            var dto = new JobApplicationDto
            {
                Id = applicationId,
                UserId = userId,
                Position = "Backend Developer"
            };

            _userContextMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _repositoryMock
                .Setup(x => x.GetByIdAsync(applicationId))
                .ReturnsAsync(application);

            _mapperMock
                .Setup(x => x.Map<JobApplicationDto>(application))
                .Returns(dto);

            var service = new JobApplicationService(
                _repositoryMock.Object,
                _mapperMock.Object,
                _companyRepositoryMock.Object,
                _jobCategoryRepositoryMock.Object,
                _userContextMock.Object);

            // Act
            var result = await service.GetByIdAsync(applicationId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(applicationId, result.Id);
            Assert.Equal(userId, result.UserId);
            Assert.Equal("Backend Developer", result.Position);

            _repositoryMock.Verify(
                x => x.GetByIdAsync(applicationId),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<JobApplicationDto>(application),
                Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WhenApplicationDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            var userId = 1L;
            var applicationId = 10L;

            _userContextMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _repositoryMock
                .Setup(x => x.GetByIdAsync(applicationId))
                .ReturnsAsync((JobApplication?)null);

            var service = new JobApplicationService(
                _repositoryMock.Object,
                _mapperMock.Object,
                _companyRepositoryMock.Object,
                _jobCategoryRepositoryMock.Object,
                _userContextMock.Object);

            // Act
            var action = async () =>
                await service.GetByIdAsync(applicationId);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(action);

            _mapperMock.Verify(
                x => x.Map<JobApplicationDto>(
                    It.IsAny<JobApplication>()),
                Times.Never);
        }

        [Fact]
        public async Task GetByIdAsync_WhenApplicationBelongsToAnotherUser_ThrowsNotFoundException()
        {
            // Arrange
            var currentUserId = 1L;
            var applicationId = 10L;

            var application = new JobApplication
            {
                Id = applicationId,
                UserId = 2L,
                Position = "Backend Developer"
            };

            _userContextMock
                .Setup(x => x.UserId)
                .Returns(currentUserId);

            _repositoryMock
                .Setup(x => x.GetByIdAsync(applicationId))
                .ReturnsAsync(application);

            var service = new JobApplicationService(
                _repositoryMock.Object,
                _mapperMock.Object,
                _companyRepositoryMock.Object,
                _jobCategoryRepositoryMock.Object,
                _userContextMock.Object);

            // Act
            var action = async () =>
                await service.GetByIdAsync(applicationId);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(action);

            _mapperMock.Verify(
                x => x.Map<JobApplicationDto>(
                    It.IsAny<JobApplication>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WhenCompanyAndCategoryExist_CreatesApplication()
        {
            // Arrange
            var userId = 1L;
            var companyId = 10L;
            var categoryId = 20L;

            var dto = new CreateJobApplicationDto
            {
                Location = "Dublin",
                Position = "Backend Developer",
                CompanyId = companyId,
                JobCategoryId = categoryId,
                WorkPlace = WorkPlace.Remote,
                Status = JobApplicationStatus.Saved
            };

            var mappedApplication = new JobApplication
            {
                Id = 100,
                CompanyId = companyId,
                JobCategoryId = categoryId
            };

            var resultDto = new JobApplicationDto
            {
                Id = 100,
                UserId = userId,
                CompanyId = companyId,
                JobCategoryId = categoryId,
                Position = dto.Position
            };

            _userContextMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _companyRepositoryMock
                .Setup(x => x.ExistsAsync(companyId))
                .ReturnsAsync(true);

            _jobCategoryRepositoryMock
                .Setup(x => x.ExistsAsync(categoryId))
                .ReturnsAsync(true);

            _mapperMock
                .Setup(x => x.Map<JobApplication>(dto))
                .Returns(mappedApplication);

            _mapperMock
                .Setup(x => x.Map<JobApplicationDto>(mappedApplication))
                .Returns(resultDto);

            var service = new JobApplicationService(
                _repositoryMock.Object,
                _mapperMock.Object,
                _companyRepositoryMock.Object,
                _jobCategoryRepositoryMock.Object,
                _userContextMock.Object);

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, mappedApplication.UserId);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(dto.Position, result.Position);

            _repositoryMock.Verify(
                x => x.CreateAsync(mappedApplication),
                Times.Once);

            _companyRepositoryMock.Verify(
                x => x.ExistsAsync(companyId),
                Times.Once);

            _jobCategoryRepositoryMock.Verify(
                x => x.ExistsAsync(categoryId),
                Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenCompanyDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            var companyId = 10L;
            var categoryId = 20L;

            var dto = new CreateJobApplicationDto
            {
                Location = "Dublin",
                Position = "Backend Developer",
                CompanyId = companyId,
                JobCategoryId = categoryId,
                WorkPlace = WorkPlace.Remote,
                Status = JobApplicationStatus.Saved
            };

            _companyRepositoryMock
                .Setup(x => x.ExistsAsync(companyId))
                .ReturnsAsync(false);

            var service = new JobApplicationService(
                _repositoryMock.Object,
                _mapperMock.Object,
                _companyRepositoryMock.Object,
                _jobCategoryRepositoryMock.Object,
                _userContextMock.Object);

            // Act
            var action = async () =>
                await service.CreateAsync(dto);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(action);

            _jobCategoryRepositoryMock.Verify(
                x => x.ExistsAsync(It.IsAny<long>()),
                Times.Never);

            _mapperMock.Verify(
                x => x.Map<JobApplication>(It.IsAny<CreateJobApplicationDto>()),
                Times.Never);

            _repositoryMock.Verify(
                x => x.CreateAsync(It.IsAny<JobApplication>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WhenJobCategoryDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            var companyId = 10L;
            var categoryId = 20L;

            var dto = new CreateJobApplicationDto
            {
                Location = "Dublin",
                Position = "Backend Developer",
                CompanyId = companyId,
                JobCategoryId = categoryId,
                WorkPlace = WorkPlace.Remote,
                Status = JobApplicationStatus.Saved
            };

            _companyRepositoryMock
                .Setup(x => x.ExistsAsync(companyId))
                .ReturnsAsync(true);

            _jobCategoryRepositoryMock
                .Setup(x => x.ExistsAsync(categoryId))
                .ReturnsAsync(false);

            var service = new JobApplicationService(
                _repositoryMock.Object,
                _mapperMock.Object,
                _companyRepositoryMock.Object,
                _jobCategoryRepositoryMock.Object,
                _userContextMock.Object);

            // Act
            var action = async () =>
                await service.CreateAsync(dto);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(action);

            _mapperMock.Verify(
                x => x.Map<JobApplication>(
                    It.IsAny<CreateJobApplicationDto>()),
                Times.Never);

            _repositoryMock.Verify(
                x => x.CreateAsync(
                    It.IsAny<JobApplication>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WhenApplicationBelongsToCurrentUser_UpdatesApplication()
        {
            // Arrange
            var userId = 1L;
            var applicationId = 10L;
            var companyId = 20L;
            var categoryId = 30L;

            var existingApplication = new JobApplication
            {
                Id = applicationId,
                UserId = userId,
                Position = "Junior Developer",
                CompanyId = 5L,
                JobCategoryId = 6L
            };

            var dto = new UpdateJobApplicationDto
            {
                Location = "Dublin",
                Position = "Backend Developer",
                CompanyId = companyId,
                JobCategoryId = categoryId,
                WorkPlace = WorkPlace.Hybrid,
                Status = JobApplicationStatus.Applied
            };

            _userContextMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _repositoryMock
                .Setup(x => x.GetByIdAsync(applicationId))
                .ReturnsAsync(existingApplication);

            _companyRepositoryMock
                .Setup(x => x.ExistsAsync(companyId))
                .ReturnsAsync(true);

            _jobCategoryRepositoryMock
                .Setup(x => x.ExistsAsync(categoryId))
                .ReturnsAsync(true);

            _mapperMock
                .Setup(x => x.Map(dto, existingApplication))
                .Callback<UpdateJobApplicationDto, JobApplication>(
                    (source, destination) =>
                    {
                        destination.Position = source.Position;
                        destination.Location = source.Location;
                        destination.CompanyId = source.CompanyId;
                        destination.JobCategoryId = source.JobCategoryId;
                        destination.WorkPlace = source.WorkPlace;
                        destination.Status = source.Status;
                    });

            var service = new JobApplicationService(
                _repositoryMock.Object,
                _mapperMock.Object,
                _companyRepositoryMock.Object,
                _jobCategoryRepositoryMock.Object,
                _userContextMock.Object);

            // Act
            await service.UpdateAsync(applicationId, dto);

            // Assert
            Assert.Equal("Backend Developer", existingApplication.Position);
            Assert.Equal("Dublin", existingApplication.Location);
            Assert.Equal(companyId, existingApplication.CompanyId);
            Assert.Equal(categoryId, existingApplication.JobCategoryId);
            Assert.Equal(WorkPlace.Hybrid, existingApplication.WorkPlace);
            Assert.Equal(JobApplicationStatus.Applied, existingApplication.Status);

            _repositoryMock.Verify(
                x => x.UpdateAsync(existingApplication),
                Times.Once);

            _companyRepositoryMock.Verify(
                x => x.ExistsAsync(companyId),
                Times.Once);

            _jobCategoryRepositoryMock.Verify(
                x => x.ExistsAsync(categoryId),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenApplicationDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            var userId = 1L;
            var applicationId = 10L;

            var dto = new UpdateJobApplicationDto
            {
                Location = "Dublin",
                Position = "Backend Developer",
                CompanyId = 20L,
                JobCategoryId = 30L,
                WorkPlace = WorkPlace.Remote,
                Status = JobApplicationStatus.Applied
            };

            _userContextMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _repositoryMock
                .Setup(x => x.GetByIdAsync(applicationId))
                .ReturnsAsync((JobApplication?)null);

            var service = new JobApplicationService(
                _repositoryMock.Object,
                _mapperMock.Object,
                _companyRepositoryMock.Object,
                _jobCategoryRepositoryMock.Object,
                _userContextMock.Object);

            // Act
            var action = async () =>
                await service.UpdateAsync(applicationId, dto);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(action);

            _companyRepositoryMock.Verify(
                x => x.ExistsAsync(It.IsAny<long>()),
                Times.Never);

            _jobCategoryRepositoryMock.Verify(
                x => x.ExistsAsync(It.IsAny<long>()),
                Times.Never);

            _mapperMock.Verify(
                x => x.Map(
                    It.IsAny<UpdateJobApplicationDto>(),
                    It.IsAny<JobApplication>()),
                Times.Never);

            _repositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<JobApplication>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WhenApplicationBelongsToAnotherUser_ThrowsNotFoundException()
        {
            // Arrange
            var currentUserId = 1L;
            var applicationId = 10L;

            var existingApplication = new JobApplication
            {
                Id = applicationId,
                UserId = 2L,
                Position = "Backend Developer"
            };

            var dto = new UpdateJobApplicationDto
            {
                Location = "Dublin",
                Position = "Senior Backend Developer",
                CompanyId = 20L,
                JobCategoryId = 30L,
                WorkPlace = WorkPlace.Remote,
                Status = JobApplicationStatus.Applied
            };

            _userContextMock
                .Setup(x => x.UserId)
                .Returns(currentUserId);

            _repositoryMock
                .Setup(x => x.GetByIdAsync(applicationId))
                .ReturnsAsync(existingApplication);

            var service = new JobApplicationService(
                _repositoryMock.Object,
                _mapperMock.Object,
                _companyRepositoryMock.Object,
                _jobCategoryRepositoryMock.Object,
                _userContextMock.Object);

            // Act
            var action = async () =>
                await service.UpdateAsync(applicationId, dto);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(action);

            _companyRepositoryMock.Verify(
                x => x.ExistsAsync(It.IsAny<long>()),
                Times.Never);

            _jobCategoryRepositoryMock.Verify(
                x => x.ExistsAsync(It.IsAny<long>()),
                Times.Never);

            _mapperMock.Verify(
                x => x.Map(
                    It.IsAny<UpdateJobApplicationDto>(),
                    It.IsAny<JobApplication>()),
                Times.Never);

            _repositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<JobApplication>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WhenCompanyDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            var userId = 1L;
            var applicationId = 10L;
            var companyId = 20L;
            var categoryId = 30L;

            var existingApplication = new JobApplication
            {
                Id = applicationId,
                UserId = userId,
                Position = "Junior Developer"
            };

            var dto = new UpdateJobApplicationDto
            {
                Location = "Dublin",
                Position = "Backend Developer",
                CompanyId = companyId,
                JobCategoryId = categoryId,
                WorkPlace = WorkPlace.Remote,
                Status = JobApplicationStatus.Applied
            };

            _userContextMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _repositoryMock
                .Setup(x => x.GetByIdAsync(applicationId))
                .ReturnsAsync(existingApplication);

            _companyRepositoryMock
                .Setup(x => x.ExistsAsync(companyId))
                .ReturnsAsync(false);

            var service = new JobApplicationService(
                _repositoryMock.Object,
                _mapperMock.Object,
                _companyRepositoryMock.Object,
                _jobCategoryRepositoryMock.Object,
                _userContextMock.Object);

            // Act
            var action = async () =>
                await service.UpdateAsync(applicationId, dto);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(action);

            _jobCategoryRepositoryMock.Verify(
                x => x.ExistsAsync(It.IsAny<long>()),
                Times.Never);

            _mapperMock.Verify(
                x => x.Map(
                    It.IsAny<UpdateJobApplicationDto>(),
                    It.IsAny<JobApplication>()),
                Times.Never);

            _repositoryMock.Verify(
                x => x.UpdateAsync(
                    It.IsAny<JobApplication>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WhenJobCategoryDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            var userId = 1L;
            var applicationId = 10L;
            var companyId = 20L;
            var categoryId = 30L;

            var existingApplication = new JobApplication
            {
                Id = applicationId,
                UserId = userId,
                Position = "Junior Developer"
            };

            var dto = new UpdateJobApplicationDto
            {
                Location = "Dublin",
                Position = "Backend Developer",
                CompanyId = companyId,
                JobCategoryId = categoryId,
                WorkPlace = WorkPlace.Remote,
                Status = JobApplicationStatus.Applied
            };

            _userContextMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _repositoryMock
                .Setup(x => x.GetByIdAsync(applicationId))
                .ReturnsAsync(existingApplication);

            _companyRepositoryMock
                .Setup(x => x.ExistsAsync(companyId))
                .ReturnsAsync(true);

            _jobCategoryRepositoryMock
                .Setup(x => x.ExistsAsync(categoryId))
                .ReturnsAsync(false);

            var service = new JobApplicationService(
                _repositoryMock.Object,
                _mapperMock.Object,
                _companyRepositoryMock.Object,
                _jobCategoryRepositoryMock.Object,
                _userContextMock.Object);

            // Act
            var action = async () =>
                await service.UpdateAsync(applicationId, dto);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(action);

            _mapperMock.Verify(
                x => x.Map(
                    It.IsAny<UpdateJobApplicationDto>(),
                    It.IsAny<JobApplication>()),
                Times.Never);

            _repositoryMock.Verify(
                x => x.UpdateAsync(
                    It.IsAny<JobApplication>()),
                Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenApplicationBelongsToCurrentUser_DeletesApplication()
        {
            // Arrange
            var userId = 1L;
            var applicationId = 10L;

            var application = new JobApplication
            {
                Id = applicationId,
                UserId = userId,
                Position = "Backend Developer"
            };

            _userContextMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _repositoryMock
                .Setup(x => x.GetByIdAsync(applicationId))
                .ReturnsAsync(application);

            var service = new JobApplicationService(
                _repositoryMock.Object,
                _mapperMock.Object,
                _companyRepositoryMock.Object,
                _jobCategoryRepositoryMock.Object,
                _userContextMock.Object);

            // Act
            await service.DeleteAsync(applicationId);

            // Assert
            _repositoryMock.Verify(
                x => x.DeleteAsync(application),
                Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenApplicationDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            var userId = 1L;
            var applicationId = 10L;

            _userContextMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _repositoryMock
                .Setup(x => x.GetByIdAsync(applicationId))
                .ReturnsAsync((JobApplication?)null);

            var service = new JobApplicationService(
                _repositoryMock.Object,
                _mapperMock.Object,
                _companyRepositoryMock.Object,
                _jobCategoryRepositoryMock.Object,
                _userContextMock.Object);

            // Act
            var action = async () =>
                await service.DeleteAsync(applicationId);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(action);

            _repositoryMock.Verify(
                x => x.DeleteAsync(It.IsAny<JobApplication>()),
                Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenApplicationBelongsToAnotherUser_ThrowsNotFoundException()
        {
            // Arrange
            var currentUserId = 1L;
            var applicationId = 10L;

            var application = new JobApplication
            {
                Id = applicationId,
                UserId = 2L,
                Position = "Backend Developer"
            };

            _userContextMock
                .Setup(x => x.UserId)
                .Returns(currentUserId);

            _repositoryMock
                .Setup(x => x.GetByIdAsync(applicationId))
                .ReturnsAsync(application);

            var service = new JobApplicationService(
                _repositoryMock.Object,
                _mapperMock.Object,
                _companyRepositoryMock.Object,
                _jobCategoryRepositoryMock.Object,
                _userContextMock.Object);

            // Act
            var action = async () =>
                await service.DeleteAsync(applicationId);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(action);

            _repositoryMock.Verify(
                x => x.DeleteAsync(It.IsAny<JobApplication>()),
                Times.Never);
        }
    }
}
