using SonosTwitch.Models;
using System.Collections.ObjectModel;

namespace SonosTwitch.ViewModels
{
    public class MainWindowVM : BaseVM
    {
        private TwitchBot clientBot;
        public TwitchBot ClientBot
        {
            get => clientBot;
            set
            {
                clientBot = value;
                OnPropertyChanged("ClientBot");
            }
        }
        private AppSetting appSetting;
        public AppSetting AppSetting
        {
            get => appSetting;
            set
            {
                appSetting = value;
                OnPropertyChanged("AppSetting");
            }
        }
        public ObservableCollection<Offer> queueOffers;
        public ObservableCollection<Offer> QueueOffers
        {
            get => queueOffers;
            set
            {
                queueOffers = value;
                OnPropertyChanged("QueueOffers");
            }
        }
        private ObservableCollection<Sound> sounds;
        public ObservableCollection<Sound> Sounds
        {
            get => sounds; 
            set
            {
                sounds = value;
                OnPropertyChanged("Sounds");
            } 
        }

        public MainWindowVM(TwitchBot client, AppSetting setting)
        {
            clientBot = client;
            appSetting = setting;
            queueOffers = new ObservableCollection<Offer>();
            sounds = new ObservableCollection<Sound>();
        }
    }
}