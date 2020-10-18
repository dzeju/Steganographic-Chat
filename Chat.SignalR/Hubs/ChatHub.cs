using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Chat.SignalR
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(Message.GoBetweenMessage message)
        {
            await Clients.Others.SendAsync("ReceiveMessage", message);
        }
    }
}