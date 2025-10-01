using AutoMapper;
using InventoryService.DTOs.StockVariation;
using InventoryService.Models;
using InventoryService.Repositories.Interfaces;
using InventoryService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Services
{
    public class StockVariationService : IStockVariationService
    {
        private readonly IStockVariationRepository _stockVariationRepository;
        private readonly IMapper _mapper;

        public StockVariationService(IStockVariationRepository stockVariationRepository, IMapper mapper)
        {
            _stockVariationRepository = stockVariationRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StockVariationDto>> GetAllStockVariationsAsync()
        {
            var stockVariations = await _stockVariationRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<StockVariationDto>>(stockVariations);
        }

        public async Task<StockVariationDto?> GetStockVariationByIdAsync(int stockId, int variationId)
        {
            var stockVariation = await _stockVariationRepository.GetByIdAsync(stockId, variationId);
            return _mapper.Map<StockVariationDto>(stockVariation);
        }

        public async Task<StockVariationDto?> CreateStockVariationAsync(CreateStockVariationDto stockVariationDto)
        {
            if (await _stockVariationRepository.ExistsAsync(stockVariationDto.StockId, stockVariationDto.VariationId))
            {
                // Return null to indicate a conflict
                return null;
            }

            var stockVariationEntity = _mapper.Map<StockVariation>(stockVariationDto);
            var newStockVariation = await _stockVariationRepository.AddAsync(stockVariationEntity);
            return _mapper.Map<StockVariationDto>(newStockVariation);
        }

        public async Task<bool> UpdateStockVariationAsync(int stockId, int variationId, UpdateStockVariationDto stockVariationDto)
        {
            var stockVariationToUpdate = await _stockVariationRepository.GetByIdAsync(stockId, variationId);
            if (stockVariationToUpdate == null)
            {
                return false;
            }

            // Map updated properties onto the existing entity
            _mapper.Map(stockVariationDto, stockVariationToUpdate);
            await _stockVariationRepository.UpdateAsync(stockVariationToUpdate);
            return true;
        }

        public async Task<bool> DeleteStockVariationAsync(int stockId, int variationId)
        {
            var stockVariationToDelete = await _stockVariationRepository.GetByIdAsync(stockId, variationId);
            if (stockVariationToDelete == null)
            {
                return false;
            }

            await _stockVariationRepository.DeleteAsync(stockVariationToDelete);
            return true;
        }
    }
}