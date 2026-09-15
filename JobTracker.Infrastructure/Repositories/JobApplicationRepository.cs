using JobTracker.Application.DTOs.Common;
using JobTracker.Application.DTOs.DashboardDtos;
using JobTracker.Application.DTOs.JobApplicationDtos;
using JobTracker.Application.Interfaces.Repository;
using JobTracker.Domain.Entities;
using JobTracker.Domain.Enums;
using JobTracker.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Infrastructure.Repositories
{
    public class JobApplicationRepository : IJobApplicationRepository
    {
        private readonly AppDbContext _context;

        public JobApplicationRepository(AppDbContext context)
        {
            _context = context;
        }


        public async Task<List<JobApplication>> GetAllAsync()
        {
            return await _context.JobApplications.ToListAsync();
        }

        public async Task<JobApplication?> GetByIdAsync(long id)
        {
            return await _context.JobApplications.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task CreateAsync(JobApplication jobApplication)
        {
            await _context.JobApplications.AddAsync(jobApplication);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(JobApplication jobApplication)
        {
            _context.JobApplications.Update(jobApplication);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(JobApplication jobApplication)
        {
            _context.JobApplications.Remove(jobApplication);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.JobApplications.AnyAsync(a => a.Id == id);
        }

        public async Task<PagedResultDto<JobApplication>> GetAllByUserIdAsync(long userId, JobApplicationQueryDto query)
        {
            var applications = _context.JobApplications
                .AsNoTracking()
                .Where(x => x.UserId == userId);

            // Search
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim();

                applications = applications.Where(x =>
                    x.Position.Contains(search) ||
                    x.Company.Name.Contains(search) ||
                    x.JobCategory.Name.Contains(search));
            }

            // Filters
            if (query.Status.HasValue)
            {
                applications = applications.Where(x =>
                    x.Status == query.Status.Value);
            }

            if (query.WorkPlace.HasValue)
            {
                applications = applications.Where(x =>
                    x.WorkPlace == query.WorkPlace.Value);
            }

            if (query.CompanyId.HasValue)
            {
                applications = applications.Where(x =>
                    x.CompanyId == query.CompanyId.Value);
            }

            if (query.JobCategoryId.HasValue)
            {
                applications = applications.Where(x =>
                    x.JobCategoryId == query.JobCategoryId.Value);
            }

            if (query.ApplicationDateFrom.HasValue)
            {
                applications = applications.Where(x =>
                    x.ApplicationDate >= query.ApplicationDateFrom.Value);
            }

            if (query.ApplicationDateTo.HasValue)
            {
                applications = applications.Where(x =>
                    x.ApplicationDate <= query.ApplicationDateTo.Value);
            }

            var totalCount = await applications.CountAsync();

            var pageNumber = query.PageNumber < 1 ? 1 : query.PageNumber;
            var pageSize = query.PageSize <= 0 ? 10 : query.PageSize;

            applications = query.SortBy?.ToLower() switch
            {
                "position" => query.SortDescending
                    ? applications.OrderByDescending(x => x.Position)
                    : applications.OrderBy(x => x.Position),

                "salary" => query.SortDescending
                    ? applications.OrderByDescending(x => x.Salary)
                    : applications.OrderBy(x => x.Salary),

                "status" => query.SortDescending
                    ? applications.OrderByDescending(x => x.Status)
                    : applications.OrderBy(x => x.Status),

                "applicationdate" => query.SortDescending
                    ? applications.OrderByDescending(x => x.ApplicationDate)
                    : applications.OrderBy(x => x.ApplicationDate),

                _ => applications.OrderByDescending(x => x.ApplicationDate)
            };

            var items = await applications
                .Include(x => x.Company)
                .Include(x => x.JobCategory)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResultDto<JobApplication>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<DashboardDto> GetDashboardAsync(long userId)
        {
            var applications = _context.JobApplications
                .AsNoTracking()
                .Where(x => x.UserId == userId);

            return new DashboardDto
            {
                TotalApplications = await applications.CountAsync(),

                Saved = await applications.CountAsync(
                    x => x.Status == JobApplicationStatus.Saved),

                Applied = await applications.CountAsync(
                    x => x.Status == JobApplicationStatus.Applied),

                Interviews = await applications.CountAsync(
                    x => x.Status == JobApplicationStatus.TechnicalInterview ||
                         x.Status == JobApplicationStatus.HRScreening ||
                         x.Status == JobApplicationStatus.FinalInterview),

                Offers = await applications.CountAsync(
                    x => x.Status == JobApplicationStatus.Offer),

                Rejected = await applications.CountAsync(
                    x => x.Status == JobApplicationStatus.Rejected),

                Withdrawn = await applications.CountAsync(
                    x => x.Status == JobApplicationStatus.Withdrawn),

                Remote = await applications.CountAsync(
                    x => x.WorkPlace == WorkPlace.Remote),

                Hybrid = await applications.CountAsync(
                    x => x.WorkPlace == WorkPlace.Hybrid),

                OnSite = await applications.CountAsync(
                    x => x.WorkPlace == WorkPlace.OnSite)
            };
        }
    }
}
