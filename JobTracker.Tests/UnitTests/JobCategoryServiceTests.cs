using AutoMapper;
using JobTracker.Application.DTOs.CompanyDtos;
using JobTracker.Application.DTOs.JobCategoryDtos;
using JobTracker.Application.Exceptions;
using JobTracker.Application.Interfaces.Repository;
using JobTracker.Application.Services;
using JobTracker.Domain.Entities;
using Moq;

namespace JobTracker.Tests.UnitTests
{
    public class JobCategoryServiceTests
    {
        private readonly Mock<IJobCategoryRepository> _jobCategoryRepositoryMock = new();
        private readonly Mock<IMapper> _mapperMock = new();

        [Fact]
        public async Task GetAllAsync_ReturnsJobCategories()
        {
            // Arrange
            var jobCategory = new List<JobCategory>
            {
                new JobCategory
                {
                    Id = 1,
                    Name = "Software"
                },
                new JobCategory
                {
                    Id = 2,
                    Name = "Programming"
                }
            };

            _jobCategoryRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(jobCategory);

            var jobCategoryDto = new List<JobCategoryDto>
            {
                new JobCategoryDto
                {
                    Id = 1,
                    Name = "Software"
                },
                new JobCategoryDto
                {
                    Id = 2,
                    Name = "Programming"
                }
            };

            _mapperMock
                .Setup(x => x.Map<List<JobCategoryDto>>(jobCategory))
                .Returns(jobCategoryDto);

            var service = new JobCategoryService(
                _jobCategoryRepositoryMock.Object,
                _mapperMock.Object );

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Software", result[0].Name);

            _jobCategoryRepositoryMock.Verify(
                x => x.GetAllAsync(),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<List<JobCategoryDto>>(jobCategory),
                Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_JobCategoryExists_ReturnsJobCategoryDto()
        {
            // Arrange
            var jobCategory = new JobCategory
            {
                Id = 1,
                Name = "Software"
            };

            _jobCategoryRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(jobCategory);

            var jobCategoryDto = new JobCategoryDto
            {
                Id = 1,
                Name = "Software"
            };

            _mapperMock
                .Setup(x => x.Map<JobCategoryDto>(jobCategory))
                .Returns(jobCategoryDto);

            var service = new JobCategoryService(
                _jobCategoryRepositoryMock.Object,
                _mapperMock.Object);

            // Act
            var result = await service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(jobCategory.Name, result.Name);

            _jobCategoryRepositoryMock.Verify(
                x => x.GetByIdAsync(1),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<JobCategoryDto>(jobCategory),
                Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_JobCategoryDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            _jobCategoryRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((JobCategory?)null);

            var service = new JobCategoryService(
                _jobCategoryRepositoryMock.Object,
                _mapperMock.Object);

            // Act
            var action = async () =>
                await service.GetByIdAsync(1);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(action);

            _jobCategoryRepositoryMock.Verify(
                x => x.GetByIdAsync(1),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<JobCategoryDto>(
                    It.IsAny<JobCategory>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateAsync_ValidJobCategory_ReturnsJobCategoryDto()
        {
            // Arrange
            var createJobCategoryDto = new CreateJobCategoryDto
            {
                Name = "Software"
            };

            var jobCategory = new JobCategory
            {
                Id = 1,
                Name = "Software"
            };

            var jobCategoryDto = new JobCategoryDto
            {
                Id = 1,
                Name = "Software"
            };

            _jobCategoryRepositoryMock
                .Setup(x => x.ExistsByNameAsync(createJobCategoryDto.Name, null))
                .ReturnsAsync(false);

            _mapperMock
                .Setup(x => x.Map<JobCategory>(createJobCategoryDto))
                .Returns(jobCategory);

            _mapperMock
                .Setup(x => x.Map<JobCategoryDto>(jobCategory))
                .Returns(jobCategoryDto);

            var service = new JobCategoryService(
                _jobCategoryRepositoryMock.Object,
                _mapperMock.Object);

            // Act
            var result = await service.CreateAsync(createJobCategoryDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Software", result.Name);

            _jobCategoryRepositoryMock.Verify(
                x => x.CreateAsync(jobCategory),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<JobCategory>(createJobCategoryDto),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<JobCategoryDto>(jobCategory),
                Times.Once);

            _jobCategoryRepositoryMock.Verify(
                x => x.ExistsByNameAsync(createJobCategoryDto.Name, null),
                Times.Once);
        }

        [Fact]
        public async Task CreateAsync_JobCategoryNameExists_ThrowsConflictException()
        {
            // Arrange
            var createJobCategoryDto = new CreateJobCategoryDto
            {
                Name = "Software"
            };

            _jobCategoryRepositoryMock
                .Setup(x => x.ExistsByNameAsync(createJobCategoryDto.Name, null))
                .ReturnsAsync(true);

            var service = new JobCategoryService(
                _jobCategoryRepositoryMock.Object,
                _mapperMock.Object);

            // Act
            var action = async () =>
                await service.CreateAsync(createJobCategoryDto);

            // Assert
            await Assert.ThrowsAsync<ConflictException>(action);

            _jobCategoryRepositoryMock.Verify(
                x => x.ExistsByNameAsync(createJobCategoryDto.Name, null),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<JobCategory>(
                    It.IsAny<CreateJobCategoryDto>()),
                Times.Never);

            _jobCategoryRepositoryMock.Verify(
                x => x.CreateAsync(
                    It.IsAny<JobCategory>()),
                Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ValidJobCategory_DeletesJobCategory()
        {
            // Arrange
            var jobCategory = new JobCategory
            {
                Id = 1,
                Name = "Software"
            };

            _jobCategoryRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(jobCategory);

            var service = new JobCategoryService(
                _jobCategoryRepositoryMock.Object,
                _mapperMock.Object);

            // Act
            await service.DeleteAsync(1);

            // Assert
            _jobCategoryRepositoryMock.Verify(
                x => x.GetByIdAsync(1),
                Times.Once);

            _jobCategoryRepositoryMock.Verify(
                x => x.DeleteAsync(jobCategory),
                Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_JobCategoryDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            _jobCategoryRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((JobCategory?)null);

            var service = new JobCategoryService(
                _jobCategoryRepositoryMock.Object,
                _mapperMock.Object);

            // Act
            var action = async () =>
                await service.DeleteAsync(1);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(action);

            _jobCategoryRepositoryMock.Verify(
                x => x.GetByIdAsync(1),
                Times.Once);

            _jobCategoryRepositoryMock.Verify(
                x => x.DeleteAsync(It.IsAny<JobCategory>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ValidJobCategory_UpdatesJobCategory()
        {
            // Arrange
            var jobCategory = new JobCategory
            {
                Id = 1,
                Name = "Software"
            };

            var updateJobCategoryDto = new UpdateJobCategoryDto
            {
                Name = "New Software"
            };

            _jobCategoryRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(jobCategory);

            _jobCategoryRepositoryMock
                .Setup(x => x.ExistsByNameAsync(updateJobCategoryDto.Name, 1))
                .ReturnsAsync(false);

            _mapperMock
                .Setup(x => x.Map(updateJobCategoryDto, jobCategory))
                .Callback<UpdateJobCategoryDto, JobCategory>((dto, entity) =>
                {
                    entity.Name = dto.Name;
                });

            var service = new JobCategoryService(
                _jobCategoryRepositoryMock.Object,
                _mapperMock.Object);

            // Act
            await service.UpdateAsync(1, updateJobCategoryDto);

            // Assert
            Assert.Equal("New Software", jobCategory.Name);

            _jobCategoryRepositoryMock.Verify(
                x => x.GetByIdAsync(1),
                Times.Once());

            _jobCategoryRepositoryMock.Verify(
                x => x.UpdateAsync(jobCategory),
                Times.Once());

            _jobCategoryRepositoryMock.Verify(
                x => x.ExistsByNameAsync(updateJobCategoryDto.Name, 1),
                Times.Once());

            _mapperMock.Verify(
                x => x.Map(updateJobCategoryDto, jobCategory),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_JobCategoryDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            var updateJobCategoryDto = new UpdateJobCategoryDto
            {
                Name = "Test"
            };

            _jobCategoryRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((JobCategory?)null);

            var service = new JobCategoryService(
                _jobCategoryRepositoryMock.Object,
                _mapperMock.Object);

            // Act
            var action = async () =>
                    await service.UpdateAsync(1, updateJobCategoryDto);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(action);

            _jobCategoryRepositoryMock.Verify(
                x => x.GetByIdAsync(1),
                Times.Once());

            _jobCategoryRepositoryMock.Verify(
                x => x.ExistsByNameAsync(updateJobCategoryDto.Name, 1),
                Times.Never());

            _jobCategoryRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<JobCategory>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_JobCategoryNameExists_ThrowsConflictException()
        {
            // Arrange
            var jobCategory = new JobCategory
            {
                Id = 1,
                Name = "Old Name"
            };

            var updateJobCategoryDto = new UpdateJobCategoryDto
            {
                Name = "Existing JobCategory"
            };

            _jobCategoryRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(jobCategory);

            _jobCategoryRepositoryMock
                .Setup(x => x.ExistsByNameAsync(updateJobCategoryDto.Name, 1))
                .ReturnsAsync(true);

            var service = new JobCategoryService(
                _jobCategoryRepositoryMock.Object,
                _mapperMock.Object);

            // Act
            var action = async () =>
                await service.UpdateAsync(1, updateJobCategoryDto);

            // Assert
            await Assert.ThrowsAsync<ConflictException>(action);

            _jobCategoryRepositoryMock.Verify(
                x => x.GetByIdAsync(1),
                Times.Once);

            _jobCategoryRepositoryMock.Verify(
                x => x.ExistsByNameAsync(updateJobCategoryDto.Name, 1),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map(updateJobCategoryDto, jobCategory),
                Times.Never);

            _jobCategoryRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<JobCategory>()),
                Times.Never);
        }
    }
}
