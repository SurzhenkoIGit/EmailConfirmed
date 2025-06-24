using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Text.Json;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;

namespace EmailConfirmed.Controllers
{
    public class ApiController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ApiController> _logger;
        public ApiController(IHttpClientFactory clientFactory, IConfiguration configuration, ILogger<ApiController> logger)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetWeather()
        {
            try
            {
                // Создаем имитацию погодных данных
                var random = new Random();
                var weather = new
                {
                    temperature = random.Next(-30, 35),
                    humidity = random.Next(30, 90),
                    windSpeed = random.Next(0, 20),
                    description = GetWeatherDescription(DateTime.Now.Hour),
                    updated = DateTime.Now.ToString("HH:mm")
                };

                return Json(weather);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Weather error: {ex.Message}");
                return BadRequest(ex.Message);
            }

            /*try
            {
                var client = _clientFactory.CreateClient();
                var apiKey = _configuration["ProjectEOL:ApiKey"];
                var url = $"https://api.projecteol.org/data?apikey={apiKey}";

                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var eolData = JObject.Parse(content);

                return Json(eolData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetWeather: {ex.Message}");
                return BadRequest(ex.Message);
            }*/
        }
        private string GetWeatherDescription(int hour)
        {
            if (hour >= 22 || hour < 6) return "Ночь";
            if (hour < 12) return "Утро";
            if (hour < 17) return "День";
            return "Вечер";
        }

        [HttpGet]
        public async Task<IActionResult> GetNews()
        {
            try
            {
                var client = _clientFactory.CreateClient();
                // RSS лента РИА Новости
                var url = "https://ria.ru/export/rss2/archive/index.xml";

                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                using var reader = XmlReader.Create(await response.Content.ReadAsStreamAsync());
                var feed = SyndicationFeed.Load(reader);

                var news = feed.Items.Take(3).Select(item => new
                {
                    title = item.Title.Text,
                    description = item.Summary?.Text ?? "",
                    url = item.Links.FirstOrDefault()?.Uri.ToString(),
                    date = item.PublishDate.DateTime
                });

                return Ok(JsonSerializer.Serialize(news));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetNews: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetQuote()
        {
            try
            {
                // Локальная коллекция цитат
                var quotes = new[]
                {
                    new { content = "Жизнь - это то, что с тобой происходит, пока ты строишь планы.", author = "Джон Леннон" },
                    new { content = "Настоящий успех - это найти дело своей жизни в работе, которую ты любишь.", author = "Дэвид Маккалоу" },
                    new { content = "Единственный способ делать великие дела — любить то, что ты делаешь.", author = "Стив Джобс" },
                    new { content = "Будущее зависит от того, что вы делаете сегодня.", author = "Махатма Ганди" },
                    new { content = "Чтобы дойти до цели, надо идти.", author = "Оноре де Бальзак" }
                };

                var random = new Random();
                var quote = quotes[random.Next(quotes.Length)];

                return Ok(JsonSerializer.Serialize(quote));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetQuote: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
    }
} 
