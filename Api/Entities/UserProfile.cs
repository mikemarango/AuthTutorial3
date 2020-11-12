using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Entities
{
  public class UserProfile
  {
    public Guid Id { get; set; }
    [Required, MaxLength(50)]
    public string Subject { get; set; }
    [Required, MaxLength(250)]
    public string SubscriptionLevel { get; set; }
  }
}
