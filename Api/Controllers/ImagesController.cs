using Api.Data;
using Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers
{
  [Route("[controller]")]
  [ApiController]
  //[Authorize]
  [Authorize(Policy = "ApiScope")] // User at api is accessed using the access_token
  public class ImagesController : ControllerBase
  {
    private readonly GalleryContext _context;
    private readonly string _hostEnvironment;

    public ImagesController(GalleryContext context, IWebHostEnvironment hostEnvironment)
    {
      _context = context;
      //_hostEnvironment = hostEnvironment.ContentRootPath; // To use with controllers
      _hostEnvironment = hostEnvironment.WebRootPath;
    }

    private static ImageModel CreateImageModel(Image image)
    {
      return new ImageModel
      {
        Id = image.Id,
        Title = image.Title,
        FileName = image.FileName
      };
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
      var ownerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

      var images = await _context.Images
        .Where(i => i.OwnerId.Equals(ownerId)).AsNoTracking().ToListAsync();

      var imageModels = images.ConvertAll(image => CreateImageModel(image));

      return Ok(imageModels);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
      var image = await _context.Images.FindAsync(id);

      var imageModel = CreateImageModel(image);

      return Ok(imageModel);
    }


    [HttpPost]
    [Authorize(Roles = "Paying")]
    public async Task<IActionResult> Post([FromBody] AddImageModel addImageModel)
    {
      //string fileName = $"{Guid.NewGuid()}.jpg";

      var filePath = $@"{_hostEnvironment}\images\{addImageModel.FileName}";

      await System.IO.File.WriteAllBytesAsync(filePath, addImageModel.Bytes);

      var image = new Image
      {
        Id = Guid.NewGuid(),
        Title = addImageModel.Title,
        FileName = addImageModel.FileName,
        OwnerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
      };

      _context.Images.Add(image);
      await _context.SaveChangesAsync();

      var imageModel = CreateImageModel(image);

      return CreatedAtAction(nameof(Get), new { id = imageModel.Id }, imageModel);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] UpdateImageModel updateImageModel)
    {
      var image = await _context.Images.FindAsync(id);

      image.Title = updateImageModel.Title ?? image.Title;

      _context.Images.Update(image);
      await _context.SaveChangesAsync();

      var imageModel = CreateImageModel(image);

      return AcceptedAtAction(nameof(Get), new { id = imageModel.Id }, imageModel);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
      var image = await _context.Images.FindAsync(id);

      _context.Images.Remove(image);
      await _context.SaveChangesAsync();

      var filePath = $@"{_hostEnvironment}\images\{image.FileName}";

      System.IO.File.Delete(filePath);

      return NoContent();
    }
  }
}
