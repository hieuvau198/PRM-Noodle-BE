using PRM.Noodle.BE.Service.Products.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Products.Services
{
    public interface IProductsService
    {
        Task<ProductDto> GetByIdAsync(int id);
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<IEnumerable<ProductDto>> GetAvailableAsync();
        Task<(IEnumerable<ProductDto> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string searchTerm = null,
            string spiceLevel = null,
            bool? isAvailable = null);
        Task<ProductDto> CreateAsync(CreateProductDto createProductDto);
        Task<ProductDto> UpdateAsync(int id, UpdateProductDto updateProductDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<ProductDto>> GetBySpiceLevelAsync(string spiceLevel);
        Task<bool> PatchIsAvailableAsync(int productId, bool isAvailable);
    }
}
