using Microsoft.AspNetCore.Mvc;
using Services.DTOs.Combo;
using Services.DTOs.Product;
using Services.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class ComboController : ControllerBase
{
    private readonly IComboService _service;

    public ComboController(IComboService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var combos = await _service.GetAllAsync();
        return Ok(combos);
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableCombos()
    {
        var combos = await _service.GetAvailableAsync();
        return Ok(combos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var combo = await _service.GetByIdAsync(id);
        if (combo == null) return NotFound();
        return Ok(combo);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateComboDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.ComboId }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateComboDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
}
