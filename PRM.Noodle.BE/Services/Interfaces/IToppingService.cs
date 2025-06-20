using Services.DTOs.Topping;

public interface IToppingService
{
    Task<IEnumerable<ToppingDto>> GetAllAsync();
    Task<ToppingDto> GetByIdAsync(int id);
    Task<ToppingDto> CreateAsync(CreateToppingDto dto);
    Task<ToppingDto> UpdateAsync(int id, UpdateToppingDto dto);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<ToppingDto>> GetAvailableAsync();
    Task<bool> PatchIsAvailableAsync(int id, bool isAvailable);
}