using AllOverIt.AspNetCore.Extensions;
using AllOverIt.Serialization.NewtonsoftJson.Converters;
using AllOverIt.Validation;
using EnrichedEnumModelBinding.Problems;
using EnrichedEnumModelBinding.Requests;
using EnrichedEnumModelBinding.Validators;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
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
            // IETF RFC 7807
            // https://datatracker.ietf.org/doc/html/rfc7807
            services.AddProblemDetails(ProblemDetailsSetup.Configure);

            services
                .AddControllers(options =>
                {
                    // Equivalent to: options.ModelBinderProviders.Insert(0, new EnrichedEnumModelBinderProvider());
                    options.AddEnrichedEnumModelBinder();
                })

                .ConfigureApiBehaviorOptions(options =>
                {
                    // Not used in the demo but showing how to set it up
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var problem = new BadRequestProblem(context);

                        return new BadRequestObjectResult(problem)
                        {
                            ContentTypes = { "application/problem+json" }
                        };
                    };
                })

                // When using NewtonsoftJson via Microsoft.AspNetCore.Mvc.NewtonsoftJson
                .AddNewtonsoftJson(options =>
                {
                    // Can register converters explicitly:
                    // options.SerializerSettings.Converters.Add(EnrichedEnumJsonConverter<ForecastPeriod>.Create());
                    //
                    // Or add a factory that will create suitable converters as they are needed
                    options.SerializerSettings.Converters.Add(new EnrichedEnumJsonConverterFactory());

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

            var validationInvoker = new ValidationInvoker();
            validationInvoker.Register<WeatherRequestMulti, WeatherRequestMultiValidator>();
            services.AddSingleton<IValidationInvoker>(validationInvoker);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment _)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}

            // IETF RFC 7807
            // https://datatracker.ietf.org/doc/html/rfc7807
            // Using 'ProblemDetails Middleware' - https://github.com/khellang/Middleware
            // Note: ConfigureProblemDetails() defaults to the equivalent of app.UseDeveloperExceptionPage() when Environment.IsDevelopment()
            app.UseProblemDetails();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
