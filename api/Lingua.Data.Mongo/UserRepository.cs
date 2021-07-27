using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Lingua.Data.Mongo
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(IOptions<MongoOptions> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            var database = client.GetDatabase(options.Value.Database);

            _users = database.GetCollection<User>("users");
        }

        public async Task<User> Create(User user)
        {
            await _users.InsertOneAsync(user);
            return user;
        }

        public Task<User> Get(Guid userId)
        {
            Expression<Func<User, bool>> filter = r => r.Id == userId;
            return Task.FromResult(_users.Find(filter).FirstOrDefault());
        }

        public Task<IEnumerable<User>> Get(Expression<Func<User, bool>> filter)
        {
            return Task.FromResult(_users.Find(filter).ToEnumerable());
        }

        public async Task Remove(Guid userId)
        {
            await _users.DeleteOneAsync(r => r.Id == userId);
        }

        public async Task Update(User updated)
        {
            await _users.ReplaceOneAsync(r => r.Id == updated.Id, updated);
        }
    }
}
