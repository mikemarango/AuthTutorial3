using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Entities
{
  public class Image
  {
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string FileName { get; set; }
    public string OwnerId { get; set; }
  }
}
