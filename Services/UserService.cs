using MongoDB.Driver;
using ASPNet_Authentication.Models;
using BC = BCrypt.Net.BCrypt;

namespace ASPNet_Authentication.Services
{
    public class UserService
    {
        public static User user = new User();
        public static Codes code = new Codes();
        public static ForgptPass forgptPass = new ForgptPass();
        private readonly IMongoCollection<User> users;
        private readonly IMongoCollection<Codes> codes;
        private readonly IMongoCollection<ForgptPass> fgPass;

        public UserService(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("HyphenDb"));

            var database = client.GetDatabase("ASP-NET-Authentication");

            users = database.GetCollection<User>("Users");

            codes = database.GetCollection<Codes>("Codes");

            fgPass = database.GetCollection<ForgptPass>("ForgotPass");
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

        public bool findUser(string email)
        {
            var user = users.Find(u => u.Email == email).FirstOrDefault();

            if (user == null)
            {
                return false;
            }
            return true;
        }

        public void forgotPassword(string code, string token, string email)
        {
            forgptPass.Code = code;
            forgptPass.token = token;
            forgptPass.Email = email;
            fgPass.InsertOne(forgptPass);
        }

        public ForgptPass verifyResetCode(string token, string code)
        {
            var fgPassData = fgPass.Find(u => u.token == token).FirstOrDefault();

            if (fgPassData == null)
            {
                return null;
            }
            if (fgPassData.Code == code)
            {
                return fgPassData;
            }
            return null;
        }

        public bool resetPassword(string token, string pass)
        {
            forgptPass.token = token;

            try
            {

                var fgPassData = fgPass.Find(u => u.token == forgptPass.token).FirstOrDefault();

                if (fgPassData != null)
                {
                    try
                    {

                        var user = users.Find(u => u.Email == fgPassData.Email).FirstOrDefault();

                        //update password
                        user.Password = BC.HashPassword(pass);

                        users.ReplaceOne(u => u.Email == user.Email, user);

                        return true;


                    }
                    catch
                    {
                        return false;
                    }

                }
            }
            catch
            {
                return false;
            }

            return false;
        }

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

        public void codeAdd(string otp, string email)
        {

            codes.InsertOne(new Codes
            {
                Code = otp,
                Email = email

            });
        }

        public Codes GetCode(string otp, string email)
        {
            var code = codes.Find(c => c.Code == otp && c.Email == email).FirstOrDefault();
            if (code == null)
            {
                return null;
            }
            return code;
        }

    }
}