using JobTracker.Application.Interfaces;
using JobTracker.Domain.Entities;
using JobTracker.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Infrastructure.Repositories
{
    public class InterviewRepository : IInterviewRepository
    {
        private readonly AppDbContext _context;

        public InterviewRepository(AppDbContext context)
        {
            _context = context;
        }


        public async Task<List<Interview>> GetAllAsync()
        {
            return await _context.Interviews.ToListAsync();
        }

        public async Task<Interview?> GetByIdAsync(long id)
        {
            return await _context.Interviews.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task CreateAsync(Interview interview)
        {
            await _context.Interviews.AddAsync(interview);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Interview interview)
        {
            _context.Interviews.Update(interview);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Interview interview)
        {
            _context.Interviews.Remove(interview);
            await _context.SaveChangesAsync();
        }
    }
}
