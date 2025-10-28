using AutoMapper;
using InventoryService.Clients.Interfaces;
using InventoryService.Data;
using InventoryService.DTOs.Internal;
using InventoryService.DTOs.Transaction;
using InventoryService.Models;
using InventoryService.Repositories.Interfaces;
using InventoryService.Services.Interfaces;
using InventoryService.Specifications;
using InventoryService.DTOs.External;

namespace InventoryService.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IStockVariationRepository _stockVariationRepository;
        private readonly InventoryDbContext _context;
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

        public async Task<PaginatedResult<TransactionCardDTO>> GetTransactionsFilteredAsync(string? searchTerm, string? dateFilter, string? sort, int page, int pageSize)
        {
            var spec = new TransactionSpecification(searchTerm, dateFilter, sort, page, pageSize);
            var transactions = await _transactionRepository.ListAsync(spec);
            var totalCount = await _transactionRepository.CountAsync(searchTerm, dateFilter);

            if (transactions == null || !transactions.Any())
            {
                return new PaginatedResult<TransactionCardDTO> { Data = new List<TransactionCardDTO>() };
            }

            var variationIds = transactions.Where(t => t.VariationId.HasValue).Select(t => t.VariationId!.Value).Distinct();
            var employeeIds = transactions.Where(t => t.InchargeEmployeeId.HasValue).Select(t => t.InchargeEmployeeId!.Value).Distinct();

            var variationsMap = new Dictionary<int, VariationDto>();
            var employeesMap = new Dictionary<Guid, UserDto>();

            try
            {
                var variationDetails = await _productCatalogClient.GetVariationsByIdsAsync(variationIds);
                if (variationDetails != null)
                {
                    variationsMap = variationDetails.ToDictionary(v => v.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch details from ProductService. The response will contain partial data.");
            }

            try
            {
                var employeeDetails = await _authServiceClient.GetUsersByIdsAsync(employeeIds);
                if (employeeDetails != null)
                {
                    employeesMap = employeeDetails.ToDictionary(e => e.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch details from AuthService. The response will contain partial data.");
            }

            var cardDtos = transactions.Select(t => MapToCardDto(t, variationsMap, employeesMap)).ToList();

            return new PaginatedResult<TransactionCardDTO>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Data = cardDtos
            };
        }

        private TransactionCardDTO MapToCardDto(Transaction t, Dictionary<int, VariationDto> variationsMap, Dictionary<Guid, UserDto> employeesMap)
        {
            variationsMap.TryGetValue(t.VariationId ?? 0, out var variation);
            employeesMap.TryGetValue(t.InchargeEmployeeId ?? Guid.Empty, out var employee);

            return new TransactionCardDTO
            {
                TransactionId = t.TransactionId,
                TransactionDate = t.TransactionDate,
                TransactionQuantity = t.TransactionQuantity,
                TransactionProductPrice = t.TransactionProductPrice,
                TransactionTypeName = t.TransactionType?.TransactionTypeName,
                StockName = t.Stock?.StockName,
                ProviderName = t.Provider?.ProviderName,
                VariationImage = variation?.VariationImage,
                ProductName = variation?.ProductName,
                BrandName = variation?.BrandName,
                CategoryName = variation?.CategoryName,
                SizeName = variation?.SizeName,
                ColorName = variation?.ColorName,
                ColorHexCode = variation?.ColorHexCode,
                InchargeEmployeeUsername = employee?.Username
            };
        }

        public async Task<bool> CreateBulkStockTransactionsAsync(StockTransactionDto importDto)
        {
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

            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var newTransactions = new List<Transaction>();
                var stockVariationsToUpdate = new List<StockVariation>();
                var stockVariationsToCreate = new List<StockVariation>();

                var existingStockVariations = await _stockVariationRepository.GetByIdsAsync(variationIds);
                var stockMap = existingStockVariations.ToDictionary(sv => sv.VariationId, sv => sv);

                foreach (var item in importDto.Variations!)
                {
                    var variationInfo = variationsFromApi.First(v => v.Id == item.VariationId!.Value);

                    newTransactions.Add(new Transaction
                    {
                        StockId = importDto.StockId,
                        ProviderId = importDto.ProviderId,
                        InchargeEmployeeId = user.Id,
                        VariationId = item.VariationId,
                        TransactionQuantity = item.Quantity,
                        TransactionTypeId = 1,
                        TransactionProductPrice = variationInfo.Price,
                        TransactionDate = DateTimeOffset.UtcNow
                    });

                    if (stockMap.TryGetValue(item.VariationId.Value, out var stockVar))
                    {
                        stockVar.StockQuantity = (stockVar.StockQuantity ?? 0) + item.Quantity;
                        stockVariationsToUpdate.Add(stockVar);
                    }
                    else
                    {
                        stockVariationsToCreate.Add(new StockVariation
                        {
                            StockId = importDto.StockId!.Value,
                            VariationId = item.VariationId!.Value,
                            StockQuantity = item.Quantity
                        });
                    }
                }

                if (newTransactions.Any()) await _context.Transactions.AddRangeAsync(newTransactions);
                if (stockVariationsToCreate.Any()) await _context.StockVariations.AddRangeAsync(stockVariationsToCreate);
                if (stockVariationsToUpdate.Any()) _context.StockVariations.UpdateRange(stockVariationsToUpdate);

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

        public async Task<bool> ReserveStockForOrderAsync(List<ProductVariationQuantityDto> items)
        {
            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var variationIds = items.Select(i => i.VariationId!.Value);
                var stockLevels = await _stockVariationRepository.GetByIdsAsync(variationIds);
                var stockMap = stockLevels.ToDictionary(sv => sv.VariationId, sv => sv);

                var transactionsToCreate = new List<Transaction>();

                foreach (var item in items)
                {
                    if (!stockMap.TryGetValue(item.VariationId!.Value, out var stockItem) || (stockItem.StockQuantity ?? 0) < item.Quantity)
                    {
                        throw new InvalidOperationException($"Insufficient stock for Variation ID {item.VariationId}.");
                    }

                    stockItem.StockQuantity -= item.Quantity;

                    transactionsToCreate.Add(new Transaction
                    {
                        VariationId = item.VariationId,
                        TransactionQuantity = item.Quantity,
                        TransactionTypeId = 2,
                        StockId = stockItem.StockId,
                        TransactionDate = DateTimeOffset.UtcNow
                    });
                }

                _context.StockVariations.UpdateRange(stockMap.Values);
                await _context.Transactions.AddRangeAsync(transactionsToCreate);

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reserve stock. Rolling back transaction.");
                await dbTransaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> ReturnStockFromOrderAsync(List<ProductVariationQuantityDto> items, Guid? employeeId)
        {
            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var variationIds = items.Select(i => i.VariationId!.Value);
                var stockLevels = await _stockVariationRepository.GetByIdsAsync(variationIds);
                var stockMap = stockLevels.ToDictionary(sv => sv.VariationId, sv => sv);

                var transactionsToCreate = new List<Transaction>();

                foreach (var item in items)
                {
                    if (stockMap.TryGetValue(item.VariationId!.Value, out var stockItem))
                    {
                        stockItem.StockQuantity += item.Quantity;
                    }
                    else
                    {
                        _logger.LogWarning("Stock record for VariationId {VariationId} not found. Cannot return stock.", item.VariationId);
                        continue;
                    }

                    transactionsToCreate.Add(new Transaction
                    {
                        VariationId = item.VariationId,
                        TransactionQuantity = item.Quantity,
                        TransactionTypeId = 1,
                        StockId = stockItem.StockId,
                        InchargeEmployeeId = employeeId,
                        TransactionDate = DateTimeOffset.UtcNow
                    });
                }

                _context.StockVariations.UpdateRange(stockMap.Values);
                await _context.Transactions.AddRangeAsync(transactionsToCreate);

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to return stock. Rolling back transaction.");
                await dbTransaction.RollbackAsync();
                return false;
            }
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
            return true;
        }

        public async Task<bool> DeleteTransactionAsync(int id)
        {
            var transactionExists = await _transactionRepository.ExistsAsync(id);
            if (!transactionExists) return false;

            await _transactionRepository.DeleteAsync(id);
            return true;
        }
    }
}