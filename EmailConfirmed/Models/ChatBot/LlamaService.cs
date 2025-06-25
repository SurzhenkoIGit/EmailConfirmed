using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EmailConfirmed.Models.ChatBot
{
    // Новые классы для формата с полем "prompt"
    public class LegacyApiChoice
    {
        public string text { get; set; }
    }

    public class LegacyApiResponse
    {
        public List<LegacyApiChoice> choices { get; set; }
    }

    public class LegacyApiRequest
    {
        public string prompt { get; set; }
        public double temperature { get; set; } = 0.7;
        public int max_tokens { get; set; } = 200; // Ограничим длину ответа
        public bool stream { get; set; } = false;
    }

    // Основной сервис, который теперь общается с LM Studio
    public class LlamaService
    {
        private readonly HttpClient _httpClient;
        // Меняем URL на конечную точку для текстовых дополнений
        private const string LmStudioUrl = "http://localhost:1234/v1/completions";

        public LlamaService()
        {
            _httpClient = new HttpClient();
            // Увеличиваем таймаут до 5 минут, чтобы модель успела загрузиться
            _httpClient.Timeout = TimeSpan.FromMinutes(5);
        }

        public async Task<string> GetResponseAsync(string userInput)
        {
            // Создаем структурированный промпт, чтобы "направить" модель и дать ей контекст.
            // Это стандартный формат ChatML, который понимают многие современные модели.
            var structuredPrompt = $"<|im_start|>system\nYou are a helpful assistant.<|im_end|>\n<|im_start|>user\n{userInput}<|im_end|>\n<|im_start|>assistant\n";

            var requestPayload = new LegacyApiRequest
            {
                prompt = structuredPrompt
            };

            var jsonPayload = JsonSerializer.Serialize(requestPayload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(LmStudioUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return $"Ошибка сервера: {response.StatusCode}. Ответ: {errorContent}";
                }

                var responseJson = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<LegacyApiResponse>(responseJson);

                if (apiResponse?.choices != null && apiResponse.choices.Count > 0)
                {
                    // Получаем ответ из поля 'text'
                    return apiResponse.choices[0].text.Trim();
                }

                return "Не удалось получить ответ от модели.";
            }
            catch (HttpRequestException ex)
            {
                return $"Не удалось подключиться к локальному серверу LM Studio. Убедитесь, что он запущен. Ошибка: {ex.Message}";
            }
        }
    }
}
