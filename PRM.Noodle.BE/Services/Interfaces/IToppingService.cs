//using Services.DTOs.Topping;

//namespace Services.Interfaces
//{
//    public interface IToppingService
//    {
//        Task<IEnumerable<ToppingDto>> GetAllAsync();
//        Task<ToppingDto> GetByIdAsync(int id);
//        Task<ToppingDto> CreateAsync(CreateToppingDto dto);
//        Task<ToppingDto> UpdateAsync(int id, UpdateToppingDto dto);
//        Task<bool> DeleteAsync(int id);
//        Task<IEnumerable<ToppingDto>> GetAvailableAsync();
//        Task<bool> PatchIsAvailableAsync(int id, bool isAvailable);
//        Task<(IEnumerable<ToppingDto> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string searchTerm = null, bool? isAvailable = null);
//    }
//}