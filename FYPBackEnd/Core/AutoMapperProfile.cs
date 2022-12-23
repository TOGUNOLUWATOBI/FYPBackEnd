using AutoMapper;
using FYPBackEnd.Data.Entities;
using FYPBackEnd.Data.Models.ViewModel;

namespace FYPBackEnd.Core
{
    public class AutoMapperProfile :Profile
    {
        public AutoMapperProfile() 
        {
            CreateMap<ApplicationUser, UserDto>();
        }
    }
}
