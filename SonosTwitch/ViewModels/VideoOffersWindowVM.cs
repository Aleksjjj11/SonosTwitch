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

        private ICommand _nextVideoCommand;

        public ICommand NextVideoCommand
        {
            get => _nextVideoCommand ?? new RelayCommand(() =>
            {
                if (YoutubeOffers.Count == 0) return;
                var currentIndex = YoutubeOffers.IndexOf(CurrentVideo);
                CurrentVideo = YoutubeOffers[currentIndex + 1];
                OnPropertyChanged(nameof(CurrentQueue));
            }, () => YoutubeOffers.IndexOf(CurrentVideo) < YoutubeOffers.Count - 1);
        }

        private ICommand _previousVideoCommand;

        public ICommand PreviousVideoCommand
        {
            get => _previousVideoCommand ?? new RelayCommand(() =>
            {
                var currentIndex = YoutubeOffers.IndexOf(CurrentVideo);
                CurrentVideo = YoutubeOffers[currentIndex - 1];
                OnPropertyChanged(nameof(CurrentQueue));
            }, () => YoutubeOffers.IndexOf(CurrentVideo) > 0);
        }

        private ICommand _closeWinCommand;

        public ICommand CloseWinCommand
        {
            get => _closeWinCommand ?? new RelayCommand<Window>(x =>
            {
                x.Close();
            }, x => true);
        }
        public VideoOffersWindowVM()
        {
            YoutubeOffers = new ObservableCollection<YoutubeVideoOffer>
            {
                new YoutubeVideoOffer("https://www.youtube.com/embed/-CNCps_3Tjs", "Me"),
                new YoutubeVideoOffer("https://www.youtube.com/embed/X1gbkD5JhcY", "Username2"),
                new YoutubeVideoOffer("https://www.youtube.com/embed/QAc7lDrXPv8", "Username3"),
                new YoutubeVideoOffer("https://www.youtube.com/embed/1GPsepFmNes", "Username4")
            };
            CurrentVideo = YoutubeOffers.First();
        }
    }
}
