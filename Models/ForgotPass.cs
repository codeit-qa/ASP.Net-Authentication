using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace ASPNet_Authentication.Models
{
    public class ForgptPass
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Code { get; set; }
        public string token { get; set; }

        public string Email { get; set; }        
    }
}