using MongoDB.Driver;
using ASPNet_Authentication.Models;
using BC = BCrypt.Net.BCrypt;

namespace ASPNet_Authentication.Services
{
    public class UserService
    {
        public static User user = new User();
        private readonly IMongoCollection<User> users;

        public UserService(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("HyphenDb"));

            var database = client.GetDatabase("ASP-NET-Authentication");

            users = database.GetCollection<User>("Users");
        }

        public User GetUser(string email, string password)
        {
            var user = users.Find(u => u.Email == email).FirstOrDefault();
            if (user == null)
            {
                return null;
            }
            if (BC.Verify(password, user.Password))
            {
                return user;
            }
            return null;
        }
        // public User GetUser(string email, string passwword) => users.Find<User>(
        //     user =>
        //         user.Email == email &&
        //         user.Password == passwword)
        //         .FirstOrDefault();

        public User CreateUser(User request)
        {
            string passwordHash = BC.HashPassword(request.Password);


            user.Name = request.Name;
            user.Email = request.Email;
            user.Password = passwordHash;
            user.Phone = request.Phone;


            users.InsertOne(user);
            return user;
        }
    }
}