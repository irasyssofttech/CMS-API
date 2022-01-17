using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using MyPlushBuddy.Api.Services;
using MyPlushBuddy.Api.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json.Serialization;
using Microsoft.Data.SqlClient;
using MyPlushBuddy.Api.Middleware;
using Microsoft.Extensions.Logging;
using MyPlushBuddy.Api.Models;
using System.Reflection;
using System.IO;
using IdentityServer4.AccessTokenValidation;

namespace MyPlushBuddy.Api
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthorization();

            services.
                AddControllers(options =>
                {
                    // These are the common return type for all action methods.
                    // So added them in global filter.
                    options.Filters.Add(
                        new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
                    options.Filters.Add(
                    new ProducesResponseTypeAttribute(StatusCodes.Status406NotAcceptable));
                    options.Filters.Add(
                        new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));

                    options.ReturnHttpNotAcceptable = true;

                    var jsonOutputFormatter = options.OutputFormatters
                    .OfType<NewtonsoftJsonOutputFormatter>().FirstOrDefault();

                    if (jsonOutputFormatter != null)
                    {
                        // remove text/json as it isn't the approved media type
                        // for working with JSON at API level
                        if (jsonOutputFormatter.SupportedMediaTypes.Contains("text/json"))
                        {
                            jsonOutputFormatter.SupportedMediaTypes.Remove("text/json");
                        }
                    }
                })
                .AddNewtonsoftJson(setupAction =>
                {
                    setupAction.SerializerSettings.ContractResolver =
                    new CamelCasePropertyNamesContractResolver();
                })
                .AddXmlDataContractSerializerFormatters()
                .ConfigureApiBehaviorOptions(setupAction =>
                {
                    setupAction.InvalidModelStateResponseFactory = context =>
                    {
                        // Create at problme details object
                        var problemDetailsFactory = context.HttpContext.RequestServices
                                                .GetRequiredService<ProblemDetailsFactory>();

                        var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(
                            context.HttpContext,
                            context.ModelState);

                        // Add additional info not added by default
                        problemDetails.Detail = "See the errors field for details.";
                        problemDetails.Instance = context.HttpContext.Request.Path;

                        var actionExecutingContext =
                        context as Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;

                        if ((context.ModelState.ErrorCount > 0) &&
                        (actionExecutingContext?.ActionArguments.Count ==
                        context.ActionDescriptor.Parameters.Count))
                        {
                            problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                            problemDetails.Title = "One or mre validation errors occurred.";

                            return new UnprocessableEntityObjectResult(problemDetails)
                            {
                                ContentTypes = { "application/problem+json" }
                            };

                        };

                        problemDetails.Status = StatusCodes.Status400BadRequest;
                        problemDetails.Title = "One or more errors on the input occurred.";
                        return new BadRequestObjectResult(problemDetails)
                        {
                            ContentTypes = { "application/problem+json" }
                        };
                    };
                });

            string connectionString = configuration["connectionStrings:cmsDBConnectionString"];
            services.AddDbContext<MyPlushBuddyContext>(o =>
            {
                o.UseSqlServer(connectionString);
            });

            services.AddHttpsRedirection((httpsOptions) =>
                {
                    // Comment this port setting during local development.
                    // It only required to uncomment while publishing the code in IIS.
                    //httpsOptions.HttpsPort = 443;
                });

            // register PropertyMappingService for sorting implementation
            services.AddTransient<IPropertyMappingService, PropertyMappingService>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddTransient<IMailService, MailService>();
            services.AddScoped<IPageTagRepository, PageTagRepository>();

            services.AddSwaggerGen(setupAction =>
            {
                setupAction.SwaggerDoc(
                        "MyPlushBuddyOpenAPISpecification",
                        new Microsoft.OpenApi.Models.OpenApiInfo()
                        {
                            Title = "MyPlushBuddy API",
                            Version = "1",
                            Description = "Through this API you can access all the api " +
                            "used for myplusbbuddy ecommerce and myplushbuddy dashboard.",
                            Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                            {
                                Email = "pushkar@irasys.biz",
                                Name = "Pushkar Thapliyal",
                                Url = new Uri("https://www.irasyssofttech.com")
                            }
                        });

                var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);

                setupAction.IncludeXmlComments(xmlCommentsFullPath);
            });

            // register the user info service
            services.AddScoped<IUserInfoService, UserInfoService>();

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "https:://localhost:44398";
                    options.ApiName = "myplushbuddyapi";
                    options.RequireHttpsMetadata = false;
                });


            // Configure CORS so the API allows requests from JavaScript.  
            // For demo purposes, all origins/headers/methods are allowed.  
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOriginsHeadersAndMethods",
                    builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });

            // Add ApplicationInsight to review the ErrorLog.
            // Instrumental Key configuration will be read from configuration file
            services.AddApplicationInsightsTelemetry();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // inject Exception handling middleware
            app.UseApiExceptionHandler(options =>
            {
                options.AddResponseDetails = UpdateApiErrorResponse;
                options.DetermineLogLevel = DetermineLogLevel;
            });

            app.UseHttpsRedirection();

            if (configuration["EnableSwagger"].ToUpper() == "TRUE")
            {
                app.UseSwagger();

                app.UseSwaggerUI(setupAction =>
                {
                    setupAction.SwaggerEndpoint(
                        "/swagger/MyPlushBuddyOpenAPISpecification/swagger.json",
                        "MyPlushBuddy API");

                    // By default swagger open at swagger/index.html.
                    // By setting this value it will open at root (/index.html)
                    setupAction.RoutePrefix = "";
                });
            }

            // Enable CORS
            app.UseCors("AllowAllOriginsHeadersAndMethods");

            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private LogLevel DetermineLogLevel(Exception ex)
        {
            if (ex.Message.StartsWith("cannot open database", StringComparison.InvariantCultureIgnoreCase) ||
                ex.Message.StartsWith("a network-related", StringComparison.InvariantCultureIgnoreCase))
            {
                return LogLevel.Critical;
            }
            return LogLevel.Error;
        }

        private void UpdateApiErrorResponse(HttpContext context, Exception ex, ApiErrorModel error)
        {
            if (ex.GetType().Name == nameof(SqlException))
            {
                error.Detail = "Exception was a database exception!";
            }
        }
    }
}
