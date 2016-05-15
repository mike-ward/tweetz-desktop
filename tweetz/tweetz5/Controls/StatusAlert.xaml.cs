using System.Windows.Input;

namespace tweetz5.Controls
{
    public partial class StatusAlert
    {
        public StatusAlert()
        {
            InitializeComponent();
        }

        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            IsOpen = false;
        }
    }
}