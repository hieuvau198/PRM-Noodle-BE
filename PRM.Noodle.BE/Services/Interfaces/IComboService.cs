using Services.DTOs.Combo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
