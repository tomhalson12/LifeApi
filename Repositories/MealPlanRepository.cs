using LifeApi.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace LifeApi.Repositories
{
    public class MealPlanRepository
    {
        private readonly IMongoCollection<MealPlan> _mealPlans;

        public MealPlanRepository(DatabaseConnection dbConnection)
        {
            _mealPlans = dbConnection.database.GetCollection<MealPlan>(dbConnection.collectionNames["MealPlansCollectionName"]);
        }

        public List<MealPlan> Get() =>
            _mealPlans.Find(mealPlan => true).ToList();

        public MealPlan Get(string id) =>
            _mealPlans.Find<MealPlan>(mealPlan => mealPlan.id == id).FirstOrDefault();

        public MealPlan Create(MealPlan mealPlan)
        {
            _mealPlans.InsertOne(mealPlan);
            return mealPlan;
        }

        public List<MealPlan> CreateMany(List<MealPlan> meals){
            _mealPlans.InsertMany(meals);
            return meals;
        }

        public void Update(string id, MealPlan mealIn) =>
            _mealPlans.ReplaceOne(mealPlan => mealPlan.id == id, mealIn);

        public void Remove(MealPlan mealIn) =>
            _mealPlans.DeleteOne(mealPlan => mealPlan.id == mealIn.id);

        public void Remove(string id) => 
            _mealPlans.DeleteOne(mealPlan => mealPlan.id == id);

        public MealPlan GetLatest() =>
            _mealPlans.Find<MealPlan>(mealPlan => true).Limit(1).Sort("{sequenceNumber:-1}").FirstOrDefault();
        
    }
}