using PRM.Noodle.BE.Service.Combos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Combos.Services
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
        Task<(IEnumerable<ComboDto> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string searchTerm = null, bool? isAvailable = null);

    }
}
