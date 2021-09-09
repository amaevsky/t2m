using Lingua.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lingua.Services.Rooms.Queries
{
    public class BaseRoomsQuery : IRequest<RoomsQueryResponse>
    {
        public Guid UserId { get; set; }
    }

    public class RoomsQueryResponse : List<Room>
    {

    }
}
