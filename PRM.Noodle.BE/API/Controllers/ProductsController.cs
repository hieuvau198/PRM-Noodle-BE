using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.DTOs.Product;
using Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Get all products
        /// </summary>
        /// <returns>List of all products</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAllAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving products.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get products with pagination and filtering
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 100)</param>
        /// <param name="searchTerm">Search term for product name or description</param>
        /// <param name="spiceLevel">Filter by spice level</param>
        /// <param name="isAvailable">Filter by availability</param>
        /// <returns>Paginated list of products</returns>
        [HttpGet("paged")]
        public async Task<ActionResult<object>> GetPagedProducts(
            [FromQuery] int pageNumber = 1,
            [FromQuery, Range(1, 100)] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? spiceLevel = null,
            [FromQuery] bool? isAvailable = null)
        {
            try
            {
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 100) pageSize = 100;

                var result = await _productService.GetPagedAsync(pageNumber, pageSize, searchTerm, spiceLevel, isAvailable);

                var response = new
                {
                    Items = result.Items,
                    TotalCount = result.TotalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize),
                    HasNextPage = pageNumber * pageSize < result.TotalCount,
                    HasPreviousPage = pageNumber > 1
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving products.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get available products only
        /// </summary>
        /// <returns>List of available products</returns>
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAvailableProducts()
        {
            try
            {
                var products = await _productService.GetAvailableAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving available products.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get products by spice level
        /// </summary>
        /// <param name="spiceLevel">Spice level to filter by</param>
        /// <returns>List of products with specified spice level</returns>
        [HttpGet("spice-level/{spiceLevel}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsBySpiceLevel(string spiceLevel)
        {
            try
            {
                var products = await _productService.GetBySpiceLevelAsync(spiceLevel);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving products by spice level.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get product by ID
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);
                return Ok(product);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Product with ID {id} not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the product.", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new product
        /// </summary>
        /// <param name="createProductDto">Product creation data</param>
        /// <returns>Created product</returns>
        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var product = await _productService.CreateAsync(createProductDto);
                return CreatedAtAction(nameof(GetProductById), new { id = product.ProductId }, product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the product.", error = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="updateProductDto">Product update data</param>
        /// <returns>Updated product</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromBody] UpdateProductDto updateProductDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var product = await _productService.UpdateAsync(id, updateProductDto);
                return Ok(product);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Product with ID {id} not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the product.", error = ex.Message });
            }
        }

        [HttpPatch("{id}/is-available")]
        public async Task<IActionResult> PatchIsAvailable(int id, [FromBody] ProductIsAvailableDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _productService.PatchIsAvailableAsync(id, dto.IsAvailable);
            if (!result)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            try
            {
                var deleted = await _productService.DeleteAsync(id);
                if (!deleted)
                    return NotFound(new { message = $"Product with ID {id} not found." });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the product.", error = ex.Message });
            }
        }

        /// <summary>
        /// Check if product exists
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Boolean indicating if product exists</returns>
        [HttpGet("{id}/exists")]
        public async Task<ActionResult<bool>> ProductExists(int id)
        {
            try
            {
                var exists = await _productService.ExistsAsync(id);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while checking product existence.", error = ex.Message });
            }
        }
    }
}
