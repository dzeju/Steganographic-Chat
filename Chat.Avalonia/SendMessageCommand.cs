using System;
using System.Windows.Input;

namespace Chat.Avalonia
{
    public class SendMessageCommand : ICommand
    {
        private readonly ChatMessage _chatMessage;
        private readonly SignalRChatService _signalRChat;
        
        public SendMessageCommand(ChatMessage chatMessage, SignalRChatService signalRChat)
        {
            _chatMessage = chatMessage;
            _signalRChat = signalRChat;
        }
        
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            try
            {
                if (string.IsNullOrEmpty(_chatMessage.MessageToSend))
                    throw new Exception();
                await _signalRChat.SendMessage(new Message.GoBetweenMessage()
                {
                    Message = _chatMessage.MessageToSend
                });

                _chatMessage.ErrorMessage = string.Empty;
            }
            catch (Exception)
            {
                _chatMessage.ErrorMessage = @"Nie można wysłać wiadomości";
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}