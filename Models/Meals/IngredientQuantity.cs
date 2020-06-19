using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LifeApi.Models
{
    public class IngredientQuantity
    {
        public Ingredient ingredient;

        public int quantity;

        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public IngredientUnit? unit;
    }
}