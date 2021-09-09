using Lingua.Shared;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lingua.Services.Rooms.Queries
{
    public class ConflictingRoomsQuery : BaseRoomsQuery
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

    public class ConflictingRoomsQueryHandler : IRequestHandler<ConflictingRoomsQuery, RoomsQueryResponse>
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IDateTimeProvider _dateTime;

        public ConflictingRoomsQueryHandler(IRoomRepository roomRepository, IDateTimeProvider dateTime)
        {
            _roomRepository = roomRepository;
            _dateTime = dateTime;
        }

        public async Task<RoomsQueryResponse> Handle(ConflictingRoomsQuery query, CancellationToken cancellationToken)
        {
            var rooms = new RoomsQueryResponse();

            rooms.AddRange((await _roomRepository.Get(r =>
                                r.StartDate < query.End
                                && query.Start < r.EndDate
                                && r.Participants.Any(p => p.Id == query.UserId)))
                            .Where(r => r.StartDate > _dateTime.UtcNow || r.IsFull));

            return rooms;
        }
    }
}
