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
using Quartz;
using Rema.DbAccess;
using Rema.Infrastructure.Email;
using Rema.ServiceLayer;
using Rema.ServiceLayer.ControllerLogic;
using Rema.ServiceLayer.Interfaces;
using Rema.ServiceLayer.Jobs;
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

      EmailSettings emailSettings = new EmailSettings();
      emailSettings.SendEmails = this.Configuration.GetValue<Boolean>("SendEmails");
      Configuration.GetSection("Email").Bind(emailSettings);
      services.AddSingleton<EmailSettings>(emailSettings);

      services.AddScoped<IEmailTrigger, EmailTrigger>();
      services.AddScoped<IAllocationService, AllocationService>();
      services.AddSingleton<IAdService>(new AdService(DomainsToSearch));
      services.AddAutoMapper(typeof(Startup));
      services.AddHttpContextAccessor();

      services.AddControllers();
      services.AddMvc(options => options.EnableEndpointRouting = false)
        .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
        .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

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

      // nur wenn die 
      var hasReminder = Configuration["Reminder"];
      var hasReminderInt = Convert.ToInt32(hasReminder);
      if (Convert.ToBoolean(hasReminderInt)) // is true when Reminder is unequal to 0
      {
        services.AddQuartz(q =>
        {
          q.SchedulerName = "Quartz Scheduler for reminder email";
          // base quartz scheduler, job and trigger configuration

         /* q.ScheduleJob<TestJob>(trigger => trigger
         // .StartNow()
          .WithSimpleSchedule(x=> x.WithIntervalInSeconds(180).RepeatForever())
          //.UsingJobData(
          ); */
          
          // quickest way to create a job with single trigger is to use ScheduleJob
          // (requires version 3.2)
          q.ScheduleJob<RemindJob>(trigger => trigger
              .WithIdentity("Combined Configuration Trigger")
              .StartNow()
              .WithSimpleSchedule(x => x.WithIntervalInHours(24).RepeatForever())
              .WithDescription("the email reminder is triggert to send mails on daily basis")
          );
          q.ScheduleJob<SupporterRemindJob>(trigger => trigger
            .WithIdentity("Supporter Reminder trigger")
            .WithCronSchedule("0 3 0 ? * *")    // cron intervall für täglich um 3 Uhr
         //   .StartNow()
         //   .WithSimpleSchedule(x => x.WithIntervalInHours(1).RepeatForever())
            .WithDescription("the supporter email reminder is triggert to send mails on daily basis")
           );

          q.UseMicrosoftDependencyInjectionScopedJobFactory();
        });

        // ASP.NET Core hosting
        services.AddQuartzServer(options =>
        {
          // when shutting down we want jobs to complete gracefully
          options.WaitForJobsToComplete = true;
        });
      }
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      app.UseAuthentication();
      app.UseStaticFiles(new StaticFileOptions()
      {
        OnPrepareResponse = context =>
        {
          context.Context.Response.Headers.Add("Cache-Control", "no-cache, no-store");
          context.Context.Response.Headers.Add("Expires", "-1");
        }
      });

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
