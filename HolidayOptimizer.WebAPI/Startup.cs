using System;
using System.IO;
using System.Reflection;
using HolidayOptimizer.Core.Providers;
using HolidayOptimizer.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Polly;

namespace HolidayOptimizer.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSingleton<IPublicHolidayProvider, PublicHolidayProvider>();
            services.AddSingleton<ITimezoneProvider, TimezoneProvider>();
            services.AddSingleton<ISupportedCountries, SupportedCountries>();
            services.AddScoped<IHolidayOptimizeService, HolidayOptimizeService>();

            services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new OpenApiInfo { Title = "HolidayOptimizer.WebAPI", Version = "1.0.0", Description = "HolidayOptimizer Web API" });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                swagger.IncludeXmlComments(xmlPath);

            });

            //memory cache
            services.AddMemoryCache();

            //response caching
            services.AddResponseCaching();


            // Configure a client named as "Nager", with various default properties.
            services.AddHttpClient("PublicHolidayApi", client =>
            {
                client.BaseAddress = new Uri("https://date.nager.at/api/v2/PublicHolidays/");
            })
            .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10)
            }));


            // Configure a client named as "TimezoneApi", with various default properties.
            services.AddHttpClient("TimezoneApi", client =>
            {
                client.BaseAddress = new Uri("https://restcountries.eu/rest/v2/alpha/");
            })
            .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10)
            }));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //response caching
                app.UseResponseCaching();
                app.Use(async (context, next) =>
                {
                    context.Response.GetTypedHeaders().CacheControl =
                        new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                        {
                            Public = true,
                            MaxAge = TimeSpan.FromMinutes(5)
                        };
                    context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] =
                        new string[] { "Accept-Encoding" };

                    await next();
                });
            }

            app.UseHttpsRedirection();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "HolidayOptimizer.WebAPI");
                c.RoutePrefix = "swagger";
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
