using AutoMapper;
using InventoryService.Clients;
using InventoryService.Clients.Interfaces;
using InventoryService.Data;
using InventoryService.DTOs;
using InventoryService.DTOs.Internal;
using InventoryService.DTOs.Transaction;
using InventoryService.Models;
using InventoryService.Repositories.Interfaces;
using InventoryService.Services.Interfaces;
using InventoryService.Specifications;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IStockVariationRepository _stockVariationRepository;
        private readonly InventoryDbContext _context; //unit of work -> do transaction on bulk so if one failed means fallback on the whole, save a lot of work
        private readonly IMapper _mapper;
        private readonly IProductCatalogClient _productCatalogClient;
        private readonly IAuthServiceClient _authServiceClient;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(
            ITransactionRepository transactionRepository,
            IStockVariationRepository stockVariationRepository,
            InventoryDbContext context,
            IMapper mapper,
            IProductCatalogClient productCatalogClient,
            IAuthServiceClient authServiceClient,
            ILogger<TransactionService> logger)
        {
            _transactionRepository = transactionRepository;
            _stockVariationRepository = stockVariationRepository;
            _context = context;
            _mapper = mapper;
            _productCatalogClient = productCatalogClient;
            _authServiceClient = authServiceClient;
            _logger = logger;
        }

    
        public async Task<bool> CreateBulkStockTransactionsAsync(StockTransactionDto importDto)
        {
            // fetch all external data first to fail fast if something is missing
            var user = await _authServiceClient.GetUserByUsernameAsync(importDto.Username!);
            if (user == null)
            {
                _logger.LogWarning("Bulk import failed: User '{Username}' not found.", importDto.Username);
                return false;
            }

            var variationIds = importDto.Variations!.Select(v => v.VariationId!.Value);
            var variationsFromApi = (await _productCatalogClient.GetVariationsByIdsAsync(variationIds)).ToList();

            if (variationsFromApi.Count != variationIds.Count())
            {
                _logger.LogWarning("Bulk import failed: Not all variation IDs were found.");
                return false;
            }

            // transaction to ensure all database operations are atomic
            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in importDto.Variations!)
                {
                    var variationInfo = variationsFromApi.First(v => v.Id == item.VariationId!.Value);

                    // create audit transaction record
                    var transactionEntity = new Transaction
                    {
                        StockId = importDto.StockId,
                        ProviderId = importDto.ProviderId,
                        InchargeEmployeeId = user.Id,
                        VariationId = item.VariationId,
                        TransactionQuantity = item.Quantity,
                        TransactionTypeId = 1, // 1 = IN
                        TransactionProductPrice = variationInfo.Price,
                        TransactionDate = DateTimeOffset.UtcNow
                    };
                    await _transactionRepository.AddAsync(transactionEntity);

                    // find or create stock variation record
                    var stockVariation = await _stockVariationRepository.GetByIdAsync(importDto.StockId!.Value, item.VariationId!.Value);
                    if (stockVariation != null)
                    {
                        stockVariation.StockQuantity += item.Quantity;
                        await _stockVariationRepository.UpdateAsync(stockVariation);
                    }
                    else
                    {
                        var newStockVariation = new StockVariation
                        {
                            StockId = importDto.StockId!.Value,
                            VariationId = item.VariationId!.Value,
                            StockQuantity = item.Quantity
                        };
                        await _stockVariationRepository.AddAsync(newStockVariation);
                    }
                }

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                _logger.LogInformation("Successfully imported {Count} stock transaction records.", importDto.Variations.Count);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during bulk stock import. Rolling back transaction.");
                await dbTransaction.RollbackAsync();
                return false;
            }
        }
        public async Task<PaginatedResult<TransactionDto>> GetTransactionsFilteredAsync(string? searchTerm, string? dateFilter, string? sort, int page, int pageSize)
        {
            var spec = new TransactionSpecification(searchTerm, dateFilter, sort, page, pageSize);
            var transactions = await _transactionRepository.ListAsync(spec);

            var totalCount = await _transactionRepository.CountAsync(searchTerm, dateFilter);

            var transactionDtos = _mapper.Map<List<TransactionDto>>(transactions);

            return new PaginatedResult<TransactionDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Data = transactionDtos
            };
        }

        public async Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync()
        {
            var transactions = await _transactionRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
        }

        public async Task<TransactionDto?> GetTransactionByIdAsync(int id)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id);
            return _mapper.Map<TransactionDto>(transaction);
        }

        public async Task<TransactionDto?> CreateTransactionAsync(CreateTransactionDto transactionDto)
        {
            var transactionEntity = _mapper.Map<Transaction>(transactionDto);
            var newTransaction = await _transactionRepository.AddAsync(transactionEntity);
            await _context.SaveChangesAsync();
            return _mapper.Map<TransactionDto>(newTransaction);
        }

        public async Task<bool> UpdateTransactionAsync(int id, UpdateTransactionDto transactionDto)
        {
            var transactionEntity = await _transactionRepository.GetByIdAsync(id);
            if (transactionEntity == null) return false;

            _mapper.Map(transactionDto, transactionEntity);
            await _transactionRepository.UpdateAsync(transactionEntity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTransactionAsync(int id)
        {
            var transactionExists = await _transactionRepository.ExistsAsync(id);
            if (!transactionExists) return false;

            await _transactionRepository.DeleteAsync(id);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}