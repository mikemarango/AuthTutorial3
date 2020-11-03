using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Mvc.HttpHandlers
{
  public class BearerTokenHandler : DelegatingHandler
  {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BearerTokenHandler(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
      var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
      if (!string.IsNullOrWhiteSpace(accessToken))
      {
        request.SetBearerToken(accessToken);
      }
      return await base.SendAsync(request, cancellationToken);
    }
  }
}
