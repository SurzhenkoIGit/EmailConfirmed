using Microsoft.AspNetCore.SignalR;

namespace EmailConfirmed.Hubs
{
    public class ChatHub : Hub
    {
        public async Task Send(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
