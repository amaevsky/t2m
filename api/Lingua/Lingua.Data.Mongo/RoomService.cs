using Lingua.Shared;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Lingua.Data.Mongo
{
    public class RoomService : IRoomService
    {
        private readonly IMongoCollection<Room> _rooms;

        public RoomService(IOptions<MongoOptions> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            var database = client.GetDatabase(options.Value.Database);

            _rooms = database.GetCollection<Room>("rooms");
        }

        public async Task AddParticipant(Guid roomId, User participant)
        {
            var filter = Builders<Room>.Update.AddToSet(r => r.Participants, participant);
            await _rooms.UpdateOneAsync(r => r.Id == roomId, filter);
        }

        public async Task<Room> Create(CreateRoomOptions options)
        {
            var room = new Room
            {

            };
            await _rooms.InsertOneAsync(room);
            return room;
        }

        public Task<Room> Get(Guid roomId)
        {
            Expression<Func<Room, bool>> a = r => r.Id == roomId;
            Expression<Func<Room, bool>> b = r => r.Id == roomId;


            var final = Builders<Room>.Filter.And(a, b);
            return Task.FromResult(_rooms.Find(final).FirstOrDefault());
        }

        public Task<IEnumerable<Room>> Get(SearchRoomOptions options)
        {
            return Task.FromResult(_rooms.Find(r => true).ToEnumerable());
        }

        public async Task Remove(Guid roomId)
        {
            await _rooms.DeleteOneAsync(r => r.Id == roomId);
        }

        public async Task Update(UpdateRoomOptions options)
        {
            var update = Builders<Room>.Update.Set(r => r.StartDate, options.Date);
            await _rooms.UpdateOneAsync(r => r.Id == options.RoomId, update);
        }
    }
}
