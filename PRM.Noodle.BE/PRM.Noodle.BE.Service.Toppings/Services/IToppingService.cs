using PRM.Noodle.BE.Service.Toppings.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Toppings.Services
{
    public interface IToppingService
    {
        Task<IEnumerable<ToppingDto>> GetAllAsync();
        Task<ToppingDto> GetByIdAsync(int id);
        Task<ToppingDto> CreateAsync(CreateToppingDto dto);
        Task<ToppingDto> UpdateAsync(int id, UpdateToppingDto dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<ToppingDto>> GetAvailableAsync();
        Task<bool> PatchIsAvailableAsync(int id, bool isAvailable);
        Task<(IEnumerable<ToppingDto> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string searchTerm = null, bool? isAvailable = null);
    }
}
