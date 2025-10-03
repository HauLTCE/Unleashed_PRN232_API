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

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(Guid id, UpdateProductDTO updateProductDto)
        {
            if (id != updateProductDto.ProductId)
            {
                return BadRequest("ID in route doesn't match ID in body");
            }

            var updatedProduct = await _productService.UpdateProductAsync(id, updateProductDto);

            if (updatedProduct == null)
            {
                return NotFound();
            }

            return Ok(updatedProduct);
        }

        // PUT: api/Products/5/variations
        [HttpPut("{id}/variations")]
        public async Task<ActionResult<ProductDetailDTO>> UpdateProductVariations(Guid id, List<UpdateVariationDTO> updateVariationDtos)
        {
            try
            {
                var updatedProduct = await _productService.UpdateProductVariationsAsync(id, updateVariationDtos);

                if (updatedProduct == null)
                {
                    return NotFound();
                }

                return Ok(updatedProduct);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating product variations");
            }
        }

        // DELETE: api/Products/variations/5
        [HttpDelete("variations/{variationId}")]
        public async Task<IActionResult> DeleteVariation(int variationId)
        {
            var result = await _productService.DeleteVariationAsync(variationId);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
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

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}