using Api.Data;
using Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class UserProfilesController : ControllerBase
  {
    private readonly GalleryContext _context;

    public UserProfilesController(GalleryContext context)
    {
      _context = context;
    }
    // GET: api/<UserProfilesController>
    [Authorize(Policy = "SubjectMatchesUser")]
    [HttpGet("{subject}", Name = "GetUserProfile")]
    public async Task<IActionResult> GetProfileAsync(string subject)
    {
      var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.Subject.Equals(subject));

      if (userProfile is null)
      {
        var subjectValue = User.Claims.FirstOrDefault(c => c.Type.Equals("sub")).Value;

        userProfile = new UserProfile()
        {
          Subject = subjectValue,
          SubscriptionLevel = "FreeUser"
        };

        _context.Add(userProfile);
        await _context.SaveChangesAsync();
      }

      return Ok(new ApplicationUser
      {
        Id = userProfile.Id,
        Subject = userProfile.Subject,
        SubscriptionLevel = userProfile.SubscriptionLevel
      });
    }

    [HttpPost]
    public void Post([FromBody] string value)
    {
    }

  }
}
