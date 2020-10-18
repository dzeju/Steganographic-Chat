using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Microsoft.AspNetCore.SignalR.Client;
using System.Linq;
using System.Reflection;
using static System.String;

namespace Chat.Avalonia
{
    public class MainWindow : Window
    {
        private readonly ChatMessage _chatMessage;
        public MainWindow()
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/chat")
                .Build();
            
            _chatMessage = new ChatMessage(new SignalRChatService(connection));

            InitializeComponent();
            
            DataContext = _chatMessage;
            this.FindControl<Button>("SendBtn").Click += SendBtn_Click;
            this.FindControl<Button>("BrowseBtn").Click += BrowseBtn_Click;
            this.FindControl<TextBox>("MessTxtBox").KeyDown += OnKeyDown;
        }

        private async void BrowseBtn_Click(object sender, RoutedEventArgs e)
        {
            await new OpenFileDialog()
            {
                Title = "Open file",
                // Almost guaranteed to exist
                InitialFileName = Assembly.GetEntryAssembly()?.GetModules().FirstOrDefault()?.FullyQualifiedName
            }.ShowAsync((Window)this.VisualRoot);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendBtn_Click(this, new RoutedEventArgs());
            }
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            _chatMessage.SendMessageCommand.Execute(sender);
            
            this.FindControl<TextBox>("MessTxtBox").Text = Empty;
        }
        
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}