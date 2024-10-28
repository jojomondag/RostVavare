using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using System;
using System.IO;
using System.Linq;
// Make sure WhisperService is available or comment out the line below
using RöstVävare.Services;

namespace RöstVävare.Views
{
    public partial class MainWindow : Window
    {
        // Comment out if WhisperService is not available
        private WhisperService _whisperService;

        public MainWindow()
        {
            InitializeComponent();
            // Initialize WhisperService if available
            _whisperService = new WhisperService("ggml-large-v3-turbo.bin");
        }

        private async void RunButton_Click(object sender, RoutedEventArgs e)
        {
            OutputTextBox.Text = string.Empty;

            var selectedItem = CommandComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem?.Content == null)
            {
                OutputTextBox.Text = "Please select a command.";
                return;
            }

            string command = selectedItem.Content?.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(command))
            {
                OutputTextBox.Text = "Please select a command.";
                return;
            }

            string fileName = FileTextBox.Text ?? string.Empty;
            string language = LanguageTextBox.Text ?? "auto";

            if (string.IsNullOrWhiteSpace(fileName) || !File.Exists(fileName))
            {
                OutputTextBox.Text = "Audio file does not exist.";
                return;
            }

            try
            {
                // Ensure WhisperService methods are available
                await _whisperService.DownloadModelAsync();

                switch (command)
                {
                    case "Language Detection":
                        var detectedLanguage = _whisperService.LanguageIdentification(fileName, language) ?? "Unknown";
                        OutputTextBox.Text = "Detected Language: " + detectedLanguage;
                        break;

                    case "Transcribe":
                    case "Translate":
                        bool translate = command == "Translate";
                        await _whisperService.TranscribeAsync(fileName, language, translate, OnSegmentReceived);
                        break;

                    default:
                        OutputTextBox.Text = "Unknown command.";
                        break;
                }
            }
            catch (FileNotFoundException ex)
            {
                OutputTextBox.Text = ex.Message;
            }
            catch (Exception ex)
            {
                OutputTextBox.Text = "An error occurred: " + ex.Message;
            }
        }

        private void OnSegmentReceived(string segmentText)
        {
            Dispatcher.UIThread.Post(() =>
            {
                OutputTextBox.Text += segmentText + Environment.NewLine;
            });
        }

        private async void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var options = new FilePickerOpenOptions
            {
                Title = "Select Audio File",
                FileTypeFilter = new FilePickerFileType[]
                {
                    new FilePickerFileType("Wave Files") { Patterns = new[] { "*.wav" } },
                    new FilePickerFileType("All Files") { Patterns = new[] { "*" } }
                },
                AllowMultiple = false
            };

            var result = await this.StorageProvider.OpenFilePickerAsync(options);

            if (result != null && result.Any())
            {
                var file = result[0];
                FileTextBox.Text = file.Path.LocalPath;
            }
        }
    }
}
