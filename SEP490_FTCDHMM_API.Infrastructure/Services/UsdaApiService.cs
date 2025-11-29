using System.Text.Json;
using Microsoft.Extensions.Options;
using SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos.USDA;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;
using SEP490_FTCDHMM_API.Infrastructure.ModelSettings;

namespace SEP490_FTCDHMM_API.Infrastructure.Services
{
    public class UsdaApiService : IUsdaApiService
    {
        private readonly HttpClient _http;
        private readonly USDASettings _usdaSettings;
        private readonly int PageSize = 1;

        private static readonly JsonSerializerOptions JsonOpt = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public UsdaApiService(HttpClient http, IOptions<USDASettings> usdaSettings)
        {
            _http = http;
            _usdaSettings = usdaSettings.Value;
        }

        public async Task<UsdaSearchResult?> SearchAsync(string keyword)
        {
            var apiKey = _usdaSettings.ApiKey;

            var url =
                $"https://api.nal.usda.gov/fdc/v1/foods/search" +
                $"?query={Uri.EscapeDataString(keyword)}" +
                $"&dataType=Foundation,SR%20Legacy,FNDDS" +
                $"&pageSize={PageSize}" +
                $"&api_key={apiKey}";

            using var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return new();

            var json = await resp.Content.ReadAsStringAsync();
            var dto = JsonSerializer.Deserialize<UsdaSearchRawResponse>(json, JsonOpt);

            if (dto?.Foods == null || dto.Foods.Count == 0)
                return null;

            var valid = dto.Foods.Where(IsValidIngredient).ToList();
            if (valid.Count == 0)
                return null;

            var best = valid.First();

            return new UsdaSearchResult
            {
                FdcId = best.FdcId,
                Description = best.Description
            };
        }

        public async Task<UsdaFoodDetail?> GetDetailAsync(int fdcId)
        {
            var apiKey = _usdaSettings.ApiKey;
            var url = $"https://api.nal.usda.gov/fdc/v1/food/{fdcId}?api_key={apiKey}";

            using var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return null;

            var json = await resp.Content.ReadAsStringAsync();
            var dto = JsonSerializer.Deserialize<UsdaFoodDetailRaw>(json, JsonOpt);

            if (dto == null)
                return null;

            return new UsdaFoodDetail
            {
                FdcId = dto.FdcId,
                Description = dto.Description,
                FoodNutrients = dto.FoodNutrients
                    .Select(n => new UsdaFoodNutrient
                    {
                        Amount = (decimal?)n.Amount,
                        Nutrient = new UsdaNutrientInfo
                        {
                            Id = n.Nutrient.Id,
                            Name = n.Nutrient.Name,
                            UnitName = n.Nutrient.UnitName
                        }
                    })
                    .ToList()
            };
        }
        private sealed class UsdaSearchRawResponse
        {
            public List<UsdaSearchRawFood> Foods { get; set; } = new();
        }

        private sealed class UsdaSearchRawFood
        {
            public int FdcId { get; set; }
            public string Description { get; set; } = string.Empty;
            public string? DataType { get; set; }
        }

        private sealed class UsdaFoodDetailRaw
        {
            public int FdcId { get; set; }
            public string Description { get; set; } = string.Empty;
            public List<UsdaFoodNutrientRaw> FoodNutrients { get; set; } = new();
        }

        private sealed class UsdaFoodNutrientRaw
        {
            public double? Amount { get; set; }
            public UsdaNutrientRaw Nutrient { get; set; } = new();
        }

        private sealed class UsdaNutrientRaw
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string UnitName { get; set; } = string.Empty;
        }

        private static bool IsValidIngredient(UsdaSearchRawFood f)
        {
            if (string.IsNullOrWhiteSpace(f.Description))
                return false;

            var name = f.Description.ToLower();

            string[] banned =
            {
                "prepared", "instant", "frozen", "cooked", "baked",
                "recipe", "dish", "entrée", "entree", "packed", "canned",
                "sauce", "meal", "mix"
            };

            return !banned.Any(x => name.Contains(x));
        }
    }
}
