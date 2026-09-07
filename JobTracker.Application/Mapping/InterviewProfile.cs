using AutoMapper;
using JobTracker.Application.DTOs.InterviewDtos;
using JobTracker.Domain.Entities;

namespace JobTracker.Application.Mapping
{
    public class InterviewProfile : Profile
    {
        public InterviewProfile()
        {
            CreateMap<Interview, InterviewDto>();
            CreateMap<CreateInterviewDto, Interview>();
            CreateMap<UpdateInterviewDto, Interview>();
        }
    }
}
