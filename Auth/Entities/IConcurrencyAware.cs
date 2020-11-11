using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.Entities
{
  public interface IConcurrencyAware
  {
    public string ConcurrencyStamp { get; set; }
  }
}
