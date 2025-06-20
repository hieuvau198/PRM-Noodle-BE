using AutoMapper;
using Repositories.Interfaces;
using Repositories.Models;
using Repositories.Persistence;
using Services.DTOs.Product;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ProductDto> GetByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {id} not found.");

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductDto>> GetAvailableAsync()
        {
            var products = await _unitOfWork.Products.FindAsync(p => p.IsAvailable == true);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<(IEnumerable<ProductDto> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string searchTerm = null,
            string spiceLevel = null,
            bool? isAvailable = null)
        {
            // Build filter expression
            Expression<Func<Product, bool>> filter = p => true;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var searchLower = searchTerm.ToLower();
                var searchFilter = new Func<Expression<Func<Product, bool>>, Expression<Func<Product, bool>>>(
                    expr => p => p.ProductName.ToLower().Contains(searchLower) ||
                                (p.Description != null && p.Description.ToLower().Contains(searchLower)));
                filter = filter.And(p => p.ProductName.ToLower().Contains(searchLower) ||
                                        (p.Description != null && p.Description.ToLower().Contains(searchLower)));
            }

            if (!string.IsNullOrWhiteSpace(spiceLevel))
            {
                filter = filter.And(p => p.SpiceLevel == spiceLevel);
            }

            if (isAvailable.HasValue)
            {
                filter = filter.And(p => p.IsAvailable == isAvailable.Value);
            }

            // Order by CreatedAt descending
            Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy =
                query => query.OrderByDescending(p => p.CreatedAt);

            var result = await _unitOfWork.Products.GetPagedAsync(
                pageNumber, pageSize, filter, orderBy);

            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(result.Items);

            return (productDtos, result.TotalCount);
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto createProductDto)
        {
            var product = _mapper.Map<Product>(createProductDto);

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> UpdateAsync(int id, UpdateProductDto updateProductDto)
        {
            var existingProduct = await _unitOfWork.Products.GetByIdAsync(id);
            if (existingProduct == null)
                throw new KeyNotFoundException($"Product with ID {id} not found.");

            // Map the updated properties
            _mapper.Map(updateProductDto, existingProduct);

            _unitOfWork.Products.Update(existingProduct);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ProductDto>(existingProduct);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
                return false;

            _unitOfWork.Products.Remove(product);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _unitOfWork.Products.ExistsAsync(p => p.ProductId == id);
        }

        public async Task<IEnumerable<ProductDto>> GetBySpiceLevelAsync(string spiceLevel)
        {
            var products = await _unitOfWork.Products.FindAsync(p => p.SpiceLevel == spiceLevel);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }
        public async Task<bool> PatchIsAvailableAsync(int productId, bool isAvailable)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null) return false;
            product.IsAvailable = isAvailable;
            product.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Products.Update(product);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }

    // Extension method for combining expressions
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> And<T>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);

            return Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(left, right), parameter);
        }

        private class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression Visit(Expression node)
            {
                if (node == _oldValue)
                    return _newValue;
                return base.Visit(node);
            }
        }
    }

}
