using AutoMapper;
using Lingua.Shared;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Lingua.Data.Mongo
{
    public class RoomRepository : IRoomRepository
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<RoomDto> _rooms;
        private readonly IMapper _mapper;

        public RoomRepository(IOptions<MongoOptions> options, IMapper mapper)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            var database = client.GetDatabase(options.Value.Database);

            _rooms = database.GetCollection<RoomDto>("rooms");
            _users = database.GetCollection<User>("users");
            _mapper = mapper;
        }

        public async Task<Room> Create(Room room)
        {
            var dto = _mapper.Map<RoomDto>(room);
            await _rooms.InsertOneAsync(dto);
            return room;
        }

        public Task<Room> Get(Guid roomId)
        {
            var dto = _rooms.Find(r => r.Id == roomId).FirstOrDefault();
            var usersIds = dto.Participants.Select(p => p.Id).Distinct();
            var users = _users.Find(u => usersIds.Contains(u.Id)).ToList().ToDictionary(u => u.Id);

            var room = _mapper.Map<Room>(dto);
            foreach (var part in dto.Participants)
            {
                room.Participants.Add(users[part.Id]);
            }

            return Task.FromResult(room);
        }

        public Task<IEnumerable<Room>> Get(Expression<Func<RoomDto, bool>> filter = null)
        {
            if (filter == null) filter = room => true;
            Expression<Func<RoomDto, bool>> withoutRemoved = (RoomDto r) => !r.IsRemoved;
            var final = Builders<RoomDto>.Filter.And(filter, withoutRemoved);

            var dtos = _rooms.Find(final).ToEnumerable();
            var rooms = new List<Room>();
            var usersIds = dtos.SelectMany(r => r.Participants.Select(p => p.Id)).Distinct();
            var users = _users.Find(u => usersIds.Contains(u.Id)).ToList().ToDictionary(u => u.Id);

            foreach (var dto in dtos)
            {
                var room = _mapper.Map<Room>(dto);
                foreach (var part in dto.Participants)
                {
                    room.Participants.Add(users[part.Id]);
                }
                rooms.Add(room);
            }

            return Task.FromResult(rooms.AsEnumerable());
        }

        public async Task Remove(Guid roomId)
        {
            await _rooms.UpdateOneAsync(r => r.Id == roomId,
                Builders<RoomDto>.Update.Set(r => r.IsRemoved, true));
        }

        public async Task Update(Room updated)
        {
            var dto = _mapper.Map<RoomDto>(updated);
            await _rooms.ReplaceOneAsync(r => r.Id == dto.Id, dto);
        }
    }
}
