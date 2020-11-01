using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Model;
using Mvc.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mvc.Controllers
{
  [Authorize]
  public class HomeController : Controller
  {
    private readonly ILogger<HomeController> _logger;
    private readonly IHttpClientFactory _clientFactory;

    public HomeController(ILogger<HomeController> logger, IHttpClientFactory clientFactory)
    {
      _logger = logger;
      _clientFactory = clientFactory;
    }

    public async Task<IActionResult> Index()
    {
      await WriteOutIdentityInformation();

      var httpClient = _clientFactory.CreateClient("Api");

      var request = new HttpRequestMessage(HttpMethod.Get, "/images/");

      var response = await httpClient.SendAsync(
        request, HttpCompletionOption.ResponseHeadersRead);

      response.EnsureSuccessStatusCode();

      using var stream = await response.Content.ReadAsStreamAsync();

      var images = await JsonSerializer.DeserializeAsync<IList<ImageModel>>(stream);

      return View(new GalleryIndexViewModel(images));
    }

    public async Task<IActionResult> EditImage(Guid id)
    {
      var httpClient = _clientFactory.CreateClient("Api");

      var request = new HttpRequestMessage(HttpMethod.Get, $"/images/{id}");

      var response = await httpClient.SendAsync(
        request, HttpCompletionOption.ResponseHeadersRead);

      response.EnsureSuccessStatusCode();

      using var stream = await response.Content.ReadAsStreamAsync();

      var deserializeImage = await JsonSerializer.DeserializeAsync<ImageModel>(stream);

      var editImageViewModel = new EditImageViewModel
      {
        Id = deserializeImage.Id,
        Title = deserializeImage.Title
      };

      return View(editImageViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditImage(EditImageViewModel editImageViewModel)
    {
      var updateImageModel = new UpdateImageModel
      {
        Title = editImageViewModel.Title
      };

      var serializedImage = JsonSerializer.Serialize(updateImageModel);

      var httpClient = _clientFactory.CreateClient("Api");

      var request = new HttpRequestMessage(
        HttpMethod.Put, $"/images/{editImageViewModel.Id}")
      {
        Content = new StringContent(
          serializedImage, Encoding.UTF8, MediaTypeNames.Application.Json)
      };

      var response = await httpClient.SendAsync(
        request, HttpCompletionOption.ResponseHeadersRead);

      response.EnsureSuccessStatusCode();

      return RedirectToAction("Index");
    }

    public IActionResult AddImage()
    {
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddImage(AddImageViewModel addImageViewModel)
    {
      var httpClient = _clientFactory.CreateClient("Api");

      var addImageModel = new AddImageModel
      {
        Title = addImageViewModel.Title,
        FileName = addImageViewModel.Files.First().FileName
      };

      var imageFile = addImageViewModel.Files.First();

      if (imageFile.Length > 0)
      {
        using var fileStream = imageFile.OpenReadStream();
        using var memoryStream = new MemoryStream();
        fileStream.CopyTo(memoryStream);
        addImageModel.Bytes = memoryStream.ToArray();
      }

      var serializedImage = JsonSerializer.Serialize(addImageModel);

      var request = new HttpRequestMessage(HttpMethod.Post, "/images")
      {
        Content = new StringContent(serializedImage, Encoding.Unicode, MediaTypeNames.Application.Json)
      };

      var response = await httpClient.SendAsync(
        request, HttpCompletionOption.ResponseHeadersRead);

      response.EnsureSuccessStatusCode();

      return RedirectToAction("Index");

    }

    public async Task<IActionResult> DeleteImage(Guid id)
    {
      var httpClient = _clientFactory.CreateClient("Api");

      var request = new HttpRequestMessage(
        HttpMethod.Delete, $"/images/{id}");

      var response = await httpClient.SendAsync(
        request, HttpCompletionOption.ResponseHeadersRead);

      response.EnsureSuccessStatusCode();

      return RedirectToAction("Index");
    }

    public IActionResult Privacy()
    {
      return View();
    }

    public async Task Logout()
    {
      await HttpContext.SignOutAsync("Cookies");
      await HttpContext.SignOutAsync("OpenIdConnect");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public async Task WriteOutIdentityInformation()
    {
      var identityToken = await HttpContext.GetTokenAsync("id_token");

      Debug.WriteLine($"Identity token: {identityToken}");

      foreach (Claim claim in User.Claims)
      {
        Debug.WriteLine($"Claim type: {claim.Type} - Claim value: {claim.Value}");
      }
    }
  }
}
