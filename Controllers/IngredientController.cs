using LifeApi.Models;
using LifeApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;

namespace LifeApi.Controllers
{
    [Route("api/meals/[controller]")]
    [ApiController]
    public class IngredientsController : ControllerBase
    {
        private readonly IngredientsRepository  _ingredientsRepository;

        public IngredientsController(IngredientsRepository ingredientsRepository)
        {
            _ingredientsRepository = ingredientsRepository;
        }

        [HttpGet]
        public ActionResult<List<Ingredient>> Get() =>
            _ingredientsRepository.Get();

        [HttpGet("{id:length(24)}", Name = "GetIngredient")]
        public ActionResult<Ingredient> Get(string id)
        {
            var ingredient = _ingredientsRepository.Get(id);

            if (ingredient == null)
            {
                return NotFound();
            }

            return ingredient;
        }

        [HttpPost]
        public ActionResult<Ingredient> Create(Ingredient ingredient)
        {
            _ingredientsRepository.Create(ingredient);

            return CreatedAtRoute("GetIngredient", new { id = ingredient.id.ToString() }, ingredient);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Ingredient ingredientIn)
        {
            var ingredient = _ingredientsRepository.Get(id);

            if (ingredient == null)
            {
                return NotFound();
            }

            _ingredientsRepository.Update(id, ingredientIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var ingredient = _ingredientsRepository.Get(id);

            if (ingredient == null)
            {
                return NotFound();
            }

            _ingredientsRepository.Remove(ingredient.id);

            return NoContent();
        }

        [HttpGet("units")]
        public ActionResult<IngredientUnit[]> GetIngredientUnits(){
            return Ok(Enum.GetNames(typeof(IngredientUnit)));
        }
    }
}