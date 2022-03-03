using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace ASPNet_Authentication.Models
{
    public class NewPassword
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Password { get; set; }
        public string Confirm { get; set; }
        
    }
}