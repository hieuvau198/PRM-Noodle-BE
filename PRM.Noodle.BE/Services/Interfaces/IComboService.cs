using Services.DTOs.Combo;

namespace Services.Interfaces
{
    public interface IComboService
    {
        Task<IEnumerable<ComboDto>> GetAllAsync();
        Task<ComboDto> GetByIdAsync(int id);
        Task<ComboDto> CreateAsync(CreateComboDto dto);
        Task<bool> UpdateAsync(int id, UpdateComboDto dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<ComboDto>> GetAvailableAsync();
        Task<bool> PatchIsAvailableAsync(int comboId, bool isAvailable);

    }
}
