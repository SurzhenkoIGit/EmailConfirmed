using EmailConfirmed.Data;
using EmailConfirmed.Hubs;
using EmailConfirmed.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace EmailConfirmed.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ApplicationContext _context;
        public ChatController(IHubContext<ChatHub> hubContext, ApplicationContext context)
        {
            _hubContext = hubContext;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Chat";

            var messages = await _context.ChatMessages.OrderByDescending(m => m.DepartTime).Take(20).OrderBy(m => m.DepartTime).ToListAsync();
            return View(messages);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessage message)
        {
            if (string.IsNullOrEmpty(message.Text))
                return BadRequest("Сообщение не может быть пустым");

            try
            {
                var chatMessage = new ChatMessage 
                { 
                    User = message.User,
                    Text = message.Text,
                    DepartTime = DateTime.UtcNow
                };

                _context.ChatMessages.Add(chatMessage);
                await _context.SaveChangesAsync();

                await _hubContext.Clients.All.SendAsync("ReceiveMessage", message.User, message.Text);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ошибка отправки сообщения", details = ex.Message });
            }
        }
    }
}
