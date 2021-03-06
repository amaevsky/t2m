using AutoMapper;
using Lingua.Shared;
using System.Collections.Generic;
using System.Linq;

namespace Lingua.Data.Mongo
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RoomDto, Room>()
                .ForMember(r => r.Participants, opt => opt.Ignore());
            CreateMap<Room, RoomDto>()
                .ForMember(dto => dto.Participants, opt => opt.MapFrom<UserValueResolver>());
        }
    }

    public class UserValueResolver : IValueResolver<Room, RoomDto, List<ObjectId>>
    {
        public List<ObjectId> Resolve(Room source, RoomDto destination, List<ObjectId> member, ResolutionContext context)
        {
            return source.Participants.Select(p => new ObjectId(p.Id)).ToList();
        }
    }
}
