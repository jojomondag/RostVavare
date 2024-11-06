using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using NAudio.Wave;

namespace RöstVävare.Views.Components
{
    public partial class MicrofoneComponent : UserControl
    {
        private ComboBox _microphoneComboBox;

        public MicrofoneComponent()
        {
            InitializeComponent();
            _microphoneComboBox = this.FindControl<ComboBox>("MicrofoneComboBox");
            this.FindControl<Button>("MicrofoneButton").Click += OnButtonClick;
            PopulateMicrophoneList();
        }

        private void PopulateMicrophoneList()
        {
            var waveInDevices = Enumerable.Range(0, WaveIn.DeviceCount)
                .Select(n => WaveIn.GetCapabilities(n).ProductName)
                .ToList();

            _microphoneComboBox.Items = waveInDevices;
            if (waveInDevices.Any())
            {
                _microphoneComboBox.SelectedIndex = 0;
            }
        }

        private async void OnButtonClick(object? sender, RoutedEventArgs e)
        {
            try
            {
                var selectedMicrophone = _microphoneComboBox.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(selectedMicrophone))
                {
                    Console.WriteLine("No microphone selected.");
                    return;
                }

                // Display a message box
                var box = MessageBoxManager
                    .GetMessageBoxStandard("Microphone Selected", $"You have selected: {selectedMicrophone}",
                        ButtonEnum.Ok);

                await box.ShowAsync();
            }
            catch (Exception ex)
            {
                // Log the exception or show an error message
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
