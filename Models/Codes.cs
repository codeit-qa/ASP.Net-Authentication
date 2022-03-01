using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace ASPNet_Authentication.Models
{
    public class Codes
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }
    }
}