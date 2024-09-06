using Application;
using Domain;
using Domain.Entities;
using Microsoft.OpenApi.Models;
using Persistence;
using Presentation;
using Serilog;

namespace WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });

            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    new string[]{}
                }
            });
        });

        builder.Services.AddIdentityApiEndpoints<User>().AddEntityFrameworkStores<ApplicationDbContext>();

        //builder.Services
        //    .AddIdentityCore<User>()
        //    .AddApiEndpoints()
        //    .AddEntityFrameworkStores<ApplicationDbContext>();
        //builder.Services
        //    .AddIdentityApiEndpoints<User>();

        builder.Services.AddAuthorization();

        builder.Services.AddTransient<IAuthenticated>(provider =>
        {
            var httpContextAccessor = provider.GetService<IHttpContextAccessor>();

            if (httpContextAccessor is not null
                && httpContextAccessor.HttpContext is not null
                && httpContextAccessor.HttpContext.User is not null
                && httpContextAccessor.HttpContext.User.Identity is not null)
            {
                var authenticated = httpContextAccessor.HttpContext.User.Identity.GetUser();

                if (authenticated is not null)
                    return authenticated;
            }

            return null;//new Unauthenticated();
        });

        builder.Services
            .AddApplication()
            .AddInfrastructure()
            .AddPersistence()
            .AddPresentation();

        builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseSerilogRequestLogging();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapGroup("/identity").MapIdentityApi<User>();

        app.AddEndpoints();

        app.Run();
    }
}
