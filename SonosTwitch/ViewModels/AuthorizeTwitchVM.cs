using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using SonosTwitch.Commands;
using SonosTwitch.Interfaces;
using SonosTwitch.Models;

namespace SonosTwitch.ViewModels
{
    public class AuthorizeTwitchVM
    {
        public string Login { get; set; }
        public string Token { get; set; }
        private IAppSetting _setting;
        private TwitchBot _bot;

        public AuthorizeTwitchVM(IAppSetting setting, TwitchBot bot)
        {
            _setting = setting;
            _bot = bot;
            Login = setting.TwitchChannel;
            Token = setting.TwitchToken;
        }
        public void CommandClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("You clicked me");
            /*Close();
            Owner.Show();*/
        }

        public ICommand LoginCommand => new RelayCommand<Window>(x =>
        {
            try
            {
                _bot = new TwitchBot(Login, Token, ref _setting);
                _setting.Save();
                foreach (var channel in _bot.Client.JoinedChannels)
                {
                    _bot.Client.LeaveChannel(channel);
                }
                _bot.Client.JoinChannel(Login);
                /*App.Setting.TwitchChannel = nameChannel;
                App.Setting.TwitchToken = token;*/
                //(Owner as MainWindow).LabelTwitchChannel.Content = _.TwitchChannel;
                
                /*System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);*/
                x.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }, x => string.IsNullOrWhiteSpace(Login) == false && string.IsNullOrWhiteSpace(Token) == false);
    }
}