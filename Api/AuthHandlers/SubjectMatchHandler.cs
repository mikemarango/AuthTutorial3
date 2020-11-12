using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Api.AuthHandlers
{
  public class SubjectMatchHandler : AuthorizationHandler<SubjectMatchRequirement>
  {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SubjectMatchHandler(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SubjectMatchRequirement requirement)
    {
      var subjectFromUri = _httpContextAccessor.HttpContext.GetRouteValue("subject").ToString();

      var subjectFromUserObject = context.User.Claims.FirstOrDefault(c => c.Type.Equals("sub")).ToString();

      if (subjectFromUri.Equals(subjectFromUserObject))
      {
        context.Fail();
        return Task.CompletedTask;
      }

      context.Succeed(requirement);
      return Task.CompletedTask;
    }
  }
}
