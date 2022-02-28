using MongoDB.Driver;
using ASPNet_Authentication.Models;

namespace ASPNet_Authentication.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> users;

        public UserService(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("HyphenDb"));

            var database = client.GetDatabase("ASP-NET-Authentication");

            users = database.GetCollection<User>("Users");
        }

        public List<User> GetUsers() => users.Find(user => true).ToList();

        public User GetUser(string id) => users.Find<User>(user => user.Id == id).FirstOrDefault();

        public User CreateUser(User user)
        {
            users.InsertOne(user);
            return user;
        }
    }
}