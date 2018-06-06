using AutoMapper;

using Service_Repository_Layer.Entity;
using Service_Repository_Layer.Common.DataTransferObjects;
using System.Linq;

namespace Service_Repository_Layer.Service.Configuration
{
    public static class MapperInitialize
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg =>
            {
                
                cfg.CreateMap<User, UserDto>();
                cfg.CreateMap<UserDto, User>();
                
            });
        }
    }
}
