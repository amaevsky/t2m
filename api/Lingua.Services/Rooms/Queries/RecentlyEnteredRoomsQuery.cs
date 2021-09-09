using Lingua.Shared;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lingua.Services.Rooms.Queries
{
    public class RecentlyEnteredRoomsQuery: BaseRoomsQuery
    {

    }

    public class RecentlyEnteredRoomsQueryHandler : IRequestHandler<RecentlyEnteredRoomsQuery, RoomsQueryResponse>
    {
        private readonly IRoomRepository _roomRepository;

        public RecentlyEnteredRoomsQueryHandler(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public async Task<RoomsQueryResponse> Handle(RecentlyEnteredRoomsQuery query, CancellationToken cancellationToken)
        {
            var all = await _roomRepository.Get();
            var rooms = all.Where(r => r.IsFull)
                        .OrderByDescending(r => r.Updated)
                        .Take(8);

            var response = new RoomsQueryResponse();
            response.AddRange(rooms);
            return response;
        }
    }
}
