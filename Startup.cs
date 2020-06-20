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

using LifeApi.Models;
using LifeApi.Repositories;

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
            services.Configure<LifeDatabaseSettings>(
                Configuration.GetSection(nameof(LifeDatabaseSettings)));

            services.AddSingleton<ILifeDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<LifeDatabaseSettings>>().Value);
            
            services.AddSingleton<DatabaseConnection>();
            
            services.AddSingleton<MealsRepository>();
            services.AddSingleton<IngredientsRepository>();

            services.AddCors(options => {
                options.AddPolicy(name: AllowSpecificOrigins,
                    builder => {
                        builder.WithOrigins("http://localhost:3000")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });   

            services.AddControllers()
                .AddNewtonsoftJson(options => options.UseMemberCasing());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
        }
    }
}
