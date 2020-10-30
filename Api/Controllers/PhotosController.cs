using Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class PhotosController : ControllerBase
  {
    private readonly GalleryContext _context;

    public PhotosController(GalleryContext context)
    {
      _context = context;
    }
    [HttpGet("{fileName}")]
    public async Task<IActionResult> Get(string fileName)
    {
      var image = await _context.Images.FirstOrDefaultAsync(p => p.FileName.Equals(fileName));

      var fileStream = System.IO.File.OpenRead($"photos/{image.FileName}");

      return File(fileStream, "image/jpeg");
    }
  }
}
