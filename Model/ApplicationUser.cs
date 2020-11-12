using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Model
{
  public class ApplicationUser
  {
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    [JsonPropertyName("subject")]
    public string Subject { get; set; }
    [JsonPropertyName("subscriptionLevel")]
    public string SubscriptionLevel { get; set; }
  }
}
