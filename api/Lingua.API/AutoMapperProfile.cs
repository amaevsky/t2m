using AutoMapper;
using Lingua.API.ViewModels;
using Lingua.Shared;

namespace Lingua.API
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Room, RoomViewModel>();
            CreateMap<User, RoomUserViewModel>();
        }
    }
}
