using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Model
{
  public class ImageModel
  {
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    [Required, MaxLength(150)]
    [JsonPropertyName("title")]
    public string Title { get; set; }
    [Required, MaxLength(200)]
    [JsonPropertyName("fileName")]
    public string FileName { get; set; }
  }
}
