using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Mvc
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
      services.AddControllersWithViews().AddJsonOptions(opts => 
        opts.JsonSerializerOptions.PropertyNamingPolicy = null);

      services.AddHttpContextAccessor();

      services.AddHttpClient("Api", options =>
      {
        options.BaseAddress = new Uri(Configuration["Url:Api"]);
        options.DefaultRequestHeaders.Clear();
        options.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json);
      });

      services.AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme = "Cookies";
        options.DefaultChallengeScheme = "OpenIdConnect";
      })
      .AddCookie()
      .AddOpenIdConnect(options =>
      {
        options.SignInScheme = "Cookies";
        options.Authority = Configuration["Url:Auth"];
        options.ClientId = "mvc";
        options.ResponseType = "code";
        //options.UsePkce = false;
        //options.Scope.Add("openid");
        //options.Scope.Add("profile");
        options.SaveTokens = true;
        options.ClientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0";
        options.GetClaimsFromUserInfoEndpoint = true;
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }
      app.UseHttpsRedirection();
      app.UseStaticFiles();

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
                  name: "default",
                  pattern: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
}
