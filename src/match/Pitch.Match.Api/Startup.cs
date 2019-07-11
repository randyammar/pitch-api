﻿using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Pitch.Match.Api.Application.Engine;
using Pitch.Match.Api.Application.Engine.Action;
using Pitch.Match.Api.Application.Engine.Events;
using Pitch.Match.Api.Hubs;
using Pitch.Match.Api.Infrastructure.Repositories;
using Pitch.Match.Api.Models;
using Pitch.Match.Api.Services;
using Pitch.Match.Api.Supporting;

namespace Pitch.Match.Api
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.Authority = Configuration.GetValue<string>("IdentityUrl");
                options.Audience = "cbf24cc4a1bb79e441a5b5937be6dd84";
                options.RequireHttpsMetadata = false;
            });

            services.AddScoped<IMatchService, MatchService>();
            services.AddSingleton<IMatchmakingService, MatchmakingService>();

            services.AddSingleton<IMatchEngine, MatchEngine>();

            services.AddScoped<IMatchRepository, MatchRepository>();

            services.AddSingleton<IAction, Application.Engine.Action.Foul>();
            services.AddSingleton<IAction, Shot>();

            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddRabbitMQ(Configuration.GetConnectionString("RabbitMQHealthCheck"), name: "rabbitmq-check", tags: new string[] { "rabbitmq" });

            services.AddSingleton<IMongoClient>(s =>
            {
                return new MongoClient(Configuration.GetConnectionString("MongoDb"));
            });

            services.AddSingleton(s =>
            {
                return RabbitHutch.CreateBus(Configuration.GetConnectionString("ServiceBus"), serviceRegister =>
                    serviceRegister.Register<ITypeNameSerializer>(serviceProvider => new SimpleTypeNameSerializer()));
            });

            services.AddSignalR(o =>
            {
                o.EnableDetailedErrors = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            BsonClassMap.RegisterClassMap<RedCard>();
            BsonClassMap.RegisterClassMap<YellowCard>();
            BsonClassMap.RegisterClassMap<Goal>();
            BsonClassMap.RegisterClassMap<ShotOnTarget>();
            BsonClassMap.RegisterClassMap<ShotOffTarget>();

            app.UseHealthChecks("/health");
            app.UseHealthChecks("/liveness", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });

            app.UseAuthentication();

            app.UseSignalR(route =>
            {
                route.MapHub<MatchmakingHub>("/hubs/matchmaking");
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
