using System.Windows.Input;

namespace AutoShot
{
    static class WindowSystemCommand
    {
        public static RoutedCommand CloseCommand { get; }

        static WindowSystemCommand()
        {
            CloseCommand = new RoutedCommand("Close", typeof(WindowSystemCommand));
        }
    }
}
