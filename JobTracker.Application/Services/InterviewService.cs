using AutoMapper;
using JobTracker.Application.DTOs.InterviewDtos;
using JobTracker.Application.Exceptions;
using JobTracker.Application.Interfaces;
using JobTracker.Domain.Entities;

namespace JobTracker.Application.Services
{
    public class InterviewService : IInterviewService
    {
        private readonly IInterviewRepository _repository;
        private readonly IMapper _mapper;

        public InterviewService(IInterviewRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }


        public async Task<List<InterviewDto>> GetAllAsync()
        {
            var result = await _repository.GetAllAsync();
            return _mapper.Map<List<InterviewDto>>(result);
        }

        public async Task<InterviewDto?> GetByIdAsync(long id)
        {
            var result = await _repository.GetByIdAsync(id);
            if (result == null)
                throw new NotFoundException($"Interview with id {id} was not found.");
            return _mapper.Map<InterviewDto>(result);
        }

        public async Task<InterviewDto> CreateAsync(CreateInterviewDto dto)
        {
            var result = _mapper.Map<Interview>(dto);
            await _repository.CreateAsync(result);
            return _mapper.Map<InterviewDto>(result);
        }

        public async Task UpdateAsync(long id, UpdateInterviewDto dto)
        {
            var result = await _repository.GetByIdAsync(id);

            if (result == null)
                throw new NotFoundException($"Interview with id {id} was not found.");

            _mapper.Map(dto, result);
            await _repository.UpdateAsync(result);
        }

        public async Task DeleteAsync(long id)
        {
            var result = await _repository.GetByIdAsync(id);
            if (result == null)
                throw new NotFoundException($"Interview with id {id} was not found.");

            await _repository.DeleteAsync(result);
        }
    }
}
