using AutoMapper;
using InventoryService.DTOs.Transaction;
using InventoryService.Models;

namespace InventoryService.Profiles
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {
            CreateMap<Transaction, TransactionDto>();
            CreateMap<CreateTransactionDto, Transaction>();
            CreateMap<UpdateTransactionDto, Transaction>();
        }
    }
}