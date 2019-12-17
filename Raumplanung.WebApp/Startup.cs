using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Raumplanung.WebApp.ViewModels;
using VueCliMiddleware;
using Raumplanung.Infrastructure.Contracts.Stores;
using Raumplanung.Data.DataAccess;
using Newtonsoft.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace Raumplanung.WebApp
{
  public class Startup
  {
    public static Role Reader;
    public static Role Editor;
    public static Role Admin;

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddAutoMapper(typeof(Startup));
      // Mapper.AssertConfigurationIsValid();
      services.AddHttpContextAccessor();

      services.AddMvc(option => option.EnableEndpointRouting = false)
        .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
        .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

      services.AddScoped<IGadgetStore, GadgetStore>();
      services.AddScoped<IUserStore, UserStore>();
      services.AddScoped<IAllocationPurposeStore, AllocatoinPurposeStore>();
      services.AddScoped<IAllocationStore, AllocationStore>();
      services.AddScoped<IRessourceStore, RessourceStore>();
      services.AddScoped<ISupplierGroupStore, SupplierGroupStore>();
      //services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
      //    .AddNegotiate();

      // In production, the Vue files will be served from this directory
      services.AddSpaStaticFiles(configuration =>
      {
        configuration.RootPath = "ClientApp/dist";
      });

      services.AddDbContext<RpDbContext>(
        options =>
        {
          options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
          // options.UseSqlServer(connection, b => b.MigrationsAssembly("AspNetCoreVueStarter"));
        });

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
      Reader = new Role() { Level = 0, Name = "Reader", AdDescription = Configuration["Auth:Reader"] };
      Editor = new Role() { Level = 10, Name = "Editor", AdDescription = Configuration["Auth:Editor"] };
      Admin = new Role() { Level = 100, Name = "Admin", AdDescription = Configuration["Auth:Admin"] };
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseCors(policy => policy
          .AllowAnyHeader()
          .AllowAnyMethod()
          .WithOrigins("http://localhost:8080")
          .AllowCredentials());
      }
      else
      {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
        app.UseHttpsRedirection();
      }
      app.UseAuthentication();

      app.UseSwagger();
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
      });

      //app.UseRouting();
      //app.UseAuthorization();
      app.UseStaticFiles();
      app.UseSpaStaticFiles();

      app.UseMvc(routes =>
      {
        routes.MapRoute(
            name: "default",
            template: "{controller}/{action=Index}/{id?}");
      });

      app.UseSpa(spa =>
      {
        spa.Options.SourcePath = "ClientApp";

        if (env.IsDevelopment())
        {
          //spa.UseVueCli(npmScript: "serve");
          spa.UseProxyToSpaDevelopmentServer("http://localhost:8080");
        }
      });
    }
  }
}