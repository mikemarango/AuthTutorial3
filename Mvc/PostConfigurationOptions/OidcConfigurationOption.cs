using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mvc.PostConfigurationOptions
{
  public class OidcConfigurationOption : IPostConfigureOptions<OpenIdConnectOptions>
  {
    private readonly IHttpClientFactory _httpClientFactory;

    public OidcConfigurationOption(IHttpClientFactory httpClientFactory)
    {
      _httpClientFactory = httpClientFactory;
    }
    public void PostConfigure(string name, OpenIdConnectOptions options)
    {
      options.Events = new OpenIdConnectEvents()
      {
        OnTicketReceived = async ticketReceivedContext =>
        {
          var subject = ticketReceivedContext.Principal.Claims
          .FirstOrDefault(c => c.Type == "sub").Value;

          var apiClient = _httpClientFactory
          .CreateClient("ApiClient");

          var request = new HttpRequestMessage(HttpMethod.Get,
              $"/userprofiles/{subject}");

          request.SetBearerToken(ticketReceivedContext
              .Properties.GetTokenValue("access_token"));

          var response = await apiClient.SendAsync(
              request, HttpCompletionOption.ResponseHeadersRead);

          response.EnsureSuccessStatusCode();

          var user = new ApplicationUser();

          using var stream = await response.Content.ReadAsStreamAsync();

          user = await JsonSerializer
              .DeserializeAsync<ApplicationUser>(stream);


          var claimsIdentity = new ClaimsIdentity();

          claimsIdentity.AddClaim(new Claim("subscriptionlevel",
              user.SubscriptionLevel));

          // add this additional identity
          ticketReceivedContext.Principal.AddIdentity(claimsIdentity);



        }
      };
    }
  }
}
