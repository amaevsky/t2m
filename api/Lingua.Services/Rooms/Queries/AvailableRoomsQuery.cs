using Lingua.Shared;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lingua.Services.Rooms.Queries
{
    public class AvailableRoomsQuery: BaseRoomsQuery
    {
        public SearchRoomOptions Options { get; set; }
    }

    public class AvailableRoomsQueryHandler : IRequestHandler<AvailableRoomsQuery, RoomsQueryResponse>
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDateTimeProvider _dateTime;

        public AvailableRoomsQueryHandler(IRoomRepository roomRepository, IUserRepository userRepository, IDateTimeProvider dateTime)
        {
            _roomRepository = roomRepository;
            _userRepository = userRepository;
            _dateTime = dateTime;
        }

        public async Task<RoomsQueryResponse> Handle(AvailableRoomsQuery query, CancellationToken cancellationToken)
        {
            var user = await _userRepository.Get(query.UserId);
            var rooms = (await _roomRepository.Get(r =>
                                    r.StartDate > _dateTime.UtcNow
                                    && r.Language == user.TargetLanguage
                                    && !r.Participants.Any(p => p.Id == query.UserId)))
                .Where(r => !r.IsFull);

            if (query.Options != null)
            {
                rooms = ApplySearchFilter(query.Options, rooms, user.Timezone);
            }

            var response = new RoomsQueryResponse();
            response.AddRange(rooms);
            return response;
        }

        private static IEnumerable<Room> ApplySearchFilter(SearchRoomOptions options, IEnumerable<Room> rooms, string timezone)
        {
            if (options?.Levels?.Any() == true)
            {
                rooms = rooms.Where(r => options.Levels.Contains(r.Host.LanguageLevel));
            }

            if (options?.Days?.Any() == true)
            {
                rooms = rooms.Where(r => options.Days.Contains(Utilities.ConvertToTimezone(r.StartDate, timezone).DayOfWeek));
            }

            if ((bool)options?.TimeFrom.HasValue && (bool)options?.TimeTo.HasValue)
            {
                //store room.till date
                rooms = rooms.Where(r => r.StartDate.TimeOfDay > options.TimeFrom.Value.TimeOfDay
                                        && r.StartDate.TimeOfDay < options.TimeTo.Value.TimeOfDay);
            }

            return rooms;
        }
    }
}
