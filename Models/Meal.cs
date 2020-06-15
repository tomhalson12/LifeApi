using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace LifeApi.Models
{
    public class Meal
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Name")]
        [JsonProperty("Name")]
        public string MealName { get; set; }

        public string Category { get; set; }
    }
}