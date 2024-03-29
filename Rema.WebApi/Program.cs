﻿// SPDX-FileCopyrightText: © 2019 Oberverwaltungsgericht Rheinland-Pfalz <poststelle@ovg.jm.rlp.de>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using PdfSharpCore.Fonts;
using PdfSharpCore.Utils;
using Serilog;
using Serilog.Events;

namespace Rema.WebApi
{
  public class Program
  {
    public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
      .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
      .Build();

    public static void Main(string[] args)
    {
      GlobalFontSettings.FontResolver = new FontResolver();

      Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(Configuration)
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .CreateLogger();

      CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
          webBuilder.UseStartup<Startup>();
        })
        .UseSerilog();
  }
}
