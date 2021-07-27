using AutoMapper;

namespace Lingua.API
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Shared.Room, Data.Room>();
            CreateMap<Data.Room, Shared.Room>();

            CreateMap<Shared.User, Data.User>();
            CreateMap<Data.User, Shared.User>();

            CreateMap<Shared.ZoomProperties, Data.ZoomProperties>();
            CreateMap<Data.ZoomProperties, Shared.ZoomProperties>();
        }
    }
}
