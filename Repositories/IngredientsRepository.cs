using LifeApi.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace LifeApi.Repositories {

    public class IngredientsRepository {
        private readonly IMongoCollection<Ingredient> _ingredients;

        public IngredientsRepository(DatabaseConnection dbConnection){
            _ingredients = dbConnection.database.GetCollection<Ingredient>(dbConnection.collectionNames["IngredientsCollectionName"]);
        }

        public List<Ingredient> Get() =>
            _ingredients.Find(ingredient => true).ToList();

        public Ingredient Get(string id) =>
            _ingredients.Find<Ingredient>(ingredient => ingredient.id == id).FirstOrDefault();

        public Ingredient GetByName(string name) =>
            _ingredients.Find<Ingredient>(ingredient => ingredient.name == name).FirstOrDefault();
    
        public Ingredient Create(Ingredient ingredient){
            _ingredients.InsertOne(ingredient);
            return ingredient;
        }

        public void Update(string id, Ingredient ingredientIn) =>
            _ingredients.ReplaceOne(ingredient => ingredient.id == id, ingredientIn);

        public void Remove(Ingredient ingredientIn) =>
            _ingredients.DeleteOne(ingredient => ingredient.id == ingredientIn.id);

        public void Remove(string id) =>
            _ingredients.DeleteOne(ingredient => ingredient.id == id);

    }

}