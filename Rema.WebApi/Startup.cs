using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Rema.DbAccess;
using Rema.DbAccess.Stores;
using Rema.Infrastructure.Contracts.Stores;
using Rema.ServiceLayer;
using Rema.ServiceLayer.Interfaces;
using Rema.ServiceLayer.Services;
using Rema.WebApi.ViewModels;

namespace Rema.WebApi
{
  public class Startup
  {
    public static Role Reader;
    public static Role Editor;
    public static Role Admin;
    public static List<Role> Roles
    {
      get =>
          new List<Role>() { Startup.Reader, Startup.Editor, Startup.Admin };
    }

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      List<string> DomainsToSearch = Configuration.GetSection("DomainsToSearch").Get<List<string>>();

      services.AddSingleton<IAdService>(new AdService(DomainsToSearch));
      services.AddAutoMapper(typeof(Startup));
      services.AddHttpContextAccessor();

      services.AddControllers();
      services.AddMvc(options => options.EnableEndpointRouting = false)
        .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
        .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

      services.AddScoped<IGadgetStore, GadgetStore>();
      services.AddScoped<IUserStore, UserStore>();
      services.AddScoped<IAllocationStore, AllocationStore>();
      services.AddScoped<IRessourceStore, RessourceStore>();
      services.AddScoped<ISupplierGroupStore, SupplierGroupStore>();

      services.AddScoped<IUserService, UsersService>();
      services.AddScoped<ISupplierGroupsService, SupplierGroupsService>();

#if DEBUG
      services.AddDbContext<RpDbContext>(
        options =>
        {
          options
          .EnableSensitiveDataLogging()
          .UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
        });
#else
      services.AddDbContext<RpDbContext>(
      options =>
        {
          options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
        });
#endif

      // Register the Swagger generator, defining 1 or more Swagger documents
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
          Version = "v1",
          Title = "Raumplanungssystem API Dokumentation",
          Description = "Eine Beschreibung der Schnittstellen welche durch ASP.NET Core Web API bereitgestellt werden",
          Contact = new OpenApiContact
          {
            Name = "EDV OVG",
            Email = "OVG_EDV@ovg.jm.rlp.de"
          },
          License = new OpenApiLicense
          {
            Name = "Use under RLP Justiz Lizenz",
            Url = new Uri("https://ovg.justiz.rlp.de"),
          }
        });
      });

      //besser: mit konfiguratiosklasse
      Reader = new Role() { Level = 1, Name = "Reader", AdDescription = Configuration["Auth:Reader"] };
      Editor = new Role() { Level = 10, Name = "Editor", AdDescription = Configuration["Auth:Editor"] };
      Admin = new Role() { Level = 100, Name = "Admin", AdDescription = Configuration["Auth:Admin"] };
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      app.UseAuthentication();
      app.UseStaticFiles();

      app.UseMvc(routes =>
      {
        routes.MapRoute(
          name: "default",
          template: "{controller}/{action=Index}/{id?}");
      });

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseCors(policy => policy
          .AllowAnyHeader()
          .AllowAnyMethod()
          .WithOrigins()
          .AllowCredentials());
      }
      else
      {
        app.UseHsts();
        app.UseHttpsRedirection();
      }
    }
  }
}
