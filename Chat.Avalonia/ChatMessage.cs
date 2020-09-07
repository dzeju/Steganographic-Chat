using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using JetBrains.Annotations;

namespace Chat.Avalonia
{
    public sealed class ChatMessage : INotifyPropertyChanged
    {
        private string _messageToSend;
        private string _errorMessage;
        private ObservableCollection<Message.GoBetweenMessage> _messageList;
        private ObservableCollection<string> _stringList;
        
        public ObservableCollection<string> StringList
        {
            get => _stringList;
            set
            {
                _stringList = value;
                OnPropertyChanged(nameof(StringList));
            }
        }

        public ObservableCollection<Message.GoBetweenMessage> MessageList
        {
            get => _messageList;
            set
            {
                _messageList = value;
                OnPropertyChanged(nameof(MessageList));
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public string MessageToSend
        {
            get => _messageToSend;
            set
            {
                _messageToSend = value;
                OnPropertyChanged(nameof(MessageToSend));
            }
        }
        
        public ICommand SendMessageCommand { get; }
        
        public ChatMessage(SignalRChatService signalRChatService)
        {
            _messageToSend = string.Empty;
            _errorMessage = string.Empty;
            _messageList = new ObservableCollection<Message.GoBetweenMessage>();
            _stringList = new ObservableCollection<string>();
            
            SendMessageCommand = new SendMessageCommand(this, signalRChatService);
            
            signalRChatService.MessageReceived += SignalRChatServiceOnMessageReceived;
            
            signalRChatService.Connect().ContinueWith(task =>
            {
                if(task.Exception != null)
                {
                    ErrorMessage = @"Błąd połączenia z serwerem";
                }
            });
        }

        private void SignalRChatServiceOnMessageReceived(Message.GoBetweenMessage message)
        {
            MessageList.Add(message);
            StringList.Add(message.Message);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}