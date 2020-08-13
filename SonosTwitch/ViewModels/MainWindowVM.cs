namespace SonosTwitch.ViewModels
{
    public class MainWindowVM
    {
        public TwitchBot ClientBot { get; set; }
        public AppSetting AppSetting { get; set; }

        public MainWindowVM(TwitchBot client, AppSetting setting)
        {
            ClientBot = client;
            AppSetting = setting;
        }
    }
}