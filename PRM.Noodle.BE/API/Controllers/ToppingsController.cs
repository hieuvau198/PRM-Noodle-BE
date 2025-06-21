using Microsoft.AspNetCore.Mvc;
using Services.DTOs.Topping;
using Services.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class ToppingsController : ControllerBase
{
    private readonly IToppingService _service;

    public ToppingsController(IToppingService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var toppings = await _service.GetAllAsync();
        return Ok(toppings);
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailable()
    {
        var toppings = await _service.GetAvailableAsync();
        return Ok(toppings);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var topping = await _service.GetByIdAsync(id);
        if (topping == null) return NotFound();
        return Ok(topping);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateToppingDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.ToppingId }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateToppingDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var updated = await _service.UpdateAsync(id, dto);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpPatch("{id}/is-available")]
    public async Task<IActionResult> PatchIsAvailable(int id, [FromBody] ToppingIsAvailableDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _service.PatchIsAvailableAsync(id, dto.IsAvailable);
        if (!result) return NotFound();
        return NoContent();
    }
}
