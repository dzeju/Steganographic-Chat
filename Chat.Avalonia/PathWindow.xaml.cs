using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Drawing;
using Image = System.Drawing.Image;
using MessageBox.Avalonia;

namespace Chat.Avalonia
{
    public class PathWindow : Window
    {
        private ChatMessage _chatMessage;
        private ImageSteganography _image;
        public PathWindow()
        {
            InitializeComponent();
            
            this.FindControl<Button>("BrowseBtn").Click += BrowseBtn_Click;
            this.FindControl<Button>("OkBtn").Click += OkBtn_Click;
        }

        public void PassDataContext(ChatMessage chatMessage, ImageSteganography image)
        {
            _chatMessage = chatMessage;
            _image = image;
            DataContext = _chatMessage;
        }
        
        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _image.MyImage = Image.FromFile(_chatMessage.PathToFile);
                _chatMessage.ErrorMessage = @"Załadowano obraz";
                this.Close(); //gotta add sth dude
            }
            catch (Exception)
            {
                var messageBox = MessageBoxManager.GetMessageBoxStandardWindow(@"Błąd", @"Błąd wczytywania pliku");
                messageBox.Show();
                //_chatMessage.ErrorMessage = exception.Message;
            }
        }

        private async void BrowseBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog()
                {
                    Title = "Open file",
                    // Almost guaranteed to exist
                    //InitialFileName = Assembly.GetEntryAssembly()?.GetModules().FirstOrDefault()?.FullyQualifiedName
                };
                var result = await dialog.ShowAsync((Window) this.VisualRoot);
                _chatMessage.PathToFile = result[0]; // CHECK IF WORKS
            }
            catch (Exception ex)
            {
                _chatMessage.ErrorMessage = ex.Message;
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}