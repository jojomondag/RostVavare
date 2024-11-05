using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace RöstVävare.Views.Components
{
    public partial class MyNewComponent : UserControl
    {
        public MyNewComponent()
        {
            InitializeComponent();
            this.FindControl<Button>("MyButton").Click += OnButtonClick;
        }

        private async void OnButtonClick(object? sender, RoutedEventArgs e)
        {
            try
            {
                // Display a message box
                var box = MessageBoxManager
                    .GetMessageBoxStandard("Caption", "Are you sure you would like to delete appender_replace_page_1?",
                        ButtonEnum.YesNo);

                var result = await box.ShowAsync();

                // Handle the result if needed
                if (result == ButtonResult.Yes)
                {
                    Console.WriteLine("User chose Yes");
                }
                else
                {
                    Console.WriteLine("User chose No");
                }
            }
            catch (Exception ex)
            {
                // Log the exception or show an error message
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
