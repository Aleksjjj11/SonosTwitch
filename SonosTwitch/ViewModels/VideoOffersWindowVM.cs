using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SonosTwitch.Commands;
using SonosTwitch.Models;

namespace SonosTwitch.ViewModels
{
    public class VideoOffersWindowVM : BaseVM
    {
        private TwitchBot _clientBot;
        public TwitchBot ClientBot
        {
            get => _clientBot;
            set
            {
                _clientBot = value;
                OnPropertyChanged();
            }
        }
        private AppSetting _appSetting;
        public AppSetting AppSetting
        {
            get => _appSetting;
            set
            {
                _appSetting = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<YoutubeVideoOffer> YoutubeOffers { get; set; }
        private YoutubeVideoOffer _currentVideo;
        private ObservableCollection<YoutubeVideoOffer> _currentQueue;
        public ObservableCollection<YoutubeVideoOffer> CurrentQueue
        {
            get
            {
                _currentQueue = _currentQueue ?? new ObservableCollection<YoutubeVideoOffer>();
                _currentQueue.Clear();
                for (int i = YoutubeOffers.IndexOf(CurrentVideo) + 1; i < YoutubeOffers.Count; i++)
                {
                    _currentQueue.Add(YoutubeOffers[i]);
                }

                return _currentQueue;
            }
            set => _currentQueue = value;
        }

        public YoutubeVideoOffer CurrentVideo
        {
            get => _currentVideo;
            set
            {
                _currentVideo = value;
                OnPropertyChanged(nameof(CurrentVideo));
            }
        }

        public ICommand NextVideoCommand
        {
            get => new RelayCommand(() =>
            {
                if (YoutubeOffers.Count == 0) return;
                var currentIndex = YoutubeOffers.IndexOf(CurrentVideo);
                CurrentVideo = YoutubeOffers[currentIndex + 1];
                OnPropertyChanged(nameof(CurrentQueue));
            }, () => YoutubeOffers.IndexOf(CurrentVideo) < YoutubeOffers.Count - 1);
        }

        public ICommand PreviousVideoCommand
        {
            get => new RelayCommand(() =>
            {
                var currentIndex = YoutubeOffers.IndexOf(CurrentVideo);
                CurrentVideo = YoutubeOffers[currentIndex - 1];
                OnPropertyChanged(nameof(CurrentQueue));
            }, () => YoutubeOffers.IndexOf(CurrentVideo) > 0);
        }

        public ICommand CloseWinCommand
        {
            get => new RelayCommand<Window>(x =>
            {
                x.Close();
            }, x => true);
        }
        public VideoOffersWindowVM(AppSetting setting, TwitchBot bot)
        {
            AppSetting = setting;
            ClientBot = bot;
            ClientBot.OnVideoOfferReceived += (sender, args) =>
            {
                //TODO Сделать преобразование обычной ссылки на видео в формат embed
                YoutubeOffers.Add(
                    new YoutubeVideoOffer(
                         args.ChatMessage.Message.Remove(0, AppSetting.Prefix.Length + AppSetting.VideoReceiveOffer.Length),
                            args.ChatMessage.Username));
                OnPropertyChanged(nameof(CurrentQueue));
            };
            YoutubeOffers = new ObservableCollection<YoutubeVideoOffer>();
        }
    }
}
