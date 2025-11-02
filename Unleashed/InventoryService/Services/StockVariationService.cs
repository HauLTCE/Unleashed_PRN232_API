using AutoMapper;
using InventoryService.DTOs.External;
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

        public async Task<Inventory_OrderDto?> GetStockByVariationIdAsync(int variationId)
        {
           var stockList = await _stockVariationRepository.GetByVariationIdAsync(variationId);
            if (stockList == null)
            {              
                return new Inventory_OrderDto
                {
                    VariationId = variationId,
                    TotalQuantity = 0
                };
            }
            var totalQuantity = stockList.Sum(ov => ov.StockQuantity);
            Inventory_OrderDto inventory_OrderDto = new()
            {
                VariationId = variationId,
                TotalQuantity = totalQuantity
            };
            return inventory_OrderDto;
        }

        public async Task<IEnumerable<Inventory_OrderDto?>> GetStockByVariationIdsAsync(List<int> variationIds)
        {
            if (variationIds == null || variationIds.Count == 0)
                return [];

            var result = new List<Inventory_OrderDto?>();

            foreach (var id in variationIds)
            {
                try
                {
                    var stock = await GetStockByVariationIdAsync(id);
                    result.Add(stock);
                }
                catch (Exception)
                {
                    result.Add(null); // keep index alignment, same behavior as before
                }
            }

            return result;
        }


        public async Task<IEnumerable<StockVariationDto>> GetStockVariationsByStockIdAsync(int stockId)
        {
            var stockVariations = await _stockVariationRepository.GetByStockIdAsync(stockId);
            return _mapper.Map<IEnumerable<StockVariationDto>>(stockVariations);
        }

        public async Task DecreaseStocksAsync(List<Order_InventoryDto> orderList)
        {
            foreach (var item in orderList)
            {
                var remainingQty = item.Quantity;

                var stockVariations = await _stockVariationRepository
                    .GetByVariationIdAsync(item.VariationId);

                foreach (var stock in stockVariations)
                {
                    if (remainingQty <= 0) break;

                    if (remainingQty >= stock.StockQuantity)
                    {
                        remainingQty -= stock.StockQuantity;
                        stock.StockQuantity = 0;
                    }
                    else
                    {
                        stock.StockQuantity -= remainingQty;
                        remainingQty = 0;
                        break;
                    }
                }

                if (remainingQty > 0)
                {
                    throw new InvalidOperationException(
                        $"Not enough stock for variation {item.VariationId}. Missing: {remainingQty}");
                }

                await _stockVariationRepository.UpdateRangeAsync(stockVariations);
            }
        }

    }
}