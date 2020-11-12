using Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.AuthHandlers
{
  public class SubscriptionLevelHandler : AuthorizationHandler<SubscriptionLevelRequirement>
  {
    private readonly GalleryContext _context;

    public SubscriptionLevelHandler(GalleryContext context)
    {
      _context = context;
    }
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SubscriptionLevelRequirement requirement)
    {
      var subject = context.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

      var subscriptionLevel = (await _context.UserProfiles.FirstOrDefaultAsync(u => u.Subject.Equals(subject)))?.SubscriptionLevel;

      if (subscriptionLevel != requirement.SubscriptionLevel)
      {
        context.Fail();
        await Task.CompletedTask;
      }

      context.Succeed(requirement);
      await Task.CompletedTask;
    }
  }
}
