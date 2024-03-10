using System.Windows;
using System.Windows.Controls;

namespace SpotifyBinder.UserController
{
    public partial class AKeyChanger : UserControl
    {
        public AKeyChanger(string Text, string DefaultContent)
        {
            InitializeComponent();
            Title.Content = Text;
            KeyNotifier.Content = DefaultContent;
        }
    }
}