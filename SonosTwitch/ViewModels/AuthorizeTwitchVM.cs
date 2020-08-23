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
    }
}