using AutoMapper;
using JobTracker.Application.DTOs.InterviewDtos;
using JobTracker.Application.Exceptions;
using JobTracker.Application.Interfaces.Repository;
using JobTracker.Application.Interfaces.Service;
using JobTracker.Application.Services;
using JobTracker.Domain.Entities;
using JobTracker.Domain.Enums;
using Moq;

namespace JobTracker.Tests.UnitTests
{
    public class InterviewServiceTests
    {
        private readonly Mock<IInterviewRepository> _interviewRepositoryMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<IJobApplicationRepository> _jobApplicationRepositoryMock = new();
        private readonly Mock<IUserContext> _userContextMock = new();

        private InterviewService Service()
        {
            return new InterviewService(
                _interviewRepositoryMock.Object,
                _mapperMock.Object,
                _jobApplicationRepositoryMock.Object,
                _userContextMock.Object);
        }


        [Fact]
        public async Task GetAllAsync_ReturnsCurrentUserInterviews()
        {
            var interviews = new List<Interview>
            {
                new Interview
                {
                    Id = 1,
                    JobApplicationId = 10,
                    InterviewDate = new DateTime(2026, 9, 20),
                    InterviewType = InterviewType.TechnicalInterview
                },
                new Interview
                {
                    Id = 2,
                    JobApplicationId = 11,
                    InterviewDate = new DateTime(2026, 9, 21),
                    InterviewType = InterviewType.HRInterview
                }
            };

            var interviewDtos = new List<InterviewDto>
            {
                new InterviewDto
                {
                    Id = 1,
                    JobApplicationId = 10,
                    InterviewDate = new DateTime(2026, 9, 20),
                    InterviewType = InterviewType.TechnicalInterview
                },
                new InterviewDto
                {
                    Id = 2,
                    JobApplicationId = 11,
                    InterviewDate = new DateTime(2026, 9, 21),
                    InterviewType = InterviewType.HRInterview
                }
            };

            _userContextMock
                .Setup(x => x.UserId)
                .Returns(1);

            _interviewRepositoryMock
                .Setup(x => x.GetAllByUserIdAsync(1))
                .ReturnsAsync(interviews);

            _mapperMock
                .Setup(x => x.Map<List<InterviewDto>>(interviews))
                .Returns(interviewDtos);

            var service = Service();

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            Assert.Equal(1, result[0].Id);
            Assert.Equal(InterviewType.TechnicalInterview, result[0].InterviewType);

            Assert.Equal(2, result[1].Id);
            Assert.Equal(InterviewType.HRInterview, result[1].InterviewType);

            _interviewRepositoryMock.Verify(
                x => x.GetAllByUserIdAsync(1),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<List<InterviewDto>>(interviews),
                Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ValidInterview_ReturnsInterview()
        {
            // Arrange
            const long userId = 1;
            const long interviewId = 1;

            var interview = new Interview
            {
                Id = interviewId,
                JobApplicationId = 10,
                InterviewDate = new DateTime(2026, 9, 20),
                InterviewType = InterviewType.TechnicalInterview
            };

            var jobApplication = new JobApplication
            {
                Id = 10,
                UserId = userId
            };

            var interviewDto = new InterviewDto
            {
                Id = interviewId,
                JobApplicationId = 10,
                InterviewDate = new DateTime(2026, 9, 20),
                InterviewType = InterviewType.TechnicalInterview
            };

            _userContextMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _interviewRepositoryMock
                .Setup(x => x.GetByIdAsync(interviewId))
                .ReturnsAsync(interview);

            _jobApplicationRepositoryMock
                .Setup(x => x.GetByIdAsync(interview.JobApplicationId))
                .ReturnsAsync(jobApplication);

            _mapperMock
                .Setup(x => x.Map<InterviewDto>(interview))
                .Returns(interviewDto);

            var service = Service();

            // Act
            var result = await service.GetByIdAsync(interviewId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(interviewId, result.Id);
            Assert.Equal(10, result.JobApplicationId);
            Assert.Equal(InterviewType.TechnicalInterview, result.InterviewType);

            _interviewRepositoryMock.Verify(
                x => x.GetByIdAsync(interviewId),
                Times.Once);

            _jobApplicationRepositoryMock.Verify(
                x => x.GetByIdAsync(interview.JobApplicationId),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<InterviewDto>(interview),
                Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_InterviewDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            const long userId = 1;
            const long interviewId = 1;

            _userContextMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _interviewRepositoryMock
                .Setup(x => x.GetByIdAsync(interviewId))
                .ReturnsAsync((Interview?)null);

            var service = Service();

            // Act
            var action = async () =>
                await service.GetByIdAsync(interviewId);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(action);

            _interviewRepositoryMock.Verify(
                x => x.GetByIdAsync(interviewId),
                Times.Once);

            _jobApplicationRepositoryMock.Verify(
                x => x.GetByIdAsync(It.IsAny<long>()),
                Times.Never);

            _mapperMock.Verify(
                x => x.Map<InterviewDto>(It.IsAny<Interview>()),
                Times.Never);
        }

        [Fact]
        public async Task GetByIdAsync_InterviewBelongsToAnotherUser_ThrowsNotFoundException()
        {
            // Arrange
            const long currentUserId = 1;
            const long anotherUserId = 2;
            const long interviewId = 1;

            var interview = new Interview
            {
                Id = interviewId,
                JobApplicationId = 10,
                InterviewDate = new DateTime(2026, 9, 20),
                InterviewType = InterviewType.TechnicalInterview,
                Notes = "Technical interview"
            };

            var jobApplication = new JobApplication
            {
                Id = 10,
                UserId = anotherUserId
            };

            _userContextMock
                .Setup(x => x.UserId)
                .Returns(currentUserId);

            _interviewRepositoryMock
                .Setup(x => x.GetByIdAsync(interviewId))
                .ReturnsAsync(interview);

            _jobApplicationRepositoryMock
                .Setup(x => x.GetByIdAsync(interview.JobApplicationId))
                .ReturnsAsync(jobApplication);

            var service = Service();

            // Act
            var action = async () =>
                await service.GetByIdAsync(interviewId);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(action);

            _interviewRepositoryMock.Verify(
                x => x.GetByIdAsync(interviewId),
                Times.Once);

            _jobApplicationRepositoryMock.Verify(
                x => x.GetByIdAsync(interview.JobApplicationId),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<InterviewDto>(It.IsAny<Interview>()),
                Times.Never);
        }

        [Fact]
        public async Task GetByIdAsync_JobApplicationDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            const long userId = 1;
            const long interviewId = 1;

            var interview = new Interview
            {
                Id = interviewId,
                JobApplicationId = 10,
                InterviewDate = new DateTime(2026, 9, 20),
                InterviewType = InterviewType.TechnicalInterview,
                Notes = "Technical interview"
            };

            _userContextMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _interviewRepositoryMock
                .Setup(x => x.GetByIdAsync(interviewId))
                .ReturnsAsync(interview);

            _jobApplicationRepositoryMock
                .Setup(x => x.GetByIdAsync(interview.JobApplicationId))
                .ReturnsAsync((JobApplication?)null);

            var service = Service();

            // Act
            var action = async () =>
                await service.GetByIdAsync(interviewId);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(action);

            _interviewRepositoryMock.Verify(
                x => x.GetByIdAsync(interviewId),
                Times.Once);

            _jobApplicationRepositoryMock.Verify(
                x => x.GetByIdAsync(interview.JobApplicationId),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<InterviewDto>(It.IsAny<Interview>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateAsync_ValidData_ReturnsCreatedInterview()
        {
            // Arrange
            const long userId = 1;
            const long jobApplicationId = 10;

            var createInterviewDto = new CreateInterviewDto
            {
                JobApplicationId = jobApplicationId,
                InterviewDate = new DateTime(2026, 9, 20),
                InterviewType = InterviewType.TechnicalInterview,
                Notes = "Technical interview"
            };

            var jobApplication = new JobApplication
            {
                Id = jobApplicationId,
                UserId = userId
            };

            var interview = new Interview
            {
                Id = 1,
                JobApplicationId = jobApplicationId,
                InterviewDate = createInterviewDto.InterviewDate,
                InterviewType = createInterviewDto.InterviewType,
                Notes = createInterviewDto.Notes
            };

            var interviewDto = new InterviewDto
            {
                Id = 1,
                JobApplicationId = jobApplicationId,
                InterviewDate = createInterviewDto.InterviewDate,
                InterviewType = createInterviewDto.InterviewType,
                Notes = createInterviewDto.Notes
            };

            _userContextMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _jobApplicationRepositoryMock
                .Setup(x => x.GetByIdAsync(jobApplicationId))
                .ReturnsAsync(jobApplication);

            _mapperMock
                .Setup(x => x.Map<Interview>(createInterviewDto))
                .Returns(interview);

            _interviewRepositoryMock
                .Setup(x => x.CreateAsync(interview))
                .Returns(Task.CompletedTask);

            _mapperMock
                .Setup(x => x.Map<InterviewDto>(interview))
                .Returns(interviewDto);

            var service = Service();

            // Act
            var result = await service.CreateAsync(createInterviewDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(interview.Id, result.Id);
            Assert.Equal(jobApplicationId, result.JobApplicationId);
            Assert.Equal(InterviewType.TechnicalInterview, result.InterviewType);
            Assert.Equal("Technical interview", result.Notes);

            _jobApplicationRepositoryMock.Verify(
                x => x.GetByIdAsync(jobApplicationId),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<Interview>(createInterviewDto),
                Times.Once);

            _interviewRepositoryMock.Verify(
                x => x.CreateAsync(interview),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<InterviewDto>(interview),
                Times.Once);
        }

        [Fact]
        public async Task CreateAsync_JobApplicationDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            const long userId = 1;
            const long jobApplicationId = 10;

            var createInterviewDto = new CreateInterviewDto
            {
                JobApplicationId = jobApplicationId,
                InterviewDate = new DateTime(2026, 9, 20),
                InterviewType = InterviewType.TechnicalInterview,
                Notes = "Technical interview"
            };

            _userContextMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _jobApplicationRepositoryMock
                .Setup(x => x.GetByIdAsync(jobApplicationId))
                .ReturnsAsync((JobApplication?)null);

            var service = Service();

            // Act
            var action = async () =>
                await service.CreateAsync(createInterviewDto);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(action);

            _jobApplicationRepositoryMock.Verify(
                x => x.GetByIdAsync(jobApplicationId),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<Interview>(It.IsAny<CreateInterviewDto>()),
                Times.Never);

            _interviewRepositoryMock.Verify(
                x => x.CreateAsync(It.IsAny<Interview>()),
                Times.Never);

            _mapperMock.Verify(
                x => x.Map<InterviewDto>(It.IsAny<Interview>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateAsync_JobApplicationBelongsToAnotherUser_ThrowsNotFoundException()
        {
            // Arrange
            const long currentUserId = 1;
            const long anotherUserId = 2;
            const long jobApplicationId = 10;

            var createInterviewDto = new CreateInterviewDto
            {
                JobApplicationId = jobApplicationId,
                InterviewDate = new DateTime(2026, 9, 20),
                InterviewType = InterviewType.TechnicalInterview,
                Notes = "Technical interview"
            };

            var jobApplication = new JobApplication
            {
                Id = jobApplicationId,
                UserId = anotherUserId
            };

            _userContextMock
                .Setup(x => x.UserId)
                .Returns(currentUserId);

            _jobApplicationRepositoryMock
                .Setup(x => x.GetByIdAsync(jobApplicationId))
                .ReturnsAsync(jobApplication);

            var service = Service();

            // Act
            var action = async () =>
                await service.CreateAsync(createInterviewDto);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(action);

            _jobApplicationRepositoryMock.Verify(
                x => x.GetByIdAsync(jobApplicationId),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<Interview>(It.IsAny<CreateInterviewDto>()),
                Times.Never);

            _interviewRepositoryMock.Verify(
                x => x.CreateAsync(It.IsAny<Interview>()),
                Times.Never);

            _mapperMock.Verify(
                x => x.Map<InterviewDto>(It.IsAny<Interview>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ValidData_UpdatesInterview()
        {
            // Arrange
            const long userId = 1;
            const long interviewId = 1;
            const long currentJobApplicationId = 10;
            const long newJobApplicationId = 20;

            var interview = new Interview
            {
                Id = interviewId,
                JobApplicationId = currentJobApplicationId,
                InterviewDate = new DateTime(2026, 9, 20),
                InterviewType = InterviewType.HRInterview,
                Notes = "Old notes"
            };

            var updateInterviewDto = new UpdateInterviewDto
            {
                JobApplicationId = newJobApplicationId,
                InterviewDate = new DateTime(2026, 9, 25),
                InterviewType = InterviewType.TechnicalInterview,
                Notes = "Updated notes"
            };

            var currentJobApplication = new JobApplication
            {
                Id = currentJobApplicationId,
                UserId = userId
            };

            var newJobApplication = new JobApplication
            {
                Id = newJobApplicationId,
                UserId = userId
            };

            _userContextMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _interviewRepositoryMock
                .Setup(x => x.GetByIdAsync(interviewId))
                .ReturnsAsync(interview);

            _jobApplicationRepositoryMock
                .Setup(x => x.GetByIdAsync(currentJobApplicationId))
                .ReturnsAsync(currentJobApplication);

            _jobApplicationRepositoryMock
                .Setup(x => x.GetByIdAsync(newJobApplicationId))
                .ReturnsAsync(newJobApplication);

            _mapperMock
                .Setup(x => x.Map(updateInterviewDto, interview))
                .Callback<UpdateInterviewDto, Interview>((dto, entity) =>
                {
                    entity.JobApplicationId = dto.JobApplicationId;
                    entity.InterviewDate = dto.InterviewDate;
                    entity.InterviewType = dto.InterviewType;
                    entity.Notes = dto.Notes;
                });

            _interviewRepositoryMock
                .Setup(x => x.UpdateAsync(interview))
                .Returns(Task.CompletedTask);

            var service = Service();

            // Act
            await service.UpdateAsync(interviewId, updateInterviewDto);

            // Assert
            Assert.Equal(newJobApplicationId, interview.JobApplicationId);
            Assert.Equal(updateInterviewDto.InterviewDate, interview.InterviewDate);
            Assert.Equal(updateInterviewDto.InterviewType, interview.InterviewType);
            Assert.Equal(updateInterviewDto.Notes, interview.Notes);

            _interviewRepositoryMock.Verify(
                x => x.GetByIdAsync(interviewId),
                Times.Once);

            _jobApplicationRepositoryMock.Verify(
                x => x.GetByIdAsync(currentJobApplicationId),
                Times.Once);

            _jobApplicationRepositoryMock.Verify(
                x => x.GetByIdAsync(newJobApplicationId),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map(updateInterviewDto, interview),
                Times.Once);

            _interviewRepositoryMock.Verify(
                x => x.UpdateAsync(interview),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_InterviewDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            const long userId = 1;
            const long interviewId = 1;

            var updateInterviewDto = new UpdateInterviewDto
            {
                JobApplicationId = 20,
                InterviewDate = new DateTime(2026, 9, 25),
                InterviewType = InterviewType.TechnicalInterview,
                Notes = "Updated notes"
            };

            _userContextMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _interviewRepositoryMock
                .Setup(x => x.GetByIdAsync(interviewId))
                .ReturnsAsync((Interview?)null);

            var service = Service();

            // Act
            var action = async () =>
                await service.UpdateAsync(interviewId, updateInterviewDto);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(action);

            _interviewRepositoryMock.Verify(
                x => x.GetByIdAsync(interviewId),
                Times.Once);

            _jobApplicationRepositoryMock.Verify(
                x => x.GetByIdAsync(It.IsAny<long>()),
                Times.Never);

            _mapperMock.Verify(
                x => x.Map(updateInterviewDto, It.IsAny<Interview>()),
                Times.Never);

            _interviewRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<Interview>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_CurrentJobApplicationDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            const long userId = 1;
            const long interviewId = 1;
            const long currentJobApplicationId = 10;

            var interview = new Interview
            {
                Id = interviewId,
                JobApplicationId = currentJobApplicationId,
                InterviewDate = new DateTime(2026, 9, 20),
                InterviewType = InterviewType.HRInterview,
                Notes = "Old notes"
            };

            var updateInterviewDto = new UpdateInterviewDto
            {
                JobApplicationId = 20,
                InterviewDate = new DateTime(2026, 9, 25),
                InterviewType = InterviewType.TechnicalInterview,
                Notes = "Updated notes"
            };

            _userContextMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _interviewRepositoryMock
                .Setup(x => x.GetByIdAsync(interviewId))
                .ReturnsAsync(interview);

            _jobApplicationRepositoryMock
                .Setup(x => x.GetByIdAsync(currentJobApplicationId))
                .ReturnsAsync((JobApplication?)null);

            var service = Service();

            // Act
            var action = async () =>
                await service.UpdateAsync(interviewId, updateInterviewDto);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(action);

            _interviewRepositoryMock.Verify(
                x => x.GetByIdAsync(interviewId),
                Times.Once);

            _jobApplicationRepositoryMock.Verify(
                x => x.GetByIdAsync(currentJobApplicationId),
                Times.Once);

            _jobApplicationRepositoryMock.Verify(
                x => x.GetByIdAsync(updateInterviewDto.JobApplicationId),
                Times.Never);

            _mapperMock.Verify(
                x => x.Map(updateInterviewDto, It.IsAny<Interview>()),
                Times.Never);

            _interviewRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<Interview>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_CurrentJobApplicationBelongsToAnotherUser_ThrowsNotFoundException()
        {
            // Arrange
            const long currentUserId = 1;
            const long anotherUserId = 2;
            const long interviewId = 1;
            const long currentJobApplicationId = 10;

            var interview = new Interview
            {
                Id = interviewId,
                JobApplicationId = currentJobApplicationId,
                InterviewDate = new DateTime(2026, 9, 20),
                InterviewType = InterviewType.HRInterview,
                Notes = "Old notes"
            };

            var currentJobApplication = new JobApplication
            {
                Id = currentJobApplicationId,
                UserId = anotherUserId
            };

            var updateInterviewDto = new UpdateInterviewDto
            {
                JobApplicationId = 20,
                InterviewDate = new DateTime(2026, 9, 25),
                InterviewType = InterviewType.TechnicalInterview,
                Notes = "Updated notes"
            };

            _userContextMock
                .Setup(x => x.UserId)
                .Returns(currentUserId);

            _interviewRepositoryMock
                .Setup(x => x.GetByIdAsync(interviewId))
                .ReturnsAsync(interview);

            _jobApplicationRepositoryMock
                .Setup(x => x.GetByIdAsync(currentJobApplicationId))
                .ReturnsAsync(currentJobApplication);

            var service = Service();

            // Act
            var action = async () =>
                await service.UpdateAsync(interviewId, updateInterviewDto);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(action);

            _interviewRepositoryMock.Verify(
                x => x.GetByIdAsync(interviewId),
                Times.Once);

            _jobApplicationRepositoryMock.Verify(
                x => x.GetByIdAsync(currentJobApplicationId),
                Times.Once);

            _jobApplicationRepositoryMock.Verify(
                x => x.GetByIdAsync(updateInterviewDto.JobApplicationId),
                Times.Never);

            _mapperMock.Verify(
                x => x.Map(updateInterviewDto, It.IsAny<Interview>()),
                Times.Never);

            _interviewRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<Interview>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_NewJobApplicationDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            const long userId = 1;
            const long interviewId = 1;
            const long currentJobApplicationId = 10;
            const long newJobApplicationId = 20;

            var interview = new Interview
            {
                Id = interviewId,
                JobApplicationId = currentJobApplicationId,
                InterviewDate = new DateTime(2026, 9, 20),
                InterviewType = InterviewType.HRInterview,
                Notes = "Old notes"
            };

            var currentJobApplication = new JobApplication
            {
                Id = currentJobApplicationId,
                UserId = userId
            };

            var updateInterviewDto = new UpdateInterviewDto
            {
                JobApplicationId = newJobApplicationId,
                InterviewDate = new DateTime(2026, 9, 25),
                InterviewType = InterviewType.TechnicalInterview,
                Notes = "Updated notes"
            };

            _userContextMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _interviewRepositoryMock
                .Setup(x => x.GetByIdAsync(interviewId))
                .ReturnsAsync(interview);

            _jobApplicationRepositoryMock
                .Setup(x => x.GetByIdAsync(currentJobApplicationId))
                .ReturnsAsync(currentJobApplication);

            _jobApplicationRepositoryMock
                .Setup(x => x.GetByIdAsync(newJobApplicationId))
                .ReturnsAsync((JobApplication?)null);

            var service = Service();

            // Act
            var action = async () =>
                await service.UpdateAsync(interviewId, updateInterviewDto);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(action);

            _interviewRepositoryMock.Verify(
                x => x.GetByIdAsync(interviewId),
                Times.Once);

            _jobApplicationRepositoryMock.Verify(
                x => x.GetByIdAsync(currentJobApplicationId),
                Times.Once);

            _jobApplicationRepositoryMock.Verify(
                x => x.GetByIdAsync(newJobApplicationId),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map(updateInterviewDto, It.IsAny<Interview>()),
                Times.Never);

            _interviewRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<Interview>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_NewJobApplicationBelongsToAnotherUser_ThrowsNotFoundException()
        {
            // Arrange
            const long currentUserId = 1;
            const long anotherUserId = 2;
            const long interviewId = 1;
            const long currentJobApplicationId = 10;
            const long newJobApplicationId = 20;

            var interview = new Interview
            {
                Id = interviewId,
                JobApplicationId = currentJobApplicationId,
                InterviewDate = new DateTime(2026, 9, 20),
                InterviewType = InterviewType.HRInterview,
                Notes = "Old notes"
            };

            var currentJobApplication = new JobApplication
            {
                Id = currentJobApplicationId,
                UserId = currentUserId
            };

            var newJobApplication = new JobApplication
            {
                Id = newJobApplicationId,
                UserId = anotherUserId
            };

            var updateInterviewDto = new UpdateInterviewDto
            {
                JobApplicationId = newJobApplicationId,
                InterviewDate = new DateTime(2026, 9, 25),
                InterviewType = InterviewType.TechnicalInterview,
                Notes = "Updated notes"
            };

            _userContextMock
                .Setup(x => x.UserId)
                .Returns(currentUserId);

            _interviewRepositoryMock
                .Setup(x => x.GetByIdAsync(interviewId))
                .ReturnsAsync(interview);

            _jobApplicationRepositoryMock
                .Setup(x => x.GetByIdAsync(currentJobApplicationId))
                .ReturnsAsync(currentJobApplication);

            _jobApplicationRepositoryMock
                .Setup(x => x.GetByIdAsync(newJobApplicationId))
                .ReturnsAsync(newJobApplication);

            var service = Service();

            // Act
            var action = async () =>
                await service.UpdateAsync(interviewId, updateInterviewDto);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(action);

            _interviewRepositoryMock.Verify(
                x => x.GetByIdAsync(interviewId),
                Times.Once);

            _jobApplicationRepositoryMock.Verify(
                x => x.GetByIdAsync(currentJobApplicationId),
                Times.Once);

            _jobApplicationRepositoryMock.Verify(
                x => x.GetByIdAsync(newJobApplicationId),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map(updateInterviewDto, It.IsAny<Interview>()),
                Times.Never);

            _interviewRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<Interview>()),
                Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ValidInterview_DeletesInterview()
        {
            // Arrange
            const long userId = 1;
            const long interviewId = 1;
            const long jobApplicationId = 10;

            var interview = new Interview
            {
                Id = interviewId,
                JobApplicationId = jobApplicationId,
                InterviewDate = new DateTime(2026, 9, 20),
                InterviewType = InterviewType.TechnicalInterview,
                Notes = "Technical interview"
            };

            var jobApplication = new JobApplication
            {
                Id = jobApplicationId,
                UserId = userId
            };

            _userContextMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _interviewRepositoryMock
                .Setup(x => x.GetByIdAsync(interviewId))
                .ReturnsAsync(interview);

            _jobApplicationRepositoryMock
                .Setup(x => x.GetByIdAsync(jobApplicationId))
                .ReturnsAsync(jobApplication);

            _interviewRepositoryMock
                .Setup(x => x.DeleteAsync(interview))
                .Returns(Task.CompletedTask);

            var service = Service();

            // Act
            await service.DeleteAsync(interviewId);

            // Assert
            _interviewRepositoryMock.Verify(
                x => x.GetByIdAsync(interviewId),
                Times.Once);

            _jobApplicationRepositoryMock.Verify(
                x => x.GetByIdAsync(jobApplicationId),
                Times.Once);

            _interviewRepositoryMock.Verify(
                x => x.DeleteAsync(interview),
                Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_InterviewDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            const long userId = 1;
            const long interviewId = 1;

            _userContextMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _interviewRepositoryMock
                .Setup(x => x.GetByIdAsync(interviewId))
                .ReturnsAsync((Interview?)null);

            var service = Service();

            // Act
            var action = async () =>
                await service.DeleteAsync(interviewId);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(action);

            _interviewRepositoryMock.Verify(
                x => x.GetByIdAsync(interviewId),
                Times.Once);

            _jobApplicationRepositoryMock.Verify(
                x => x.GetByIdAsync(It.IsAny<long>()),
                Times.Never);

            _interviewRepositoryMock.Verify(
                x => x.DeleteAsync(It.IsAny<Interview>()),
                Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_JobApplicationDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            const long userId = 1;
            const long interviewId = 1;
            const long jobApplicationId = 10;

            var interview = new Interview
            {
                Id = interviewId,
                JobApplicationId = jobApplicationId,
                InterviewDate = new DateTime(2026, 9, 20),
                InterviewType = InterviewType.TechnicalInterview,
                Notes = "Technical interview"
            };

            _userContextMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _interviewRepositoryMock
                .Setup(x => x.GetByIdAsync(interviewId))
                .ReturnsAsync(interview);

            _jobApplicationRepositoryMock
                .Setup(x => x.GetByIdAsync(jobApplicationId))
                .ReturnsAsync((JobApplication?)null);

            var service = Service();

            // Act
            var action = async () =>
                await service.DeleteAsync(interviewId);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(action);

            _interviewRepositoryMock.Verify(
                x => x.GetByIdAsync(interviewId),
                Times.Once);

            _jobApplicationRepositoryMock.Verify(
                x => x.GetByIdAsync(jobApplicationId),
                Times.Once);

            _interviewRepositoryMock.Verify(
                x => x.DeleteAsync(It.IsAny<Interview>()),
                Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_JobApplicationBelongsToAnotherUser_ThrowsNotFoundException()
        {
            // Arrange
            const long currentUserId = 1;
            const long anotherUserId = 2;
            const long interviewId = 1;
            const long jobApplicationId = 10;

            var interview = new Interview
            {
                Id = interviewId,
                JobApplicationId = jobApplicationId,
                InterviewDate = new DateTime(2026, 9, 20),
                InterviewType = InterviewType.TechnicalInterview,
                Notes = "Technical interview"
            };

            var jobApplication = new JobApplication
            {
                Id = jobApplicationId,
                UserId = anotherUserId
            };

            _userContextMock
                .Setup(x => x.UserId)
                .Returns(currentUserId);

            _interviewRepositoryMock
                .Setup(x => x.GetByIdAsync(interviewId))
                .ReturnsAsync(interview);

            _jobApplicationRepositoryMock
                .Setup(x => x.GetByIdAsync(jobApplicationId))
                .ReturnsAsync(jobApplication);

            var service = Service();

            // Act
            var action = async () =>
                await service.DeleteAsync(interviewId);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(action);

            _interviewRepositoryMock.Verify(
                x => x.GetByIdAsync(interviewId),
                Times.Once);

            _jobApplicationRepositoryMock.Verify(
                x => x.GetByIdAsync(jobApplicationId),
                Times.Once);

            _interviewRepositoryMock.Verify(
                x => x.DeleteAsync(It.IsAny<Interview>()),
                Times.Never);
        }
    }
}
