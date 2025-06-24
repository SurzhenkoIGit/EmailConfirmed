using EmailConfirmed.Models;
using EmailConfirmed.Models.ChatBot;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Headers;
using System.Text;
using EmailConfirmed.Data;
using LLama;

namespace EmailConfirmed.Controllers
{
    public class ChatbotController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly Dictionary<string, ChatbotModel> _topicTree;
        private readonly Dictionary<string, string> _responses;
        private readonly List<string> _greetings;
        private readonly List<string> _farewells;
        private readonly List<string> _thanks;
        private readonly Random _random;
        private readonly LlamaService _service;

        public ChatbotController(IWebHostEnvironment environment, LlamaService service)
        {
            _environment = environment;
            _random = new Random();
            _service = service;
            // Загружаем данные из JSON файлов
            var topicsPath = Path.Combine(_environment.ContentRootPath, "Data", "topics.json");
            var responsesPath = Path.Combine(_environment.ContentRootPath, "Data", "responses.json");

            var topicsJson = System.IO.File.ReadAllText(topicsPath);
            var responsesJson = System.IO.File.ReadAllText(responsesPath);

            var topics = JsonConvert.DeserializeObject<TopicsData>(topicsJson);
            var responses = JsonConvert.DeserializeObject<ResponsesData>(responsesJson);

            _topicTree = topics.Topics.ToDictionary(t => t.Name);
            _responses = responses.Responses.Topics;
            _greetings = responses.Responses.Greetings;
            _farewells = responses.Responses.Farewells;
            _thanks = responses.Responses.Thanks;
        }

        private async Task<string> GetBotResponse(string message)
        {
            message = message.ToLower().Trim();

            if (message.Contains("привет"))
                return _greetings[_random.Next(_greetings.Count)];

            if (message.Contains("пока") || message.Contains("до свидания"))
                return _farewells[_random.Next(_farewells.Count)];

            if (message.Contains("спасибо"))
                return _thanks[_random.Next(_thanks.Count)];

            if (_responses.TryGetValue(message, out string? response))
                return response;
            try
            {
                var botResponse = new StringBuilder();
                foreach(var output in await _service.GetResponseAsync(message))
                {
                    botResponse.Append(output);
                }
                return botResponse.ToString().Trim();
            }
            catch
            {
                return "Извините, я не совсем понял ваш вопрос. Можете переформулировать или выбрать тему из предложенных выше?";
            }
        }

        [HttpGet]
        public IActionResult GetTopics()
        {
            var topicsPath = Path.Combine(_environment.ContentRootPath, "Data", "topics.json");
            var topicsJson = System.IO.File.ReadAllText(topicsPath);
            var topics = JsonConvert.DeserializeObject<TopicsData>(topicsJson);
            return Json(topics?.Topics);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatbotModel request)
        {
            if(string.IsNullOrEmpty(request?.Message) || request.Message.Length > 1000)
            {
                return BadRequest(new { error = "Invalid message" });
            }

            try
            {
                string response = await GetBotResponse(request.Message);
                return Json(new { response });
            }
            catch (Exception ex)
            {
                
                return BadRequest(new { error = ex.Message });
            }
        }

    }
}
