using Microsoft.AspNetCore.Mvc;
using ProductService.DTOs.Common;
using ProductService.DTOs.ProductDTOs;
using ProductService.DTOs.VariationDTOs;
using ProductService.Services.IServices;

namespace ProductService.Controllers
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

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<PagedResult<ProductDetailDTO>>> GetProducts([FromQuery] PaginationParams paginationParams)
        {
            var result = await _productService.GetPagedProductsAsync(paginationParams);
            return Ok(result);
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDetailDTO>> GetProduct(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> PutProduct(Guid id, [FromBody] UpdateProductDTO updateProductDto)
        {
            if (id != updateProductDto.ProductId)
                return BadRequest("ID in route doesn't match ID in body");

            try
            {
                var updated = await _productService.UpdateProductAsync(id, updateProductDto);
                if (updated == null) return NotFound();

                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return StatusCode(500, "An error occurred while updating the product");
            }
        }

        // POST: api/Products
        [HttpPost]
        public async Task<ActionResult<ProductDetailDTO>> PostProduct(CreateProductDTO createProductDto)
        {
            try
            {
                var createdProduct = await _productService.CreateProductAsync(createProductDto);
                return CreatedAtAction("GetProduct", new { id = createdProduct.ProductId }, createdProduct);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the product");
            }
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        // GET: api/products/for-wishlist/{productId}
        [HttpGet("for-wishlist/{productId:guid}")]
        public async Task<IActionResult> GetProductForWishlist(Guid productId)
        {
            try
            {
                var productInfo = await _productService.GetProductInfoForWishlistAsync(productId);

                if (productInfo == null)
                {
                    return NotFound();
                }

                return Ok(productInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get product info for wishlist. ProductId={ProductId}", productId);
                return StatusCode(500, "Internal server error");
            }
        }


    }
}