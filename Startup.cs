using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Hangfire;
using Hangfire.MemoryStorage;

using LifeApi.Models;
using LifeApi.Repositories;
using LifeApi.ScheduledTasks;

namespace LifeApi
{
    public class Startup
    {
        readonly string AllowSpecificOrigins = "_allowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHangfire(config => 
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseDefaultTypeSerializer()
                .UseMemoryStorage()
            );

            services.AddHangfireServer();

            services.Configure<LifeDatabaseSettings>(
                Configuration.GetSection(nameof(LifeDatabaseSettings)));

            services.AddSingleton<ILifeDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<LifeDatabaseSettings>>().Value);
            
            services.AddSingleton<DatabaseConnection>();
            
            services.AddSingleton<MealsRepository>();
            services.AddSingleton<IngredientsRepository>();
            services.AddSingleton<MealPlanRepository>();

            services.AddCors(options => {
                options.AddPolicy(name: AllowSpecificOrigins,
                    builder => {
                        builder.WithOrigins("http://localhost:3000")
                            .AllowAnyHeader()
                            .AllowAnyMethod();

                        builder.WithOrigins("http://localhost:5001")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });   

            services.AddControllers()
                .AddNewtonsoftJson(options => options.UseMemberCasing());

            services.AddSingleton<NewMealPlanJob>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IRecurringJobManager recurringJobManager, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(AllowSpecificOrigins);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseHangfireDashboard();

            recurringJobManager.AddOrUpdate(
                "Create New Meal Plan",
                () => serviceProvider.GetService<NewMealPlanJob>().CreateNewMealPlan(),
                "0 */30 * ? * *"
            );
        }
    }
}
