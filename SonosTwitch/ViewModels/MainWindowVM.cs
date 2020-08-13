using System.Collections.ObjectModel;

namespace SonosTwitch.ViewModels
{
    public class MainWindowVM
    {
        public TwitchBot ClientBot { get; set; }
        public AppSetting AppSetting { get; set; }
        public ObservableCollection<Offer> QueueOffers { get; set; }

        public MainWindowVM(TwitchBot client, AppSetting setting)
        {
            ClientBot = client;
            AppSetting = setting;
            QueueOffers = new ObservableCollection<Offer>();
        }
    }
}