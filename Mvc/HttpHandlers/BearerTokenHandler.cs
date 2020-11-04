using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Mvc.HttpHandlers
{
  public class BearerTokenHandler : DelegatingHandler
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHttpClientFactory _httpClientFactory;

    public BearerTokenHandler(IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory)
    {
      _httpContextAccessor = httpContextAccessor;
      _httpClientFactory = httpClientFactory;
    }
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
      //var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");

      //var refreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync("refresh_token");

      var accessToken = await GetAccessTokenAsync();

      if (!string.IsNullOrWhiteSpace(accessToken))
      {
        request.SetBearerToken(accessToken);
      }
      return await base.SendAsync(request, cancellationToken);
    }

    public async Task<string> GetAccessTokenAsync()
    {
      //var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
      var expiresAt = await _httpContextAccessor.HttpContext.GetTokenAsync("expires_at");

      var expiryTime = DateTimeOffset.Parse(expiresAt, CultureInfo.InvariantCulture);

      if (expiryTime.AddSeconds(-60).ToUniversalTime() > DateTime.UtcNow)
      {
        return await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
      }

      var authority = _httpClientFactory.CreateClient("Auth");

      var discoveryDocument = await authority.GetDiscoveryDocumentAsync();

      var refreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync("refresh_token");

      var tokenResponse = await authority.RequestRefreshTokenAsync(new RefreshTokenRequest
      {
        Address = discoveryDocument.TokenEndpoint,
        ClientId = "mvc",
        ClientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0",
        RefreshToken = refreshToken
      });

      var updatedTokens = new AuthenticationToken[]
      {
        new AuthenticationToken
        {
          Name = OpenIdConnectParameterNames.IdToken,
          Value = tokenResponse.IdentityToken
        },
        new AuthenticationToken
        {
          Name = OpenIdConnectParameterNames.AccessToken,
          Value = tokenResponse.AccessToken
        },
        new AuthenticationToken
        {
          Name = OpenIdConnectParameterNames.RefreshToken,
          Value = tokenResponse.RefreshToken
        },
        new AuthenticationToken
        {
          Name = "expires_at",
          Value = (DateTime.UtcNow + TimeSpan.FromSeconds(tokenResponse.ExpiresIn)).
                  ToString("o", CultureInfo.InvariantCulture)
        }
      };

      var authenticateResult = await _httpContextAccessor
                .HttpContext.AuthenticateAsync("Cookies");

      authenticateResult.Properties.StoreTokens(updatedTokens);

      // Sign in

      await _httpContextAccessor.HttpContext.SignInAsync("Cookies", 
        authenticateResult.Principal, authenticateResult.Properties);

      return tokenResponse.AccessToken;
    }
  }
}
