using AutoMapper;
using FYPBackEnd.Data.Entities;
using FYPBackEnd.Data.Models.DTO;
using FYPBackEnd.Data.Models.ViewModel;
using System.Collections.Generic;

namespace FYPBackEnd.Core
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ApplicationUser, UserDto>();
            CreateMap<Account, AccountDto>();
            CreateMap<Transaction, TransactionDto>();
            
        }
    }
}
