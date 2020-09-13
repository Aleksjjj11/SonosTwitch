using System.Windows;

namespace SonosTwitch.ViewModels
{
    public class AuthorizeTwitchVM
    {
        public string Login { get; set; }
        public string Token { get; set; }

        public AuthorizeTwitchVM()
        {
            Login = App.Setting.TwitchChannel;
            Token = App.Setting.TwitchToken;
        }
        public void CommandClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("You clicked me");
            /*Close();
            Owner.Show();*/
        }
    }
}