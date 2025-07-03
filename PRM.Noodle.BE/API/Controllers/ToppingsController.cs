using Microsoft.AspNetCore.Mvc;
using Services.DTOs.Topping;
using Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ToppingsController : ControllerBase
    {
        private readonly IToppingService _service;

        public ToppingsController(IToppingService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all toppings
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ToppingDto>>> GetAll()
        {
            var toppings = await _service.GetAllAsync();
            return Ok(toppings);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedToppingResponse>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool? isAvailable = null)
        {
            var (items, totalCount) = await _service.GetPagedAsync(page, pageSize, searchTerm, isAvailable);
            var response = new PagedToppingResponse
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
            return Ok(response);
        }

        /// <summary>
        /// Get available toppings only
        /// </summary>
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<ToppingDto>>> GetAvailable()
        {
            var toppings = await _service.GetAvailableAsync();
            return Ok(toppings);
        }

        /// <summary>
        /// Get topping by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ToppingDto>> GetById(int id)
        {
            var topping = await _service.GetByIdAsync(id);
            if (topping == null) return NotFound();
            return Ok(topping);
        }

        /// <summary>
        /// Create new topping
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ToppingDto>> Create([FromBody] CreateToppingDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.ToppingId }, created);
        }

        /// <summary>
        /// Update topping
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ToppingDto>> Update(int id, [FromBody] UpdateToppingDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        /// <summary>
        /// Delete topping
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Patch isAvailable for topping
        /// </summary>
        [HttpPatch("{id}/is-available")]
        public async Task<IActionResult> PatchIsAvailable(int id, [FromBody] ToppingIsAvailableDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _service.PatchIsAvailableAsync(id, dto.IsAvailable);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}