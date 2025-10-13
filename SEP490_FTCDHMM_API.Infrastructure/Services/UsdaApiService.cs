//// Infrastructure/ExternalServices/UsdaApiService.cs
//using System.Text.Json;
//using Microsoft.Extensions.Configuration;
//using SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos;
//using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;

//namespace SEP490_FTCDHMM_API.Infrastructure.Services
//{
//    public class UsdaApiService : IUsdaApiService
//    {
//        private readonly HttpClient _http;
//        private readonly string _apiKey;
//        private static readonly JsonSerializerOptions JsonOpt = new() { PropertyNameCaseInsensitive = true };

//        public UsdaApiService(HttpClient http, IConfiguration cfg)
//        {
//            _http = http;
//            _apiKey = cfg["Usda:ApiKey"] ?? throw new InvalidOperationException("Missing Usda:ApiKey");
//        }

//        public async Task<List<IngredientNameResponse>> GetAsync(string keyword, int take = 5, CancellationToken ct = default)
//        {
//            var url = $"https://api.nal.usda.gov/fdc/v1/foods/search?query={Uri.EscapeDataString(keyword)}&pageSize={take}&api_key={_apiKey}";
//            using var resp = await _http.GetAsync(url, ct);
//            if (!resp.IsSuccessStatusCode) return new();

//            var json = await resp.Content.ReadAsStringAsync(ct);
//            var dto = JsonSerializer.Deserialize<UsdaSearchResponse>(json, JsonOpt);
//            if (dto?.Foods is null || dto.Foods.Count == 0) return new();

//            return dto.Foods
//                .Select(f => new IngredientNameResponse { Id = Guid.Empty, Name = f.Description })
//                .ToList();
//        }

//        private sealed class UsdaSearchResponse
//        {
//            public List<UsdaFood> Foods { get; set; } = new();
//        }
//        private sealed class UsdaFood
//        {
//            public string Description { get; set; } = string.Empty;
//        }
//    }
//}
