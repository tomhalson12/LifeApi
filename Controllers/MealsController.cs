using LifeApi.Models;
using LifeApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace MealsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealsController : ControllerBase
    {
        private readonly MealService _mealService;

        public MealsController(MealService mealService)
        {
            _mealService = mealService;
        }

        [HttpGet]
        public ActionResult<List<Meal>> Get() =>
            _mealService.Get();

        [HttpGet("{id:length(24)}", Name = "GetMeal")]
        public ActionResult<Meal> Get(string id)
        {
            var meal = _mealService.Get(id);

            if (meal == null)
            {
                return NotFound();
            }

            return meal;
        }

        [HttpPost]
        public ActionResult<Meal> Create(Meal meal)
        {
            _mealService.Create(meal);

            return CreatedAtRoute("GetMeal", new { id = meal.Id.ToString() }, meal);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Meal mealIn)
        {
            var meal = _mealService.Get(id);

            if (meal == null)
            {
                return NotFound();
            }

            _mealService.Update(id, mealIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var meal = _mealService.Get(id);

            if (meal == null)
            {
                return NotFound();
            }

            _mealService.Remove(meal.Id);

            return NoContent();
        }
    }
}