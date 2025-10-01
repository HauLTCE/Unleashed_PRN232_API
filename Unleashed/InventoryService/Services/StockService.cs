using AutoMapper;
using InventoryService.DTOs.Stock;
using InventoryService.Models;
using InventoryService.Repositories.Interfaces;
using InventoryService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Services
{
    public class StockService : IStockService
    {
        private readonly IStockRepository _stockRepository;
        private readonly IMapper _mapper;

        public StockService(IStockRepository stockRepository, IMapper mapper)
        {
            _stockRepository = stockRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StockDto>> GetAllStocksAsync()
        {
            var stocks = await _stockRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<StockDto>>(stocks);
        }

        public async Task<StockDto?> GetStockByIdAsync(int id)
        {
            var stock = await _stockRepository.GetByIdAsync(id);
            return _mapper.Map<StockDto>(stock);
        }

        public async Task<StockDto> CreateStockAsync(CreateStockDto stockDto)
        {
            var stockEntity = _mapper.Map<Stock>(stockDto);
            var createdStock = await _stockRepository.AddAsync(stockEntity);
            return _mapper.Map<StockDto>(createdStock);
        }

        public async Task<bool> UpdateStockAsync(int id, UpdateStockDto stockDto)
        {
            var stockToUpdate = await _stockRepository.GetByIdAsync(id);
            if (stockToUpdate == null)
            {
                return false;
            }

            _mapper.Map(stockDto, stockToUpdate);

            try
            {
                await _stockRepository.UpdateAsync(stockToUpdate);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _stockRepository.ExistsAsync(id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
            return true;
        }

        public async Task<bool> DeleteStockAsync(int id)
        {
            var stockExists = await _stockRepository.ExistsAsync(id);
            if (!stockExists)
            {
                return false;
            }

            await _stockRepository.DeleteAsync(id);
            return true;
        }
    }
}