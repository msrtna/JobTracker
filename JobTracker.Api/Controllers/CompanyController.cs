using JobTracker.Application.DTOs.CompanyDtos;
using JobTracker.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JobTracker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var company = await _companyService.GetAllAsync();
            return Ok(company);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var company = await _companyService.GetByIdAsync(id);
            if (company == null)
                return NotFound();
            return Ok(company);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateCompanyDto dto)
        {
            var company = await _companyService.CreateAsync(dto);
            return CreatedAtAction(
                        nameof(GetById),
                        new { id = company.Id },
                        company);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(long id, [FromBody] UpdateCompanyDto dto)
        {
            var company = await _companyService.UpdateAsync(id, dto);
            if (!company)
                return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            var company = await _companyService.DeleteAsync(id);
            if (!company)
                return NotFound();
            return NoContent();
        }
    }
}
