using AutoMapper;
using JobTracker.Application.DTOs.JobCategoryDtos;
using JobTracker.Domain.Entities;

namespace JobTracker.Application.Mapping
{
    public class JobCategoryProfile : Profile
    {
        public JobCategoryProfile()
        {
            CreateMap<JobCategory, JobCategoryDto>();
            CreateMap<CreateJobCategoryDto, JobCategory>();
            CreateMap<UpdateJobCategoryDto, JobCategory>();
        }
    }
}
