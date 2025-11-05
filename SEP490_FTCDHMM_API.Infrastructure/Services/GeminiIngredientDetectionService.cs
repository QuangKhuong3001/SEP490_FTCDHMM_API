using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SEP490_FTCDHMM_API.Application.Dtos.IngredientDetectionDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;

namespace SEP490_FTCDHMM_API.Infrastructure.Services
{
    public class GeminiIngredientDetectionService : IGeminiIngredientDetectionService
    {
        private readonly HttpClient _client;
        private readonly IIngredientRepository _ingredientRepo;
        private readonly string _apiKey;
        private readonly int _timeoutSeconds;

        public GeminiIngredientDetectionService(HttpClient client, IConfiguration config, IIngredientRepository ingredientRepo)
        {
            _client = client;
            _ingredientRepo = ingredientRepo;
            _apiKey = config["Gemini:ApiKey"] ?? throw new Exception("Missing Gemini API key.");
            _timeoutSeconds = int.TryParse(config["Gemini:TimeoutSeconds"], out var timeout) ? timeout : 60;
            _client.Timeout = TimeSpan.FromSeconds(_timeoutSeconds);
        }

        public async Task<List<IngredientDetectionResult>> DetectIngredientsAsync(IFormFile image)
        {
            if (image == null || image.Length == 0)
                throw new ArgumentException("Image file is required.");

            using var ms = new MemoryStream();
            await image.CopyToAsync(ms);
            var base64Image = Convert.ToBase64String(ms.ToArray());

            var ingredients = await _ingredientRepo.GetAllAsync();
            var ingredientNames = string.Join("\n- ", ingredients.Select(i => i.Name));

            var prompt = $@"
                Vấn đề bạn đang gặp phải là sự không ổn định (inconsistency) và nhận diện sai (misclassification) giữa các lần chạy, điều này rất phổ biến khi sử dụng các mô hình thị giác máy tính đa phương thức (multimodal models) với đầu ra yêu cầu định dạng cứng như JSON.Dưới đây là các phương pháp tối ưu hóa để tăng tính chính xác và độ ổn định của phản hồi Gemini, tập trung vào việc cải thiện Prompt (Lệnh) và Thiết lập Mô hình (Model Configuration).1. Tối ưu hóa Prompt để Tăng Độ Chính XácCách bạn viết prompt là yếu tố quan trọng nhất. Hãy làm cho prompt trở nên cụ thể, nghiêm ngặt và tập trung hơn.A. Tăng cường Danh sách Nguyên liệu và Hạn chếThay đổi phần prompt của bạn như sau:Vấn đề hiện tạiGiải pháp tối ưu trong PromptGemini cố gắng tìm kiếm các vật thể ngoài danh sách của bạn.Nhấn mạnh việc CHỈ được chọn từ danh sách.Gemini nhầm lẫn giữa các vật thể tương tự (ví dụ: củ quả màu tím/sẫm).Cung cấp ngữ cảnh hoặc đặc điểm nếu có.Gemini thêm các ký tự thừa hoặc không tuân thủ JSON.Dùng hướng dẫn định dạng cực kỳ nghiêm ngặt.Prompt đề xuất (cải tiến):PlaintextBạn là hệ thống nhận diện nguyên liệu. Nhiệm vụ của bạn là kiểm tra bức ảnh và xác định các nguyên liệu có mặt, nhưng **CHỈ** được chọn từ danh sách sau:
                - ${ingredientNames}

                **QUY TẮC BẮT BUỘC:**
                1. **Đầu ra:** CHỈ là một mảng JSON (JSON Array).
                2. **Không giải thích:** KHÔNG được bao gồm bất kỳ văn bản, ghi chú hoặc lời giải thích nào ngoài JSON.
                3. **Chỉ liệt kê:** Chỉ liệt kê những nguyên liệu có **ĐỘ TIN CẬY CAO** (Confidence > 0.5)
                4. **Không thay thế:** Không được thêm, thay thế hoặc phỏng đoán nguyên liệu. Ví dụ: Nếu thấy cà tím nhưng trong danh sách không có, KHÔNG liệt kê.

                **Định dạng JSON yêu cầu:**
                [
                  {{ ""ingredient"": ""Tên nguyên liệu"", ""confidence"": 0.xx }}
                ]
                ";

            var payload = new
            {
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new object[]
                        {
                            new { text = prompt },
                            new { inline_data = new { mime_type = image.ContentType, data = base64Image } }
                        }
                    }
                }
            };

            var modelName = "gemini-2.5-pro";
            var apiEndpoint = $"https://generativelanguage.googleapis.com/v1beta/models/{modelName}:generateContent?key={_apiKey}";

            var response = await _client.PostAsync(
                apiEndpoint,
                new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            );

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return ParseGeminiResponse(json);
        }

        private List<IngredientDetectionResult> ParseGeminiResponse(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                var text = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                if (string.IsNullOrEmpty(text))
                    return new List<IngredientDetectionResult>();

                var rawJsonText = text.Trim();

                if (rawJsonText.StartsWith("```json", StringComparison.OrdinalIgnoreCase))
                {
                    rawJsonText = rawJsonText.Substring("```json".Length).Trim();
                }

                if (rawJsonText.EndsWith("```"))
                {
                    rawJsonText = rawJsonText.Substring(0, rawJsonText.Length - "```".Length).Trim();
                }

                var parsed = JsonSerializer.Deserialize<List<IngredientDetectionResult>>(rawJsonText,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return parsed ?? new List<IngredientDetectionResult>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Gemini parse error: " + ex.Message);
                Console.WriteLine("Gemini raw response (để debug): " + json);
                return new List<IngredientDetectionResult>();
            }
        }
    }
}
