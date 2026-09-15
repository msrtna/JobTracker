using AutoMapper;
using JobTracker.Application.DTOs.CompanyDtos;
using JobTracker.Application.Exceptions;
using JobTracker.Application.Interfaces.Repository;
using JobTracker.Application.Services;
using JobTracker.Domain.Entities;
using Moq;

namespace JobTracker.Tests.UnitTests
{
    public class CompanyServiceTests
    {
        private readonly Mock<ICompanyRepository> _companyRepositoryMock = new();
        private readonly Mock<IMapper> _mapperMock = new();

        [Fact]
        public async Task GetAllAsync_ReturnsCompanies()
        {
            // Arrange
            var company = new List<Company>
            {
                new Company
                {
                    Id = 1,
                    Name = "Test1",
                    Website = "https://test1.com"
                },
                new Company
                {
                    Id = 2,
                    Name = "Test2",
                    Website = "https://test2.com"
                }
            };

            _companyRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(company);

            var companyDto = new List<CompanyDto>
            {
                new CompanyDto
                {
                    Id = 1,
                    Name = "Test1",
                    Website = "https://test1.com"
                },
                new CompanyDto
                {
                    Id = 2,
                    Name = "Test2",
                    Website = "https://test2.com"
                }
            };

            _mapperMock
                .Setup(x=> x.Map<List<CompanyDto>>(company))
                .Returns(companyDto);

            var service = new CompanyService(
                _companyRepositoryMock.Object,
                _mapperMock.Object);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Test1", result[0].Name);
            Assert.Equal("https://test1.com", result[0].Website);

            _companyRepositoryMock.Verify(
                x => x.GetAllAsync(),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<List<CompanyDto>>(company),
                Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_CompanyExists_ReturnsCompanyDto()
        {
            // Arrange
            var company = new Company
            {
                Id = 1,
                Name = "Test1",
                Website = "https://test1.com"
            };

            _companyRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(company);

            var companyDto = new CompanyDto
            {
                Id = 1,
                Name = "Test1",
                Website = "https://test1.com"
            };

            _mapperMock
                .Setup(x=> x.Map<CompanyDto>(company))
                .Returns(companyDto);

            var service = new CompanyService(
                _companyRepositoryMock.Object,
                _mapperMock.Object);

            // Act
            var result = await service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Test1", result.Name);

            _companyRepositoryMock.Verify(
                x => x.GetByIdAsync(1),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<CompanyDto>(company),
                Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_CompanyDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            _companyRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((Company?)null);

            var service = new CompanyService(
                _companyRepositoryMock.Object,
                _mapperMock.Object);

            // Act
            var action = async () =>
                await service.GetByIdAsync(1);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(action);

            _mapperMock.Verify(
                x => x.Map<CompanyDto>(
                    It.IsAny<Company>()),
                Times.Never);

            _companyRepositoryMock.Verify(
                x => x.GetByIdAsync(1),
                Times.Once);
        }

        [Fact]
        public async Task CreateCompanyAsync_ValidCompany_ReturnsCompanyDto()
        {
            // Arrange
            var createCompanyDto = new CreateCompanyDto
            {
                Name = "Test",
                Website = "http://test.com"
            };

            var company = new Company
            {
                Id = 1,
                Name = "Test",
                Website = "http://test.com"
            };

            var companyDto = new CompanyDto
            {
                Id = 1,
                Name = "Test",
                Website = "http://test.com"
            };

            _companyRepositoryMock
                .Setup(x => x.ExistsByNameAsync(createCompanyDto.Name, null))
                .ReturnsAsync(false);

            _companyRepositoryMock
                .Setup(x => x.ExistsByWebsiteAsync(createCompanyDto.Website, null))
                .ReturnsAsync(false);

            _mapperMock
                .Setup(x => x.Map<Company>(createCompanyDto))
                .Returns(company);

            _mapperMock
                .Setup(x => x.Map<CompanyDto>(company))
                .Returns(companyDto);

            var service = new CompanyService(
                _companyRepositoryMock.Object,
                _mapperMock.Object);

            // Act
            var result = await service.CreateAsync(createCompanyDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test", result.Name);
            Assert.Equal("http://test.com", result.Website);

            _companyRepositoryMock.Verify(
                x=> x.CreateAsync(company),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<Company>(createCompanyDto),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<CompanyDto>(company),
                Times.Once);

            _companyRepositoryMock.Verify(
                x => x.ExistsByNameAsync(createCompanyDto.Name, null),
                Times.Once);

            _companyRepositoryMock.Verify(
                x => x.ExistsByWebsiteAsync(createCompanyDto.Website, null),
                Times.Once);
        }

        [Fact]
        public async Task CreateCompanyAsync_CompanyNameExists_ThrowsConflictException()
        {
            // Arrange
            var createCompanyDto = new CreateCompanyDto
            {
                Name = "Test",
                Website = "http://test.com"
            };

            _companyRepositoryMock
                .Setup(x => x.ExistsByNameAsync(createCompanyDto.Name, null))
                .ReturnsAsync(true);

            var service = new CompanyService(
                _companyRepositoryMock.Object,
                _mapperMock.Object);

            // Act
            var action = async () =>
                await service.CreateAsync(createCompanyDto);

            // Assert
            await Assert.ThrowsAsync<ConflictException>(action);

            _mapperMock.Verify(
                x => x.Map<Company>(
                    It.IsAny<CreateCompanyDto>()),
                Times.Never);

            _companyRepositoryMock.Verify(
                x => x.CreateAsync(
                    It.IsAny<Company>()),
                Times.Never);

            _companyRepositoryMock.Verify(
                x => x.ExistsByNameAsync(createCompanyDto.Name, null),
                Times.Once);
        }

        [Fact]
        public async Task CreateCompanyAsync_CompanyWebsiteExists_ThrowsConflictException()
        {
            // Arrange
            var createCompanyDto = new CreateCompanyDto
            {
                Name = "Test",
                Website = "http://test.com"
            };

            _companyRepositoryMock
                .Setup(x => x.ExistsByWebsiteAsync(createCompanyDto.Website, null))
                .ReturnsAsync(true);

            var service = new CompanyService(
                _companyRepositoryMock.Object,
                _mapperMock.Object);

            // Act
            var action = async () =>
                await service.CreateAsync(createCompanyDto);

            // Assert
            await Assert.ThrowsAsync<ConflictException>(action);

            _mapperMock.Verify(
                x => x.Map<Company>(
                    It.IsAny<CreateCompanyDto>()),
                Times.Never);

            _companyRepositoryMock.Verify(
                x => x.CreateAsync(
                    It.IsAny<Company>()),
                Times.Never);

            _companyRepositoryMock.Verify(
                x => x.ExistsByWebsiteAsync(createCompanyDto.Website, null),
                Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ValidCompany_DeletesCompany()
        {
            // Arrange
            var company = new Company
            {
                Id = 1,
                Name = "Test",
                Website = "http://test.com"
            };

            _companyRepositoryMock
                .Setup(x=> x.GetByIdAsync(1))
                .ReturnsAsync(company);

            var service = new CompanyService(
                _companyRepositoryMock.Object,
                _mapperMock.Object);

            // Act
            await service.DeleteAsync(1);

            // Assert
            _companyRepositoryMock.Verify(
                x => x.GetByIdAsync(1),
                Times.Once);

            _companyRepositoryMock.Verify(
                x => x.DeleteAsync(company),
                Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_CompanyDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            _companyRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((Company?)null);

            var service = new CompanyService(
                _companyRepositoryMock.Object,
                _mapperMock.Object);

            // Act
            var action = async () =>
                await service.DeleteAsync(1);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(action);

            _companyRepositoryMock.Verify(
                x => x.GetByIdAsync(1),
                Times.Once);

            _companyRepositoryMock.Verify(
                x => x.DeleteAsync(It.IsAny<Company>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ValidCompany_UpdatesCompany()
        {
            // Arrange
            var company = new Company
            {
                Id = 1,
                Name = "Test",
                Website = "http://test.com"
            };

            var updateCompanyDto = new UpdateCompanyDto
            {
                Name = "New Test",
                Website = "https://newtest.com"
            };

            _companyRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(company);

            _companyRepositoryMock
                .Setup(x => x.ExistsByNameAsync(updateCompanyDto.Name, 1))
                .ReturnsAsync(false);

            _companyRepositoryMock
                .Setup(x => x.ExistsByWebsiteAsync(updateCompanyDto.Website, 1))
                .ReturnsAsync(false);

            _mapperMock
                .Setup(x => x.Map(updateCompanyDto, company))
                .Callback<UpdateCompanyDto, Company>((dto, entity) =>
                {
                    entity.Name = dto.Name;
                    entity.Website = dto.Website;
                });

            var service = new CompanyService(
                _companyRepositoryMock.Object,
                _mapperMock.Object);

            // Act
            await service.UpdateAsync(1, updateCompanyDto);

            // Assert
            Assert.Equal("New Test", company.Name);
            Assert.Equal("https://newtest.com", company.Website);

            _companyRepositoryMock.Verify(
                x=> x.GetByIdAsync(1),
                Times.Once());

            _companyRepositoryMock.Verify(
                x=> x.UpdateAsync(company),
                Times.Once());

            _companyRepositoryMock.Verify(
                x=> x.ExistsByNameAsync(updateCompanyDto.Name, 1),
                Times.Once());

            _companyRepositoryMock.Verify(
                x=> x.ExistsByWebsiteAsync(updateCompanyDto.Website, 1),
                Times.Once());

            _mapperMock.Verify(
                x => x.Map(updateCompanyDto, company),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_CompanyDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            var updateCompanyDto = new UpdateCompanyDto
            {
                Name = "Test",
                Website = "http://test.com"
            };

            _companyRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((Company?)null);

            var service = new CompanyService(
                _companyRepositoryMock.Object,
                _mapperMock.Object);

            // Act
            var action = async () =>
                    await service.UpdateAsync(1, updateCompanyDto);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(action);

            _companyRepositoryMock.Verify(
                x => x.GetByIdAsync(1),
                Times.Once());

            _companyRepositoryMock.Verify(
                x => x.ExistsByNameAsync(updateCompanyDto.Name, 1),
                Times.Never());

            _companyRepositoryMock.Verify(
                x => x.ExistsByWebsiteAsync(
                    updateCompanyDto.Website, 1),
                Times.Never);

            _companyRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<Company>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_CompanyNameExists_ThrowsConflictException()
        {
            // Arrange
            var company = new Company
            {
                Id = 1,
                Name = "Old Name",
                Website = "https://old.com"
            };

            var updateCompanyDto = new UpdateCompanyDto
            {
                Name = "Existing Company",
                Website = "https://new.com"
            };

            _companyRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(company);

            _companyRepositoryMock
                .Setup(x => x.ExistsByNameAsync(updateCompanyDto.Name, 1))
                .ReturnsAsync(true);

            var service = new CompanyService(
                _companyRepositoryMock.Object,
                _mapperMock.Object);

            // Act
            var action = async () =>
                await service.UpdateAsync(1, updateCompanyDto);

            // Assert
            await Assert.ThrowsAsync<ConflictException>(action);

            _companyRepositoryMock.Verify(
                x => x.GetByIdAsync(1),
                Times.Once);

            _companyRepositoryMock.Verify(
                x => x.ExistsByNameAsync(updateCompanyDto.Name, 1),
                Times.Once);

            _companyRepositoryMock.Verify(
                x => x.ExistsByWebsiteAsync(
                    updateCompanyDto.Website, 1),
                Times.Never);

            _mapperMock.Verify(
                x => x.Map(updateCompanyDto, company),
                Times.Never);

            _companyRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<Company>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_CompanyWebsiteExists_ThrowsConflictException()
        {
            // Arrange
            var company = new Company
            {
                Id = 1,
                Name = "Old Name",
                Website = "https://old.com"
            };

            var updateCompanyDto = new UpdateCompanyDto
            {
                Name = "Existing Company",
                Website = "https://new.com"
            };

            _companyRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(company);

            _companyRepositoryMock
                .Setup(x => x.ExistsByNameAsync(updateCompanyDto.Name, 1))
                .ReturnsAsync(false);

            _companyRepositoryMock
                .Setup(x => x.ExistsByWebsiteAsync(updateCompanyDto.Website, 1))
                .ReturnsAsync(true);

            var service = new CompanyService(
                _companyRepositoryMock.Object,
                _mapperMock.Object);

            // Act
            var action = async () =>
                await service.UpdateAsync(1, updateCompanyDto);

            // Assert
            await Assert.ThrowsAsync<ConflictException>(action);

            _companyRepositoryMock.Verify(
                x => x.GetByIdAsync(1),
                Times.Once);

            _companyRepositoryMock.Verify(
                x => x.ExistsByNameAsync(updateCompanyDto.Name, 1),
                Times.Once);

            _companyRepositoryMock.Verify(
                x => x.ExistsByWebsiteAsync(
                    updateCompanyDto.Website, 1),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map(updateCompanyDto, company),
                Times.Never);

            _companyRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<Company>()),
                Times.Never);
        }
    }
}
