using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace UnleashedAIO.JSON
{
    class goTLSResponse
    {
        // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
        [JsonPropertyName("cookies")]
        public List<string> Cookies { get; set; }

        [JsonPropertyName("body")]
        public string Body { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}
