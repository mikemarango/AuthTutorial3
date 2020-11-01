using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
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
      JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // Remove claim mappings
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

      services.AddHttpClient("Auth", options =>
      {
        options.BaseAddress = new Uri(Configuration["Url:Auth"]);
        options.DefaultRequestHeaders.Clear();
        options.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json);
      });

      services.AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme = "Cookies";
        options.DefaultChallengeScheme = "OpenIdConnect";
      })
      .AddCookie("Cookies", options =>
      {
        options.AccessDeniedPath = "/Auth/AccessDenied";
      })
      .AddOpenIdConnect(options =>
      {
        options.SignInScheme = "Cookies";
        options.Authority = Configuration["Url:Auth"];
        options.ClientId = "mvc";
        options.ResponseType = "code";
        options.Scope.Add("address");
        options.Scope.Add("roles");
        options.ClaimActions.DeleteClaim("sid");
        options.ClaimActions.DeleteClaim("idp");
        options.ClaimActions.DeleteClaim("s_hash");
        options.ClaimActions.DeleteClaim("auth_time");
        options.ClaimActions.MapUniqueJsonKey("role", "role");
        options.SaveTokens = true;
        options.ClientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0";
        options.GetClaimsFromUserInfoEndpoint = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
          NameClaimType = "given_name",
          RoleClaimType = "role"
        };
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
