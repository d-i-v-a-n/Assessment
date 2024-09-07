using Application;
using Domain;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Persistence;
using Presentation;
using Serilog;
using System.Text;
using WebApi.Configuration;

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

        builder.Services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddDefaultUI();


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

        //builder.Services
        //    .AddAuthentication("Identity.Bearer")
        //    .AddCookie("Identity.Bearer", options =>
        //    {
        //        options.Cookie.Name = "bearer";
        //    });
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "TheIssuer",
                ValidAudience = "TheAudience",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TheSuperSecretKey"))
            };
        });

        builder.Services
            .AddAuthorization();
            
        var app = builder.Build();

        

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.CreateUsersAndRoles().Wait();

        app.UseSerilogRequestLogging();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapGroup("/identity").MapIdentityApi<User>();

        app.AddEndpoints();

        app.Run();
    }
}
