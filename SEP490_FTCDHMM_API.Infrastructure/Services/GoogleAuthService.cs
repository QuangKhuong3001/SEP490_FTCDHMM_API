using System.Net.Http.Headers;
using System.Text.Json;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using SEP490_FTCDHMM_API.Application.Dtos.GoogleAuthDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;
using SEP490_FTCDHMM_API.Infrastructure.ModelSettings;

namespace SEP490_FTCDHMM_API.Infrastructure.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly HttpClient _http;
        private readonly GoogleAuthSettings _settings;

        public GoogleAuthService(HttpClient http, IOptions<GoogleAuthSettings> settings)
        {
            _http = http;
            _settings = settings.Value;
        }

        public async Task<GoogleTokenResponse?> ExchangeCodeForTokenAsync(GoogleCodeLoginRequest dto)
        {
            var values = new Dictionary<string, string>
            {
                ["code"] = dto.Code!,
                ["client_id"] = _settings.ClientId,
                ["redirect_uri"] = _settings.RedirectUri,
                ["grant_type"] = "authorization_code",
                ["code_verifier"] = dto.CodeVerifier!
            };

            var res = await _http.PostAsync(_settings.TokenEndpoint, new FormUrlEncodedContent(values));
            if (!res.IsSuccessStatusCode)
            {
                var errorJson = await res.Content.ReadAsStringAsync();
                throw new Exception($"Google Error: {res.StatusCode} - {errorJson}");
            }

            var json = await res.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<GoogleTokenResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<GoogleTokenPayload> ValidateIdTokenAsync(string idToken)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(
                idToken,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _settings.ClientId }
                });

            return new GoogleTokenPayload
            {
                Email = payload.Email,
                EmailVerified = payload.EmailVerified,
                Subject = payload.Subject,
                Name = payload.Name,
                GivenName = payload.GivenName,
                FamilyName = payload.FamilyName,
                PictureUrl = payload.Picture,
            };
        }

        public async Task<GoogleUserInfo?> FetchUserInfoWithPeopleApiAsync(string accessToken)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, _settings.PeopleApiEndpoint);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var res = await _http.SendAsync(req);
            if (!res.IsSuccessStatusCode) return null;

            var json = await res.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var info = new GoogleUserInfo();

            if (root.TryGetProperty("genders", out var genders) && genders.GetArrayLength() > 0)
            {
                info.Gender = genders[0].GetProperty("value").GetString();
            }

            if (root.TryGetProperty("birthdays", out var birthdays) && birthdays.GetArrayLength() > 0)
            {
                var date = birthdays[0].GetProperty("date");

                var year = date.TryGetProperty("year", out var y) ? y.GetInt32() : 1;
                var month = date.GetProperty("month").GetInt32();
                var day = date.GetProperty("day").GetInt32();

                info.Birthday = DateTime.SpecifyKind(
                    new DateTime(year, month, day),
                    DateTimeKind.Unspecified
                );
            }

            return info;
        }


    }
}
