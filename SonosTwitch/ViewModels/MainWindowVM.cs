using SonosTwitch.Commands;
using SonosTwitch.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SonosTwitch.Interfaces;
using SonosTwitch.Views;

namespace SonosTwitch.ViewModels
{
    public class MainWindowVM : BaseVM
    {
        const string FileName = @"SavedSetting.bin";
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
        private IAppSetting _appSetting;
        public IAppSetting AppSetting
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
        private string _login;
        private string _token;

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
        public ObservableCollection<Offer> QueueOffers { get; set; }

        public MainWindowVM()
        {
            AppSetting = new AppSettingJson("SettingApp.json");
            ClientBot = new TwitchBot(AppSetting.TwitchChannel, AppSetting.TwitchToken, ref _appSetting);
            QueueOffers = new ObservableCollection<Offer>();
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
        public ICommand AddSoundCommand => new RelayCommand(() =>
        {
            AppSetting.DictionaryCommands.Add(new Sound($"NewCommand{AppSetting.DictionaryCommands.Count}", "Path"));
        }, () => true);

        public ICommand ResetAllSoundsCommand => new RelayCommand(() =>
        {
            AppSetting.DictionaryCommands.Clear();
        }, () => true);

        public ICommand SaveSettingCommand => new RelayCommand(() =>
        {
            AppSetting.Save();
        }, () => true);

        public ICommand HideAppCommand => new RelayCommand<Window>(x =>
        {
            x.Hide();
        }, x => true);

        public ICommand DeclineOfferCommand => new RelayCommand<TextBlock>(x =>
        {
            QueueOffers.Remove(QueueOffers.First(k => k.Id == (x.Tag as string)));
        }, x => true);


        public ICommand AcceptOfferCommand => new RelayCommand<TextBlock>(x =>
        {
            try
            {
                var sound = AppSetting.DictionaryCommands.First(k => k.Command == QueueOffers.First(y => y.Id == x.Tag as string).Message.Replace(AppSetting.Prefix, ""));
                string path = sound.PathSound;
                MediaPlayer player = new MediaPlayer();
                player.Open(new Uri(path));
                player.Volume = sound.Volume;
                player.Play();
                QueueOffers.Remove(QueueOffers.First(y => y.Id == x.Tag as string));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }, x => true);


        public ICommand DeleteSoundCommand => new RelayCommand<string>(x =>
        {
            //Delete sound by X's values from dictionary
            AppSetting.DictionaryCommands.Remove(AppSetting.DictionaryCommands.First(k => k.Command == x));
        }, x => true);

        public ICommand NextVideoCommand => new RelayCommand(() =>
        {
            if (YoutubeOffers.Count == 0) return;
            var currentIndex = YoutubeOffers.IndexOf(CurrentVideo);
            CurrentVideo = YoutubeOffers[currentIndex + 1];
            OnPropertyChanged(nameof(CurrentQueue));
        }, () => YoutubeOffers.IndexOf(CurrentVideo) < YoutubeOffers.Count - 1);

        public ICommand PreviousVideoCommand => new RelayCommand(() =>
        {
            var currentIndex = YoutubeOffers.IndexOf(CurrentVideo);
            CurrentVideo = YoutubeOffers[currentIndex - 1];
            OnPropertyChanged(nameof(CurrentQueue));
        }, () => YoutubeOffers.IndexOf(CurrentVideo) > 0);

        public ICommand CloseWinCommand => new RelayCommand<Window>(x =>
        {
            x.Close();
        }, x => true);
        public ICommand LoginCommand => new RelayCommand(() =>
        {
            try
            {
                ClientBot = new TwitchBot(Login, Token, ref _appSetting);
                AppSetting.TwitchChannel = Login;
                AppSetting.TwitchToken = Token;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }, () => true);

        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                OnPropertyChanged();
            }
        }

        public string Token
        {
            get => _token;
            set
            {
                _token = value;
                OnPropertyChanged();
            }
        }
    }
}