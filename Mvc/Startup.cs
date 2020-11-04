using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
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
using Mvc.HttpHandlers;
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
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllersWithViews().AddJsonOptions(opts =>
        opts.JsonSerializerOptions.PropertyNamingPolicy = null);

      JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

      services.AddHttpContextAccessor();

      services.AddAuthorization(options =>
      {
        options.AddPolicy("CanOrder", builder =>
        {
          builder.RequireAuthenticatedUser();
          builder.RequireClaim("country", "be");
          builder.RequireClaim("subscription", "Paying");
          //builder.RequireRole("role", "role"); // Used for roles
        });
      });

      services.AddTransient<BearerTokenHandler>();

      services.AddHttpClient("Api", options =>
      {
        options.BaseAddress = new Uri(Configuration["Url:Api"]);
        options.DefaultRequestHeaders.Clear();
        options.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json);
      })
      .AddHttpMessageHandler<BearerTokenHandler>();

      services.AddHttpClient("Auth", options =>
      {
        options.BaseAddress = new Uri("https://localhost:44300");
        options.DefaultRequestHeaders.Clear();
        options.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
      });

      services.AddAuthentication(options =>
      {
        options.DefaultScheme = "Cookies";
        options.DefaultChallengeScheme = "oidc";
      })
      .AddCookie("Cookies", options =>
      {
        options.AccessDeniedPath = "/Auth/AccessDenied";
      })
      .AddOpenIdConnect("oidc", options =>
      {
        options.Authority = "https://localhost:44300"; //has trailing slash + use pkce false
        options.ClientId = "mvc";
        options.ClientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0";
        options.ResponseType = "code";
        options.SaveTokens = true;
        options.Scope.Add("address");
        options.Scope.Add("roles");
        options.Scope.Add("api");
        options.Scope.Add("subscription");
        options.Scope.Add("country");
        options.ClaimActions.DeleteClaim("address");
        options.ClaimActions.MapUniqueJsonKey("role", "role"); // required in claims identity
        options.ClaimActions.MapUniqueJsonKey("subscription", "subscription");
        options.ClaimActions.MapUniqueJsonKey("country", "country");
        options.TokenValidationParameters = new TokenValidationParameters
        {
          NameClaimType = "given_name",
          RoleClaimType = "role" // subscription??
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
