using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using System.Threading.Tasks;

using LifeApi.Controllers;
using LifeApi.Models;

namespace LifeApi.ScheduledTasks
{
    public class NewMealPlanJob
    {

        private HttpClient httpClient = new HttpClient();

        public NewMealPlanJob(){ 
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            httpClient = new HttpClient(clientHandler);
        }

        public async Task CreateNewMealPlan(){
            int today = (int) DateTime.Today.DayOfWeek;

            if(today == (int) DayOfWeek.Friday || today == (int) DayOfWeek.Saturday || today == (int) DayOfWeek.Sunday) {
                bool noUpcomingPlan = false;

                HttpResponseMessage response = await httpClient.GetAsync("https://localhost:5001/api/meals/plans/latest");

                if(response.StatusCode == HttpStatusCode.NotFound){
                    noUpcomingPlan = true;           
                } else if(response.IsSuccessStatusCode) {
                    var responseContent = await response.Content.ReadAsStringAsync();
                        
                    MealPlan mealPlan = JsonConvert.DeserializeObject<MealPlan>(responseContent);

                    DateTime date = mealPlan.startDate;

                    if(date < DateTime.Today){
                        Console.WriteLine("Latest Plan not in future");
                        noUpcomingPlan = true;
                    }
                } else {
                    Console.WriteLine("Unable to call api");       
                }

                if(noUpcomingPlan){
                    Console.WriteLine("Creating new meal plan");

                    var parameters = new Dictionary<string, string> {{ "numMeals", "5" }};
                    var encodedContent = new FormUrlEncodedContent (parameters);
                                
                    HttpResponseMessage res = await httpClient.PostAsync("https://localhost:5001/api/meals/plans?numMeals=5", encodedContent);            
                }
            }
        }
    }
}