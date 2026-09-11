using AutoMapper;
using JobTracker.Application.DTOs.InterviewDtos;
using JobTracker.Application.Exceptions;
using JobTracker.Application.Interfaces.Repository;
using JobTracker.Application.Interfaces.Service;
using JobTracker.Domain.Entities;

namespace JobTracker.Application.Services
{
    public class InterviewService : IInterviewService
    {
        private readonly IInterviewRepository _repository;
        private readonly IMapper _mapper;
        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly IUserContext _userContext;

        public InterviewService(
            IInterviewRepository repository,
            IMapper mapper,
            IJobApplicationRepository jobApplicationRepository,
            IUserContext userContext)
        {
            _repository = repository;
            _mapper = mapper;
            _jobApplicationRepository = jobApplicationRepository;
            _userContext = userContext;
        }

        public async Task<List<InterviewDto>> GetAllAsync()
        {
            var userId = _userContext.UserId;

            var result = await _repository.GetAllByUserIdAsync(userId);

            return _mapper.Map<List<InterviewDto>>(result);
        }

        public async Task<InterviewDto?> GetByIdAsync(long id)
        {
            var userId = _userContext.UserId;

            var result = await _repository.GetByIdAsync(id);

            if (result == null)
                throw new NotFoundException(
                    $"Interview with id {id} was not found.");

            var jobApplication =
                await _jobApplicationRepository.GetByIdAsync(
                    result.JobApplicationId);

            if (jobApplication == null ||
                jobApplication.UserId != userId)
            {
                throw new NotFoundException(
                    $"Interview with id {id} was not found.");
            }

            return _mapper.Map<InterviewDto>(result);
        }

        public async Task<InterviewDto> CreateAsync(
            CreateInterviewDto dto)
        {
            var userId = _userContext.UserId;

            var jobApplication =
                await _jobApplicationRepository.GetByIdAsync(
                    dto.JobApplicationId);

            if (jobApplication == null ||
                jobApplication.UserId != userId)
            {
                throw new NotFoundException(
                    $"Job application with id {dto.JobApplicationId} was not found.");
            }

            var result = _mapper.Map<Interview>(dto);

            await _repository.CreateAsync(result);

            return _mapper.Map<InterviewDto>(result);
        }

        public async Task UpdateAsync(
            long id,
            UpdateInterviewDto dto)
        {
            var userId = _userContext.UserId;

            var result = await _repository.GetByIdAsync(id);

            if (result == null)
                throw new NotFoundException(
                    $"Interview with id {id} was not found.");

            var currentJobApplication =
                await _jobApplicationRepository.GetByIdAsync(
                    result.JobApplicationId);

            if (currentJobApplication == null ||
                currentJobApplication.UserId != userId)
            {
                throw new NotFoundException(
                    $"Interview with id {id} was not found.");
            }

            var newJobApplication =
                await _jobApplicationRepository.GetByIdAsync(
                    dto.JobApplicationId);

            if (newJobApplication == null ||
                newJobApplication.UserId != userId)
            {
                throw new NotFoundException(
                    $"Job application with id {dto.JobApplicationId} was not found.");
            }

            _mapper.Map(dto, result);

            await _repository.UpdateAsync(result);
        }

        public async Task DeleteAsync(long id)
        {
            var userId = _userContext.UserId;

            var result = await _repository.GetByIdAsync(id);

            if (result == null)
                throw new NotFoundException(
                    $"Interview with id {id} was not found.");

            var jobApplication =
                await _jobApplicationRepository.GetByIdAsync(
                    result.JobApplicationId);

            if (jobApplication == null ||
                jobApplication.UserId != userId)
            {
                throw new NotFoundException(
                    $"Interview with id {id} was not found.");
            }

            await _repository.DeleteAsync(result);
        }
    }
}