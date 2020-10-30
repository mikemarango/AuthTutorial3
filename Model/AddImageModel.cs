using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Model
{
  public class AddImageModel
  {
    [Required, MaxLength(150)]
    [JsonPropertyName("title")]
    public string Title { get; set; }
    [Required]
    [JsonPropertyName("bytes")]
    public byte[] Bytes { get; set; }
    [JsonPropertyName("fileName")]
    public string FileName { get; set; }
  }
}
