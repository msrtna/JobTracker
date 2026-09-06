using AutoMapper;
using JobTracker.Application.DTOs.CompanyDtos;
using JobTracker.Domain.Entities;

namespace JobTracker.Application.Mapping
{
    public class CompanyProfile : Profile
    {
        public CompanyProfile()
        {
            CreateMap<Company, CompanyDto>();
            CreateMap<CreateCompanyDto, Company>();
            CreateMap<UpdateCompanyDto, Company>();
        }
    }
}
