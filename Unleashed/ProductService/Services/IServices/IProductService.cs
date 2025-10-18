using ProductService.DTOs.Common;
using ProductService.DTOs.ProductDTOs;
using ProductService.DTOs.VariationDTOs;

namespace ProductService.Services.IServices
{
    public interface IProductService
    {
        Task<PagedResult<ProductDetailDTO>> GetPagedProductsAsync(PaginationParams pagination);
        Task<ProductDetailDTO?> GetProductByIdAsync(Guid id);
        Task<ProductDetailDTO> CreateProductAsync(CreateProductDTO createProductDto);
        Task<ProductDetailDTO?> UpdateProductAsync(Guid id, UpdateProductDTO updateProductDto);
        Task<bool> DeleteProductAsync(Guid id);
        Task<ProductDetailDTO?> UpdateProductVariationsAsync(Guid productId, List<UpdateVariationDTO> updateVariationDtos);
        Task<bool> DeleteVariationAsync(int variationId);
        Task<ProductForWishlistDTO?> GetProductInfoForWishlistAsync(Guid productId);
    }
}
