using JobTracker.Application.Interfaces.Repository;
using JobTracker.Domain.Entities;
using JobTracker.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Infrastructure.Repositories
{
    public class JobCategoryRepository : IJobCategoryRepository
    {
        private readonly AppDbContext _context;

        public JobCategoryRepository(AppDbContext context)
        {
            _context = context;
        }


        public async Task<List<JobCategory>> GetAllAsync()
        {
            return await _context.JobCategories.ToListAsync();
        }

        public async Task<JobCategory?> GetByIdAsync(long id)
        {
            return await _context.JobCategories.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task CreateAsync(JobCategory jobCategory)
        {
            await _context.JobCategories.AddAsync(jobCategory);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(JobCategory jobCategory)
        {
            _context.JobCategories.Update(jobCategory);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(JobCategory jobCategory)
        {
            _context.JobCategories.Remove(jobCategory);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByNameAsync(string name, long? excludeId = null)
        {
            return await _context.JobCategories.AnyAsync(n => n.Name == name && (n.Id != excludeId || excludeId == null));
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.JobCategories.AnyAsync(a => a.Id == id);
        }
    }
}
