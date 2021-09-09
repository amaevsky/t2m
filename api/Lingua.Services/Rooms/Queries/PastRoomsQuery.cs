using Lingua.Shared;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lingua.Services.Rooms.Queries
{
    public class PastRoomsQuery : BaseRoomsQuery
    {

    }

    public class PastRoomsQueryHandler : IRequestHandler<PastRoomsQuery, RoomsQueryResponse>
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IDateTimeProvider _dateTime;

        public PastRoomsQueryHandler(IRoomRepository roomRepository, IDateTimeProvider dateTime)
        {
            _roomRepository = roomRepository;
            _dateTime = dateTime;
        }

        public async Task<RoomsQueryResponse> Handle(PastRoomsQuery query, CancellationToken cancellationToken)
        {
            var rooms = await _roomRepository.Get(r =>
                            r.Participants.Any(p => p.Id == query.UserId)
                            && !(
                                (r.EndDate > _dateTime.UtcNow && r.Participants.Count > 1)
                                || r.StartDate > _dateTime.UtcNow
                               )
                            );

            var response = new RoomsQueryResponse();
            response.AddRange(rooms.OrderByDescending(r => r.StartDate));
            return response;
        }
    }
}
