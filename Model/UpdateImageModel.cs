using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Model
{
  public class UpdateImageModel
  {
    [Required, MaxLength(150)]
    [JsonPropertyName("title")]
    public string Title { get; set; }
  }
}
