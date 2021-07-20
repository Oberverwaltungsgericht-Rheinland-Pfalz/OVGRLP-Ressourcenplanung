using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
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

      services.AddScoped<ISupporterRemindJob, SupporterRemindJob>();
      services.AddScoped<IRemindJob, RemindJob>();
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

      services.AddHealthChecks()
        .AddDbContextCheck<RpDbContext>("ef");
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
      var remindSTimeString = Configuration["RemindSupporterGroupsIfSetAtTime"];
      //DateTime remindSupporterGroupsAtTime = DateTime.ParseExact("06:00", "HH:mm", CultureInfo.CurrentCulture);
      string remindSupporterHour = "0" , remindSupporterMinute = "0";

      Task.Run(() => {
        Thread.CurrentThread.IsBackground = true;
        while (true)
        {
          WebRequest req = WebRequest.Create(Configuration["siteUrl"] +"/api/health");
          req.GetResponse();
          try
          {
            Thread.Sleep(19 * 60000);
          }
          catch (ThreadAbortException)
          {
            break;
          }
        }
      });

      if (!string.IsNullOrEmpty(remindSTimeString))
      {
        if (remindSTimeString.Contains(":"))
        {
          remindSupporterHour = remindSTimeString.Split(':')[0];
          remindSupporterMinute = remindSTimeString.Split(':')[1];
        } else
        {
        remindSupporterHour = remindSTimeString;
        }
      }
      
        services.AddQuartz(q =>
        {
          q.UseMicrosoftDependencyInjectionScopedJobFactory();
          q.SchedulerName = "Quartz Scheduler for reminder email";

          if (Convert.ToBoolean(hasReminderInt)) // is true when Reminder is unequal to 0
          {
            // Create a "key" for the job
            var reminderJobKey = new JobKey("reminderJobKey");
            // Register the job with the DI container
            q.AddJob<IRemindJob>(opts => opts.WithIdentity(reminderJobKey));
            q.AddTrigger(opts => opts
              .ForJob(reminderJobKey)
              .WithIdentity("reminderJobKey-trigger")
              .WithSimpleSchedule(x => x.WithIntervalInHours(24).RepeatForever()
            ));
          }

          if (!string.IsNullOrEmpty(remindSTimeString))
          {
            var groupRemiderJobKey = new JobKey("groupReminderJobKey");
            q.AddJob<ISupporterRemindJob>(opts => opts.WithIdentity(groupRemiderJobKey));
            // Create a trigger for the job
            q.AddTrigger(opts => opts
                .ForJob(groupRemiderJobKey) // link to the HelloWorldJob
                .WithIdentity("groupRemiderJobKey-trigger") // give the trigger a unique name
                .WithCronSchedule($"0 {remindSupporterMinute} {remindSupporterHour} * * ?")); // run every 5 seconds
                //.WithCronSchedule($"0/30 * * * * ?")); // run every 5 seconds
                                                       //
    }
  });

        services.AddQuartzServer(options =>
        {
          // when shutting down we want jobs to complete gracefully
          options.WaitForJobsToComplete = true;
        });
      
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      app.UseHealthChecks("/health");
//      app.UseRouting();
      app.UseAuthentication();
/*      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers().RequireAuthorization();
        endpoints.MapHealthChecks("/health").AllowAnonymous();
      });
*/
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
