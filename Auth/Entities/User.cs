using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.Entities
{
  public class User : IConcurrencyAware
  {
    [Key]
    public Guid Id { get; set; }
    [MaxLength(200), Required]
    public string Subject { get; set; }
    [MaxLength(200)]
    public string UserName { get; set; }
    [MaxLength(200)]
    public string Password { get; set; }
    [Required]
    public bool IsActive { get; set; }
    [ConcurrencyCheck]
    public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    public ICollection<UserClaim> Claims { get; set; } = new List<UserClaim>();
  }
}
