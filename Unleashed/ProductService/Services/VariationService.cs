using AutoMapper;
using ProductService.DTOs.VariationDTOs;
using ProductService.Repositories.IRepositories;
using ProductService.Services.IServices;

namespace ProductService.Services
{
    public class VariationService : IVariationQueryService
    {
        private readonly IVariationRepository _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<VariationService> _logger;

        public VariationService(IVariationRepository repo, IMapper mapper, ILogger<VariationService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<VariationDetailDTO?> GetDetailByIdAsync(int id)
        {
            var v = await _repo.GetByIdAsync(id);
            return v == null ? null : _mapper.Map<VariationDetailDTO>(v);
        }

        public async Task<List<VariationDetailDTO>> GetDetailsByIdsAsync(IEnumerable<int> ids)
        {
            var list = await _repo.GetByIdsAsync(ids ?? Array.Empty<int>());
            return _mapper.Map<List<VariationDetailDTO>>(list);
        }

        public async Task<List<VariationDetailDTO>> SearchDetailsAsync(string? search, Guid? productId, int? colorId, int? sizeId)
        {
            var list = await _repo.SearchAsync(search, productId, colorId, sizeId);
            return _mapper.Map<List<VariationDetailDTO>>(list);
        }
    }
}
