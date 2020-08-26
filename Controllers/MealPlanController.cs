using LifeApi.Models;
using LifeApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;

namespace LifeApi.Controllers
{
    [Route("api/meals/plans")]
    [ApiController]
    public class MealPlanController : ControllerBase
    {
        private readonly MealPlanRepository _mealPlanRepository;
        private readonly MealsRepository _mealsRepository;

        public MealPlanController(MealPlanRepository mealPlanRepository, MealsRepository mealsRepository)
        {
            _mealPlanRepository = mealPlanRepository;
            _mealsRepository = mealsRepository;
        }

        [HttpGet]
        public ActionResult<List<MealPlan>> Get() =>
            _mealPlanRepository.Get();

        [HttpGet("{id:length(24)}", Name = "GetMealPlan")]
        public ActionResult<MealPlan> Get(string id)
        {
            var mealPlan = _mealPlanRepository.Get(id);

            if (mealPlan == null)
            {
                return NotFound();
            }

            return mealPlan;
        }

        [HttpGet("{date}", Name = "GetMealPlanByDate")]
        public ActionResult<MealPlan> GetByDate(DateTime date)
        {
           var mealPlan = _mealPlanRepository.GetByDate(date);

            if (mealPlan == null)
            {
                return NotFound();
            }

            return mealPlan;
        }

        /*
        [HttpPost]
        public ActionResult<MealPlan> Create(MealPlan mealPlan)
        {
            _mealPlanRepository.Create(mealPlan);

            return CreatedAtRoute("GetMeal", new { id = mealPlan.id.ToString() }, mealPlan);
        }
        */

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, MealPlan mealIn)
        {
            var mealPlan = _mealPlanRepository.Get(id);

            if (mealPlan == null)
            {
                return NotFound();
            }

            _mealPlanRepository.Update(id, mealIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var mealPlan = _mealPlanRepository.Get(id);

            if (mealPlan == null)
            {
                return NotFound();
            }

            _mealPlanRepository.Remove(mealPlan.id);

            return NoContent();
        }

        [HttpPost]
        public ActionResult<MealPlan> Create(int numMeals)
        {
            Random ran = new Random();

            MealPlan newMealPlan = new MealPlan();

            MealPlan latestMealPlan = _mealPlanRepository.GetLatest();
            bool latestMealPlanAvailable = latestMealPlan != null;
            List<string> previousMealIds = !latestMealPlanAvailable ? new List<string>() : latestMealPlan.mealIds.ToList();

            List<Meal> allMeals = _mealsRepository.Get();

            List<string> mealIds = new List<string>();

            for (int i = 0; i < numMeals; i++)
            {
                bool mealCheck = false;
                do {
                    int mealIndex = ran.Next(allMeals.Count);

                    Meal meal = allMeals[mealIndex];

                    if(!mealIds.Contains(meal.id) && !previousMealIds.Contains(meal.id)){
                        mealIds.Add(meal.id);
                        mealCheck = true;
                    }

                } while (!mealCheck);
            }

            newMealPlan.mealIds = mealIds;
            newMealPlan.previousPlanId = !latestMealPlanAvailable ? null : latestMealPlan.id;
            newMealPlan.sequenceNumber = !latestMealPlanAvailable ? 1 : latestMealPlan.sequenceNumber + 1;

            DateTime today = DateTime.UtcNow.Date;
            int daysUntilMonday = ((int) DayOfWeek.Monday - (int) today.DayOfWeek + 7) % 7;
            DateTime nextMonday = today.AddDays(daysUntilMonday);

            newMealPlan.startDate = nextMonday;

            _mealPlanRepository.Create(newMealPlan);

            return CreatedAtRoute("GetMeal", new { id = newMealPlan.id.ToString() }, newMealPlan);
        }

        [HttpGet("latest")]
        public ActionResult<MealPlan> GetLatest()
        {
            var mealPlan = _mealPlanRepository.GetLatest();

            if (mealPlan == null)
            {
                return NotFound();
            }

            return mealPlan;
        }

        [HttpGet("currentWeek/meals")]
        public ActionResult<List<Meal>> GetCurrentWeekMeals()
        {
            DateTime previousMonday = DateTime.UtcNow.Date.AddDays(DayOfWeek.Monday - DateTime.UtcNow.DayOfWeek).ToLocalTime();

            MealPlan currentMealPlan = _mealPlanRepository.GetByDate(previousMonday);

            if(currentMealPlan == null){
                return NotFound();
            }

            List<Meal> meals = new List<Meal>();

            foreach(string mealId in currentMealPlan.mealIds){
                meals.Add(_mealsRepository.Get(mealId));
            }

            return meals;
        }

        [HttpGet("nextWeek")]
        public ActionResult<MealPlan> GetNextWeekMealPlan()
        {
            DateTime nextMonday = DateTime.UtcNow.Date.AddDays(DayOfWeek.Monday - DateTime.UtcNow.DayOfWeek + 7).ToLocalTime();

            MealPlan mealPlan = _mealPlanRepository.GetByDate(nextMonday);

            if(mealPlan == null){
                return NotFound();
            }

            return mealPlan;
        }

        [HttpGet("nextWeek/meals")]
        public ActionResult<List<Meal>> GetNextWeekMeals()
        {
            DateTime nextMonday = DateTime.UtcNow.Date.AddDays(DayOfWeek.Monday - DateTime.UtcNow.DayOfWeek + 7).ToLocalTime();

            MealPlan mealPlan = _mealPlanRepository.GetByDate(nextMonday);

            if(mealPlan == null){
                return NotFound();
            }

            List<Meal> meals = new List<Meal>();

            foreach(string mealId in mealPlan.mealIds){
                meals.Add(_mealsRepository.Get(mealId));
            }

            return meals;
        }

        [HttpGet("nextWeek/meals/ingredients")]
        public ActionResult<List<IngredientQuantity>> GetNextWeekIngredients()
        {
            DateTime nextMonday = DateTime.UtcNow.Date.AddDays(DayOfWeek.Monday - DateTime.UtcNow.DayOfWeek + 7).ToLocalTime();

            MealPlan mealPlan = _mealPlanRepository.GetByDate(nextMonday);
            Console.WriteLine("here");
            if(mealPlan == null){
                return NotFound();
            }

            List<Meal> meals = new List<Meal>();

            foreach(string mealId in mealPlan.mealIds){
                meals.Add(_mealsRepository.Get(mealId));
            }

            if(meals.Count == 0){
                return NotFound();
            }

            List<IngredientQuantity> ingredientQuantities = new List<IngredientQuantity>();
            meals.ForEach(meal => meal.ingredients.ToList().ForEach(ingredientQuantity => ingredientQuantities.Add(ingredientQuantity)));

            List<IngredientQuantity> finalIngredients = new List<IngredientQuantity>();

            foreach(IngredientQuantity iQ in ingredientQuantities){
                if(finalIngredients.Count == 0){
                    finalIngredients.Add(iQ);
                } else {
                    bool alreadyHaveIngredient = false;
                    foreach(IngredientQuantity fIq in finalIngredients){
                        if(iQ.ingredient.id == fIq.ingredient.id && iQ.unit == fIq.unit){
                            fIq.quantity += iQ.quantity;
                            alreadyHaveIngredient = true;
                            break;
                        }
                    }
                    if(!alreadyHaveIngredient){
                        finalIngredients.Add(iQ);
                    }
                }
            }

            return finalIngredients;
        }
    }
}