// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Api
{
  public class Startup
  {
    public IWebHostEnvironment Environment { get; }
    public IConfiguration Configuration { get; }

    public Startup(IWebHostEnvironment environment, IConfiguration configuration)
    {
      Environment = environment;
      Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllersWithViews().AddJsonOptions(options =>
        options.JsonSerializerOptions.PropertyNamingPolicy = null);

      var builder = services.AddIdentityServer(options =>
      {
        options.Events.RaiseErrorEvents = true;
        options.Events.RaiseInformationEvents = true;
        options.Events.RaiseFailureEvents = true;
        options.Events.RaiseSuccessEvents = true;

              // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
              options.EmitStaticAudienceClaim = true;
      })
          .AddTestUsers(TestUsers.Users);

      //// in-memory, code config
      //builder.AddInMemoryIdentityResources(Config.IdentityResources);
      //builder.AddInMemoryApiScopes(Config.ApiScopes);
      //builder.AddInMemoryApiResources(Config.ApiResources);
      //builder.AddInMemoryClients(Config.Clients);

      // not recommended for production - you need to store your key material somewhere secure
      builder.AddDeveloperSigningCredential();
      //builder.AddSigningCredential(LoadCertificateFromStore());

      builder.AddConfigurationStore(options =>
      {
        options.ConfigureDbContext = builder =>
          builder.UseSqlServer(Configuration.GetConnectionString("LocalSqlConnection"), 
          b => b.MigrationsAssembly("Auth"));
      });

      builder.AddOperationalStore(options =>
      {
        options.ConfigureDbContext = builder =>
          builder.UseSqlServer(Configuration.GetConnectionString("LocalSqlConnection"),
          b => b.MigrationsAssembly("Auth"));
      });

      services.AddAuthentication()
          .AddGoogle(options =>
          {
            options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                  // register your IdentityServer with Google at https://console.developers.google.com
                  // enable the Google+ API
                  // set the redirect URI to https://localhost:5001/signin-google
                  options.ClientId = "copy client ID from Google here";
            options.ClientSecret = "copy client secret from Google here";
          });
    }

    public void Configure(IApplicationBuilder app)
    {
      if (Environment.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      InitializeDatabase(app);

      app.UseStaticFiles();

      app.UseRouting();
      app.UseIdentityServer();
      app.UseAuthorization();
      app.UseEndpoints(endpoints =>
      {
        endpoints.MapDefaultControllerRoute();
      });
    }

    public X509Certificate2 LoadCertificateFromStore()
    {
      string thumbprint = Configuration["ThumbPrint"];
      using var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
      store.Open(OpenFlags.ReadOnly);
      var certCollection = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, true);
      if (certCollection.Count == 0)
      {
        throw new Exception("Certificate not found");
      }
      return certCollection[0];
    }

    private void InitializeDatabase(IApplicationBuilder app)
    {
      using var serviceScope = app.ApplicationServices
        .GetService<IServiceScopeFactory>().CreateScope();

      var context = serviceScope.ServiceProvider
        .GetRequiredService<ConfigurationDbContext>();

      context.Database.Migrate();

      if (!context.Clients.Any())
      {
        foreach (var client in Config.Clients)
        {
          context.Clients.Add(client.ToEntity());
        }
        context.SaveChanges();
      }
      if (!context.IdentityResources.Any())
      {
        foreach (var resource in Config.IdentityResources)
        {
          context.IdentityResources.Add(resource.ToEntity());
        }
        context.SaveChanges();
      }
      if (!context.ApiResources.Any())
      {
        foreach (var resource in Config.ApiResources)
        {
          context.ApiResources.Add(resource.ToEntity());
        }
        context.SaveChanges();
      }
      if (!context.ApiScopes.Any())
      {
        foreach (var scope in Config.ApiScopes)
        {
          context.ApiScopes.Add(scope.ToEntity());
        }
        context.SaveChanges();
      }
    }
  }
}