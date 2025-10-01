using AutoMapper;
using InventoryService.DTOs.Provider;
using InventoryService.Models;
using InventoryService.Repositories.Interfaces;
using InventoryService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Services
{
    public class ProviderService : IProviderService
    {
        private readonly IProviderRepository _providerRepository;
        private readonly IMapper _mapper;

        public ProviderService(IProviderRepository providerRepository, IMapper mapper)
        {
            _providerRepository = providerRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProviderDto>> GetAllProvidersAsync()
        {
            var providers = await _providerRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProviderDto>>(providers);
        }

        public async Task<ProviderDto?> GetProviderByIdAsync(int id)
        {
            var provider = await _providerRepository.GetByIdAsync(id);
            return _mapper.Map<ProviderDto>(provider);
        }

        public async Task<ProviderDto> CreateProviderAsync(CreateProviderDto providerDto)
        {
            var providerEntity = _mapper.Map<Provider>(providerDto);

            providerEntity.ProviderCreatedAt = DateTimeOffset.UtcNow;
            providerEntity.ProviderUpdatedAt = DateTimeOffset.UtcNow;

            var createdProvider = await _providerRepository.AddAsync(providerEntity);
            return _mapper.Map<ProviderDto>(createdProvider);
        }

        public async Task<bool> UpdateProviderAsync(int id, UpdateProviderDto providerDto)
        {
            var providerToUpdate = await _providerRepository.GetByIdAsync(id);
            if (providerToUpdate == null)
            {
                return false;
            }

            _mapper.Map(providerDto, providerToUpdate);
            providerToUpdate.ProviderUpdatedAt = DateTimeOffset.UtcNow;

            try
            {
                await _providerRepository.UpdateAsync(providerToUpdate);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _providerRepository.ExistsAsync(id))
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

        public async Task<bool> DeleteProviderAsync(int id)
        {
            var providerExists = await _providerRepository.ExistsAsync(id);
            if (!providerExists)
            {
                return false;
            }

            await _providerRepository.DeleteAsync(id);
            return true;
        }
    }
}