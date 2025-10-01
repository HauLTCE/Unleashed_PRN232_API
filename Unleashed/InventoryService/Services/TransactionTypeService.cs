using AutoMapper;
using InventoryService.DTOs.TransactionType;
using InventoryService.Models;
using InventoryService.Repositories.Interfaces;
using InventoryService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Services
{
    public class TransactionTypeService : ITransactionTypeService
    {
        private readonly ITransactionTypeRepository _repository;
        private readonly IMapper _mapper;

        public TransactionTypeService(ITransactionTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TransactionTypeDto>> GetAllTransactionTypesAsync()
        {
            var transactionTypes = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<TransactionTypeDto>>(transactionTypes);
        }

        public async Task<TransactionTypeDto?> GetTransactionTypeByIdAsync(int id)
        {
            var transactionType = await _repository.GetByIdAsync(id);
            return _mapper.Map<TransactionTypeDto>(transactionType);
        }

        public async Task<TransactionTypeDto> CreateTransactionTypeAsync(CreateTransactionTypeDto transactionTypeDto)
        {
            var transactionTypeEntity = _mapper.Map<TransactionType>(transactionTypeDto);
            var newTransactionType = await _repository.AddAsync(transactionTypeEntity);
            return _mapper.Map<TransactionTypeDto>(newTransactionType);
        }

        public async Task<bool> UpdateTransactionTypeAsync(int id, UpdateTransactionTypeDto transactionTypeDto)
        {
            var transactionTypeToUpdate = await _repository.GetByIdAsync(id);
            if (transactionTypeToUpdate == null)
            {
                return false;
            }

            _mapper.Map(transactionTypeDto, transactionTypeToUpdate);

            try
            {
                await _repository.UpdateAsync(transactionTypeToUpdate);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _repository.ExistsAsync(id))
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

        public async Task<bool> DeleteTransactionTypeAsync(int id)
        {
            if (!await _repository.ExistsAsync(id))
            {
                return false;
            }

            await _repository.DeleteAsync(id);
            return true;
        }
    }
}