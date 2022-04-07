using AllOverIt.AspNetCore.Extensions;
using AllOverIt.Serialization.NewtonsoftJson.Converters;
using EnrichedEnumModelBinding.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EnrichedEnumModelBinding
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
            services
                .AddControllers(options =>
                {
                    // Equivalent to: options.ModelBinderProviders.Insert(0, new EnrichedEnumModelBinderProvider());
                    options.AddEnrichedEnumModelBinder();
                })

                // When using NewtonsoftJson via Microsoft.AspNetCore.Mvc.NewtonsoftJson
                .AddNewtonsoftJson(options =>
                {
                    // alternative to creating a class inherited from EnrichedEnumJsonConverter<>
                    options.SerializerSettings.Converters.Add(EnrichedEnumJsonConverter<ForecastPeriod>.Create());

                    // The controller uses the local time but, for testing, this converter changes the kind so it is treated as UTC.
                    options.SerializerSettings.Converters.Add(new DateTimeAsUtcConverter());
                });

                // When using SystemTextJson
                //
                // .AddJsonOptions(options =>
                // {
                //     // alternative to creating a class inherited from EnrichedEnumJsonConverter<>
                //     options.JsonSerializerOptions.Converters.Add(EnrichedEnumJsonConverter<ForecastPeriod>.Create());

                //     // The controller uses the local time but, for testing, this converter changes the kind so it is treated as UTC.
                //     options.JsonSerializerOptions.Converters.Add(new DateTimeAsUtcConverter());
                // });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
