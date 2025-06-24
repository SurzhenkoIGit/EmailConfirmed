using EmailConfirmed.Controllers;
using EmailConfirmed.Models.ChatBot;

namespace EmailConfirmed.Data
{
    public class TopicsData
    {
        public List<ChatbotModel> Topics { get; set; }
    }
    public class ResponsesData
    {
        public ResponseContent Responses { get; set; }
    }
    public class ResponseContent
    {
        public List<string> Greetings { get; set; }
        public List<string> Farewells { get; set; }
        public List<string> Thanks { get; set; }
        public Dictionary<string, string> Topics { get; set; }
    }
}
