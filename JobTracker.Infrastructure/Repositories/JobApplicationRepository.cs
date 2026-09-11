using JobTracker.Application.Interfaces.Repository;
using JobTracker.Domain.Entities;
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

        public async Task<List<JobApplication>> GetAllByUserIdAsync(long userId)
        {
            return await _context.JobApplications.Where(a => a.UserId == userId).ToListAsync();
        }
    }
}
