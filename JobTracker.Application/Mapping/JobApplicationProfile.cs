using AutoMapper;
using JobTracker.Application.DTOs.JobApplicationDtos;
using JobTracker.Domain.Entities;

namespace JobTracker.Application.Mapping
{
    public class JobApplicationProfile : Profile
    {
        public JobApplicationProfile()
        {
            CreateMap<JobApplication, JobApplicationDto>();
            CreateMap<CreateJobApplicationDto, JobApplication>();
            CreateMap<UpdateJobApplicationDto, JobApplication>();
        }
    }
}
