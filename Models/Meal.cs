using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System.Collections.Generic;
using LifeApi.Models;

namespace LifeApi.Models
{
    public class Meal
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }

        public string name { get; set; }

        public string category { get; set; }

        public int time { get; set; }

        public ICollection<IngredientQuantity> ingredients { get; set; }
    }
}