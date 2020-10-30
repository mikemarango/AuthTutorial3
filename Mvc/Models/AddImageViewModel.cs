using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Mvc.Models
{
  public class AddImageViewModel
  {
    public List<IFormFile> Files { get; set; } = new List<IFormFile>();

    [Required]
    public string Title { get; set; }
  }
}
