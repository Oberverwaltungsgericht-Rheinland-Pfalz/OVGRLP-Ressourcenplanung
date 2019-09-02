using DbRaumplanung.Contracts;
using DbRaumplanung.DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VueCliMiddleware;
using Microsoft.OpenApi.Models;
using System;
using Newtonsoft.Json.Serialization;

namespace AspNetCoreVueStarter
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
            services.AddMvc()
               .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            services.AddScoped<IGadgetStore, GadgetStore>();
            services.AddScoped<IUserStore, UserStore>();
            services.AddScoped<IAllocationPurposeStore, AllocatoinPurposeStore>();
            services.AddScoped<IAllocationStore, AllocationStore>();
            services.AddScoped<IRessourceStore, RessourceStore>();
            services.AddScoped<ISupplierGroupStore, SupplierGroupStore>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddDbContext<RpDbContext>(
              options => {
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
                app.UseExceptionHandler("/Error");
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

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
                    // run npm process with client app
                    spa.UseVueCli(npmScript: "serve", port: 8080, regex: "Compiled ");
                    // if you just prefer to proxy requests from client app, use proxy to SPA dev server instead:
                    // app should be already running before starting a .NET client
                    // spa.UseProxyToSpaDevelopmentServer("http://localhost:8080"); // your Vue app port
                }
            });
        }
    }
}
