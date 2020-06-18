using LifeApi.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace LifeApi.Repositories
{
    public class MealsRepository
    {
        private readonly IMongoCollection<Meal> _meals;

        public MealsRepository(DatabaseConnection dbConnection)
        {
            _meals = dbConnection.database.GetCollection<Meal>(dbConnection.collectionNames["MealsCollectionName"]);
        }

        public List<Meal> Get() =>
            _meals.Find(meal => true).ToList();

        public Meal Get(string id) =>
            _meals.Find<Meal>(meal => meal.Id == id).FirstOrDefault();

        public Meal Create(Meal meal)
        {
            _meals.InsertOne(meal);
            return meal;
        }

        public void Update(string id, Meal mealIn) =>
            _meals.ReplaceOne(meal => meal.Id == id, mealIn);

        public void Remove(Meal mealIn) =>
            _meals.DeleteOne(meal => meal.Id == mealIn.Id);

        public void Remove(string id) => 
            _meals.DeleteOne(meal => meal.Id == id);
    }
}