using Model;
using System.Collections.Generic;

namespace Mvc.Models
{
  public class GalleryIndexViewModel
  {
    public IEnumerable<ImageModel> ImageModels { get; private set; }
        = new List<ImageModel>();

    public GalleryIndexViewModel(IEnumerable<ImageModel> imageModels)
    {
      ImageModels = imageModels;
    }
  }
}
