using System.Text.Json;

namespace GummyMeter.Services
{
    public class TmdbService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly string _apiKey;

        public TmdbService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
            _apiKey = _config["Tmdb:ApiKey"];
        }

        public async Task<JsonElement> GetTrendingMoviesAsync()
        {
            var response = await _httpClient.GetAsync($"https://api.themoviedb.org/3/trending/movie/week?api_key={_apiKey}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<JsonElement>(content);
        }

        public async Task<JsonElement> SearchMoviesAsync(string query, int page = 1)
        {
            var response = await _httpClient.GetAsync($"https://api.themoviedb.org/3/search/movie?api_key={_apiKey}&query={Uri.EscapeDataString(query)}&page={page}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<JsonElement>(content);
        }

        public async Task<string> GetUsMpaaRatingAsync(int movieId)
        {
            // 1) Fetch the JSON  
            var response = await _httpClient
                .GetAsync($"https://api.themoviedb.org/3/movie/{movieId}/release_dates?api_key={_apiKey}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            // 2) Parse into a JsonElement  
            var root = JsonSerializer.Deserialize<JsonElement>(content);

            // 3) Drill into "results" → pick the US block  
            if (root.TryGetProperty("results", out var results) &&
                results.ValueKind == JsonValueKind.Array)
            {
                var usEntry = results
                    .EnumerateArray()
                    .FirstOrDefault(r =>
                        string.Equals(
                            r.GetProperty("iso_3166_1").GetString(),
                            "US",
                            StringComparison.OrdinalIgnoreCase
                        )
                    );

                if (usEntry.ValueKind != JsonValueKind.Undefined &&
                    usEntry.TryGetProperty("release_dates", out var rdates) &&
                    rdates.ValueKind == JsonValueKind.Array)
                {
                    // 4) Scan for the first non-empty "certification"  
                    foreach (var rd in rdates.EnumerateArray())
                    {
                        var cert = rd.GetProperty("certification").GetString();
                        if (!string.IsNullOrWhiteSpace(cert))
                            return cert;
                    }
                }
            }

            // 5) Fallback  
            return "None";
        }




        public async Task<JsonElement> GetMovieDetailsAsync(int id)
        {
            var response = await _httpClient.GetAsync($"https://api.themoviedb.org/3/movie/{id}?api_key={_apiKey}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<JsonElement>(content);
        }

        public async Task<JsonElement> GetMovieCreditsAsync(int id)
        {
            var response = await _httpClient.GetAsync($"https://api.themoviedb.org/3/movie/{id}/credits?api_key={_apiKey}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<JsonElement>(content);
        }

        public async Task<JsonElement> GetTopRatedMoviesAsync()
        {
            var response = await _httpClient.GetAsync($"https://api.themoviedb.org/3/movie/top_rated?api_key={_apiKey}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<JsonElement>(content);
        }

        public async Task<JsonElement> GetPopularMoviesAsync()
        {
            var response = await _httpClient.GetAsync($"https://api.themoviedb.org/3/movie/popular?api_key={_apiKey}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<JsonElement>(content);
        }

        public async Task<JsonElement> GetNowPlayingMoviesAsync()
        {
            var response = await _httpClient.GetAsync($"https://api.themoviedb.org/3/movie/now_playing?api_key={_apiKey}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<JsonElement>(content);
        }

        public async Task<JsonElement> GetUpcomingMoviesAsync()
        {
            var response = await _httpClient.GetAsync($"https://api.themoviedb.org/3/movie/upcoming?api_key={_apiKey}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<JsonElement>(content);
        }

        public async Task<JsonElement> GetMovieVideosAsync(int movieId)
        {
            var url = $"https://api.themoviedb.org/3/movie/{movieId}/videos?api_key={_apiKey}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<JsonElement>(content);
        }


        public async Task<JsonElement> DiscoverMoviesAsync(int page = 1)
        {
            var url = $"https://api.themoviedb.org/3/discover/movie?api_key={_apiKey}&page={page}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<JsonElement>(content);
        }

        // Services/TmdbService.cs
        public async Task<JsonElement> GetMoviesByCategoryAsync(string category, int page = 1)
        {
            // map your category names to TMDB endpoints
            string endpoint = category.ToLower() switch
            {
                "trending" => $"trending/movie/day",
                "toprated" => $"movie/top_rated",
                "popular" => $"movie/popular",
                "nowplaying" => $"movie/now_playing",
                "upcoming" => $"movie/upcoming",
                _ => throw new ArgumentException("Unknown category", nameof(category))
            };

            var url = $"https://api.themoviedb.org/3/{endpoint}?api_key={_apiKey}&page={page}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<JsonElement>(content);
        }


    }
}
