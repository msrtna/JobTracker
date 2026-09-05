using JobTracker.Application.Interfaces;
using JobTracker.Domain.Entities;
using JobTracker.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Infrastructure.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly AppDbContext _context;

        public CompanyRepository(AppDbContext context)
        {
            _context = context;
        }


        public async Task<List<Company>> GetAllAsync()
        {
            return await _context.Companies.ToListAsync();
        }

        public async Task<Company?> GetByIdAsync(long id)
        {
            return await _context.Companies.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task CreateAsync(Company company)
        {
            await _context.Companies.AddAsync(company);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Company company)
        {
            _context.Companies.Update(company);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Company company)
        {
            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();
        }
    }
}
