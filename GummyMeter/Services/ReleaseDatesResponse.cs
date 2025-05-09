using System.Text.Json.Serialization;

namespace GummyMeter.Services
{
    public class ReleaseDatesResponse
    {
        [JsonPropertyName("certification")]
        public string Certification { get; set; }

        [JsonPropertyName("release_date")]
        public DateTime ReleaseDate { get; set; }
    }
}