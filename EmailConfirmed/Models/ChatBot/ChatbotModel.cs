using EmailConfirmed.Controllers;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EmailConfirmed.Models.ChatBot
{
    public class ChatbotModel
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public List<ChatbotModel> Subtopics { get; set; }
    }
}
