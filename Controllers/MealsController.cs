using LifeApi.Models;
using LifeApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using LifeApi.Utils;

namespace LifeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealsController : ControllerBase
    {
        private readonly MealsRepository _mealRepository;
        private readonly IngredientsRepository _ingredientsRepository;

        public MealsController(MealsRepository mealRepository, IngredientsRepository ingredientsRepository)
        {
            _mealRepository = mealRepository;
            _ingredientsRepository = ingredientsRepository;
        }

        [HttpGet]
        public ActionResult<List<Meal>> Get() =>
            _mealRepository.Get();

        [HttpGet("{id:length(24)}", Name = "GetMeal")]
        public ActionResult<Meal> Get(string id)
        {
            var meal = _mealRepository.Get(id);

            if (meal == null)
            {
                return NotFound();
            }

            return meal;
        }

        [HttpPost]
        public ActionResult<Meal> Create(Meal meal)
        {
            _mealRepository.Create(meal);

            return CreatedAtRoute("GetMeal", new { id = meal.id.ToString() }, meal);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Meal mealIn)
        {
            var meal = _mealRepository.Get(id);

            if (meal == null)
            {
                return NotFound();
            }

            _mealRepository.Update(id, mealIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var meal = _mealRepository.Get(id);

            if (meal == null)
            {
                return NotFound();
            }

            _mealRepository.Remove(meal.id);

            return NoContent();
        }

        [HttpGet("categories")]
        public ActionResult<MealCategory[]> GetCategories()
        {
            return Ok(Enum.GetNames(typeof(MealCategory)));
        }

        [HttpGet("readFile")]
        public ActionResult<List<Meal>> mealsFromFile()
        {
            ParserCsv parser = new ParserCsv();

            List<Meal> meals = parser.parseMealsCsv();

            foreach (Meal meal in meals)
            {
                ICollection<IngredientQuantity> ingredients = meal.ingredients;

                foreach (IngredientQuantity ingredientQuantity in ingredients)
                {
                    Ingredient ingredient = ingredientQuantity.ingredient;

                    Ingredient ingredientFromDb = _ingredientsRepository.GetByName(ingredient.name);

                    if (ingredientFromDb == null)
                    {
                        ingredientFromDb = _ingredientsRepository.Create(ingredient);
                    }
                    
                    ingredientQuantity.ingredient = ingredientFromDb;
                }
            }

            return _mealRepository.CreateMany(meals);
        }
    }
}