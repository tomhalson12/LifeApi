using LifeApi.Models;
using CsvHelper;
using System.IO;
using System.Collections.Generic;
using System;

namespace LifeApi.Utils
{
    public class ParserCsv
    {
        public List<Meal> parseMealsCsv()
        {

            string fileName = "Meals.csv";
            TextReader reader = new StreamReader(fileName);
            CsvReader csvReader = new CsvReader(reader, System.Globalization.CultureInfo.CurrentCulture);
            IEnumerable<MealCsv> csvMealsList = csvReader.GetRecords<MealCsv>();

            List<Meal> meals = new List<Meal>();

            foreach (MealCsv csvMeal in csvMealsList)
            {
                Meal meal = new Meal
                {
                    name = csvMeal.name,
                    category = csvMeal.category,
                    time = csvMeal.time,
                    ingredients = parseIngredients(csvMeal.ingredients)
                };

                meals.Add(meal);
            }

            return meals;
        }

        public List<IngredientQuantity> parseIngredients(string csvIngredients)
        {
            string[] ingredients = csvIngredients.Split(',');

            List<IngredientQuantity> ingredientQuantities = new List<IngredientQuantity>();

            foreach (string ingred in ingredients)
            {
                string[] parts = ingred.Split('-');
                Ingredient ingredient = new Ingredient{
                    name = parts[0]
                };

                Enum.TryParse(parts[2], out IngredientUnit unit);
                IngredientQuantity ingredientQuantity = new IngredientQuantity{
                    ingredient = ingredient,
                    quantity = int.Parse(parts[1]),
                    unit = unit
                };

                ingredientQuantities.Add(ingredientQuantity);
            }

            return ingredientQuantities;
        }
    }


    public class MealCsv
    {
        public string name { get; set; }

        public MealCategory category { get; set; }

        public int time { get; set; }

        public string ingredients { get; set; }
    }

}