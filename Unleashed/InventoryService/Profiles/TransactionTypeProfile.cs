using AutoMapper;
using InventoryService.DTOs.TransactionType;
using InventoryService.Models;

namespace InventoryService.Profiles
{
    public class TransactionTypeProfile : Profile
    {
        public TransactionTypeProfile()
        {
            CreateMap<TransactionType, TransactionTypeDto>();
            CreateMap<CreateTransactionTypeDto, TransactionType>();
            CreateMap<UpdateTransactionTypeDto, TransactionType>();
        }
    }
}