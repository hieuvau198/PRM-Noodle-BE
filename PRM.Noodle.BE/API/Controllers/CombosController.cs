using Microsoft.AspNetCore.Mvc;
using Services.DTOs.Combo;
using Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CombosController : ControllerBase
    {
        private readonly IComboService _service;

        public CombosController(IComboService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all combos
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ComboDto>>> GetAll()
        {
            var combos = await _service.GetAllAsync();
            return Ok(combos);
        }

        /// <summary>
        /// Get all available combos
        /// </summary>
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<ComboDto>>> GetAvailableCombos()
        {
            var combos = await _service.GetAvailableAsync();
            return Ok(combos);
        }

        /// <summary>
        /// Get combo by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ComboDto>> GetById(int id)
        {
            var combo = await _service.GetByIdAsync(id);
            if (combo == null) return NotFound();
            return Ok(combo);
        }

        /// <summary>
        /// Create new combo
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ComboDto>> Create([FromBody] CreateComboDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.ComboId }, created);
        }

        /// <summary>
        /// Update combo
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ComboDto>> Update(int id, [FromBody] UpdateComboDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            if (!updated) return NotFound();
            // Optional: Trả về combo vừa update, hoặc chỉ NoContent()
            return NoContent();
        }

        /// <summary>
        /// Patch isAvailable for combo
        /// </summary>
        [HttpPatch("{id}/is-available")]
        public async Task<IActionResult> PatchIsAvailable(int id, [FromBody] ComboIsAvailableDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.PatchIsAvailableAsync(id, dto.IsAvailable);
            if (!result)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Delete combo
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}