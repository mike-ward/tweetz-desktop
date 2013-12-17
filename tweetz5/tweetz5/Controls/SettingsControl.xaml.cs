using System.Windows.Input;

namespace tweetz5.Controls
{
    public partial class SettingsControl
    {
        public SettingsControl()
        {
            InitializeComponent();
            CommandBindings.Add(new CommandBinding(Commands.ChangeTheme.Command, Commands.ChangeTheme.CommandHandler));
        }
    }
}