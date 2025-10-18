using AutoMapper;
using ProductService.DTOs.OtherDTOs;
using ProductService.Repositories.IRepositories;
using ProductService.Services.IServices;

namespace ProductService.Services
{
    public class FilterService : IFilterService
    {
        private readonly IColorRepository _colorRepo;
        private readonly ISizeRepository _sizeRepo;
        private readonly IMapper _mapper;

        public FilterService(IColorRepository colorRepo, ISizeRepository sizeRepo, IMapper mapper)
        {
            _colorRepo = colorRepo;
            _sizeRepo = sizeRepo;
            _mapper = mapper;
        }

        public async Task<FilterOptionsDTO> GetFilterOptionsAsync(bool onlyAvailable = false, bool onlyActiveProducts = false)
        {
            if (!onlyAvailable)
            {
                var colors = await _colorRepo.GetAllAsync();
                var sizes = await _sizeRepo.GetAllAsync();

                return new FilterOptionsDTO
                {
                    Colors = _mapper.Map<List<ColorDTO>>(colors),
                    Sizes = _mapper.Map<List<SizeDTO>>(sizes)
                };
            }
            else
            {
                var colors = await _colorRepo.GetAvailableAsync(onlyActiveProducts);
                var sizes = await _sizeRepo.GetAvailableAsync(onlyActiveProducts);

                return new FilterOptionsDTO
                {
                    Colors = _mapper.Map<List<ColorDTO>>(colors),
                    Sizes = _mapper.Map<List<SizeDTO>>(sizes)
                };
            }
        }
    }
}

