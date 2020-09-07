using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace Chat.Avalonia
{
    public class SignalRChatService
    {
        private readonly HubConnection _connection;

        public event Action<Message.GoBetweenMessage> MessageReceived;

        public SignalRChatService(HubConnection connection)
        {
            _connection = connection;

            _connection.On<Message.GoBetweenMessage>("ReceiveMessage", message => MessageReceived?.Invoke(message));
        }

        public async Task Connect()
        {
            await _connection.StartAsync();
        }

        public async Task SendMessage(Message.GoBetweenMessage message)
        {
            await _connection.SendAsync("SendMessage", message);
        }
    }
}