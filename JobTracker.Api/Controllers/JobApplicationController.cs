using JobTracker.Application.DTOs.JobApplicationDtos;
using JobTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobTracker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class JobApplicationController : ControllerBase
    {
        private readonly IJobApplicationService _service;

        public JobApplicationController(IJobApplicationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateJobApplicationDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(
                        nameof(GetById),
                        new { id = result.Id },
                        result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(long id, [FromBody] UpdateJobApplicationDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
