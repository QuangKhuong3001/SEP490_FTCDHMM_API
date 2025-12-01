using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;

namespace SEP490_FTCDHMM_API.Infrastructure.Services
{
    public class GoogleTranslateService : ITranslateService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;

        public GoogleTranslateService(HttpClient http, IConfiguration cfg)
        {
            _http = http;
            _apiKey = cfg["GoogleTranslate:ApiKey"]
                ?? throw new InvalidOperationException("Missing GoogleTranslate:ApiKey");
        }

        public async Task<string> TranslateToEnglishAsync(string vietnamese)
        {
            var url = $"https://translation.googleapis.com/language/translate/v2?key={_apiKey}";

            var body = new
            {
                q = vietnamese,
                source = "vi",
                target = "en",
                format = "text"
            };

            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            var resp = await _http.PostAsync(url, content);
            resp.EnsureSuccessStatusCode();

            using var stream = await resp.Content.ReadAsStreamAsync();
            var json = await JsonSerializer.DeserializeAsync<JsonElement>(stream);

            return json
                .GetProperty("data")
                .GetProperty("translations")[0]
                .GetProperty("translatedText")
                .GetString() ?? "";
        }

        public async Task<string> TranslateToVietnameseAsync(string english)
        {
            var url = $"https://translation.googleapis.com/language/translate/v2?key={_apiKey}";

            var body = new
            {
                q = english,
                source = "en",
                target = "vi",
                format = "text"
            };

            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            var resp = await _http.PostAsync(url, content);
            resp.EnsureSuccessStatusCode();

            using var stream = await resp.Content.ReadAsStreamAsync();
            var json = await JsonSerializer.DeserializeAsync<JsonElement>(stream);

            return json
                .GetProperty("data")
                .GetProperty("translations")[0]
                .GetProperty("translatedText")
                .GetString() ?? "";
        }
    }

}
