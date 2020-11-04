using Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Api.AuthHandlers
{
  public class OwnershipHandler : AuthorizationHandler<ImageOwnership>
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly GalleryContext _context;

    public OwnershipHandler(IHttpContextAccessor httpContextAccessor, GalleryContext context)
    {
      _httpContextAccessor = httpContextAccessor;
      _context = context;
    }
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ImageOwnership requirement)
    {
      var imageId = _httpContextAccessor.HttpContext.GetRouteValue("id").ToString();
      if (!Guid.TryParse(imageId, out Guid id))
      {
        context.Fail();
        await Task.CompletedTask;
      }

      var ownerId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

      var imageExists = await _context.Images
        .AnyAsync(i => i.Id.Equals(id) && i.OwnerId.Equals(ownerId));

      if (!imageExists)
      {
        context.Fail();
        await Task.CompletedTask;
      }

      context.Succeed(requirement);
      await Task.CompletedTask;
    }
  }
}
