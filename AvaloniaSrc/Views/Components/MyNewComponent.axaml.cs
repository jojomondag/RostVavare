using Avalonia.Controls;
using Avalonia.Interactivity;

namespace RöstVävare.Views.Components
{
    public partial class MyNewComponent : UserControl
    {
        public MyNewComponent()
        {
            InitializeComponent();
            this.FindControl<Button>("MyButton").Click += OnButtonClick;
        }

        private void OnButtonClick(object? sender, RoutedEventArgs e)
        {
            // Handle the button click event
            MessageBox.Avalonia.MessageBoxManager
                .GetMessageBoxStandardWindow("Button Clicked", "You clicked the button!")
                .Show();
        }
    }
}
