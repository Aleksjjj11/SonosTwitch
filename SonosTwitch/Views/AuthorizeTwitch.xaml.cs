using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using SonosTwitch.ViewModels;

namespace SonosTwitch
{
    public partial class AuthorizeTwitch : Window
    {
        private AuthorizeTwitchVM viewModel;
        public AuthorizeTwitch()
        {
            InitializeComponent();
            DataContext = viewModel = new AuthorizeTwitchVM(); //Init view model for the window 
        }

        private void AuthorizeTwitch_OnClosed(object sender, EventArgs e)
        {
            
        }

        private void ButtonLogin_OnClick(object sender, RoutedEventArgs e)
        {
            var owner = this.Owner as MainWindow;
            var nameChannel = TextBoxLogin.Text;
            var token = TextBoxToken.Text;
            try
            {
                owner.ViewModel.ClientBot = new TwitchBot(nameChannel, token);
                foreach (var channel in owner.ViewModel.ClientBot.Client.JoinedChannels)
                {
                    owner.ViewModel.ClientBot.Client.LeaveChannel(channel);
                }
                owner.ViewModel.ClientBot.Client.JoinChannel(nameChannel);
                App.Setting.TwitchChannel = nameChannel;
                App.Setting.TwitchToken = token;
                (Owner as MainWindow).LabelTwitchChannel.Content = App.Setting.TwitchChannel;
                TextBlockMightName.Foreground = new SolidColorBrush(Colors.Chartreuse);
                TextBlockMightName.Text = "Канал успешно изменён";
                TextBlockMightName.Visibility = Visibility.Visible;
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                TextBlockMightName.Foreground = new SolidColorBrush(Colors.Red);
                TextBlockMightName.Text = "Произошла ошибка";
                TextBlockMightName.Visibility = Visibility.Visible;
            }
        }

        private void ButtonExit_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
            Owner.Show();
        }

        private void TextChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TextBoxLogin.Text != "" && TextBoxToken.Text != "")
                {
                    ButtonLogin.IsEnabled = true;
                    ButtonLogin.Style = Application.Current.FindResource("ButtonGreen") as Style;
                }
                else
                {
                    ButtonLogin.IsEnabled = false;
                    ButtonLogin.Style = Application.Current.FindResource("ButtonBlue") as Style;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start((sender as Hyperlink).NavigateUri.ToString());
        }
    }
}