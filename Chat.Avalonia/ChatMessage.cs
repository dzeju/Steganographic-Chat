using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using JetBrains.Annotations;

namespace Chat.Avalonia
{
    public sealed class ChatMessage : INotifyPropertyChanged
    {
        private string _messageToHide;
        private string _errorMessage;
        private string _pathToFile;
        private ObservableCollection<Message.GoBetweenMessage> _messageList;
        private ObservableCollection<string> _stringList;

        public string ConcealedFilePath { get; set; }

        public string BlobName { get; set; }

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

        public string PathToFile
        {
            get => _pathToFile;
            set
            {
                _pathToFile = value;
                OnPropertyChanged(nameof(PathToFile));
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

        public string MessageToHide
        {
            get => _messageToHide;
            set
            {
                _messageToHide = value;
                OnPropertyChanged(nameof(MessageToHide));
            }
        }
        
        public ICommand SendMessageCommand { get; }
        
        public ChatMessage(SignalRChatService signalRChatService)
        {
            _messageToHide = string.Empty;
            _errorMessage = string.Empty;
            BlobName = string.Empty;
            _messageList = new ObservableCollection<Message.GoBetweenMessage>();
            _stringList = new ObservableCollection<string>();
            ConcealedFilePath = string.Empty;
            
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

        private async void SignalRChatServiceOnMessageReceived(Message.GoBetweenMessage message)
        {
            var downloadPath = await Blobs.DownloadImageAsync(message.Message);
            var image = new ImageSteganography();
            var mess = image.RevealMessage(Image.FromFile(downloadPath));
            //MessageList.Add(message);
            StringList.Add(mess);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}