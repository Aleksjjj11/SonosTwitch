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
using TwitchLib.Api.Helix.Models.Games;

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
        public ObservableCollection<YoutubeVideoOffer> YoutubeOffers { get; }
        public ObservableCollection<YoutubeVideoOffer> MusicOffers { get; }
        private ObservableCollection<YoutubeVideoOffer> _currentMusicQueue;
        private YoutubeVideoOffer _currentMusic;

        public ObservableCollection<GameOffer> GameOffers
        {
            get
            {
                var result = new ObservableCollection<GameOffer>();
                foreach (var gameOffer in _gameOffers)
                {
                    result.Add(gameOffer);
                }

                return result;
            }
        }
        private ObservableCollection<GameOffer> _gameOffers;
        private YoutubeVideoOffer _currentVideo;
        private ObservableCollection<YoutubeVideoOffer> _currentVideoQueue;
        private string _login;
        private string _token;

        public YoutubeVideoOffer CurrentMusic
        {
            get => _currentMusic;
            set
            {
                _currentMusic = value;
                OnPropertyChanged(nameof(CurrentMusic));
            }
        }

        public ObservableCollection<YoutubeVideoOffer> CurrentVideoQueue
        {
            get
            {
                _currentVideoQueue = _currentVideoQueue ?? new ObservableCollection<YoutubeVideoOffer>();
                _currentVideoQueue.Clear();
                for (int i = YoutubeOffers.IndexOf(CurrentVideo) + 1; i < YoutubeOffers.Count; i++)
                {
                    _currentVideoQueue.Add(YoutubeOffers[i]);
                }

                return _currentVideoQueue;
            }
            set => _currentVideoQueue = value;
        }

        public ObservableCollection<YoutubeVideoOffer> CurrentMusicQueue
        {
            get
            {
                _currentMusicQueue = _currentMusicQueue ?? new ObservableCollection<YoutubeVideoOffer>();
                _currentMusicQueue.Clear();
                for (int i = MusicOffers.IndexOf(CurrentMusic) + 1; i < MusicOffers.Count; i++)
                {
                    _currentMusicQueue.Add(MusicOffers[i]);
                }

                return _currentMusicQueue;
            }
            set => _currentMusicQueue = value;
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
            YoutubeOffers = new ObservableCollection<YoutubeVideoOffer>();
            MusicOffers = new ObservableCollection<YoutubeVideoOffer>();
            _gameOffers = new ObservableCollection<GameOffer>();
            //Action when to receive video offer
            ClientBot.OnVideoOfferReceived += (sender, args) =>
            {
                YoutubeOffers.Add(
                    new YoutubeVideoOffer(
                         args.ChatMessage.Message.Remove(0, AppSetting.Prefix.Length + AppSetting.CommandVideoReceiver.Length),
                            args.ChatMessage.Username));
                OnPropertyChanged(nameof(CurrentVideoQueue));
            };
            //Action when to receive music offer
            ClientBot.OnMusicOfferReceived += (sender, args) =>
            {
                MusicOffers.Add(new YoutubeVideoOffer(
                    args.ChatMessage.Message.Remove(0, AppSetting.Prefix.Length + AppSetting.CommandMusicReceiver.Length),
                    args.ChatMessage.Username, false));
                OnPropertyChanged(nameof(CurrentMusicQueue));
            };
            //Action when to receive game offer
            ClientBot.OnGameOfferReceived += (sender, args) =>
            {
                _gameOffers.Add(new GameOffer(args.ChatMessage.Username,  args.ChatMessage.Message.Remove(0, AppSetting.Prefix.Length + AppSetting.CommandGameReceiver.Length)));
                OnPropertyChanged(nameof(GameOffers));
            };
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
            OnPropertyChanged(nameof(CurrentVideoQueue));
        }, () => YoutubeOffers.IndexOf(CurrentVideo) < YoutubeOffers.Count - 1);

        public ICommand PreviousVideoCommand => new RelayCommand(() =>
        {
            var currentIndex = YoutubeOffers.IndexOf(CurrentVideo);
            CurrentVideo = YoutubeOffers[currentIndex - 1];
            OnPropertyChanged(nameof(CurrentVideoQueue));
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

        public ICommand NextMusicCommand => new RelayCommand(() =>
        {
            if (MusicOffers.Count == 0) return;
            var currentIndex = MusicOffers.IndexOf(CurrentMusic);
            CurrentMusic = MusicOffers[currentIndex + 1];
            OnPropertyChanged(nameof(CurrentMusicQueue));
        }, () => MusicOffers.IndexOf(CurrentMusic) < MusicOffers.Count - 1);

        public ICommand PreviousMusicCommand => new RelayCommand(() =>
        {
            var currentIndex = MusicOffers.IndexOf(CurrentMusic);
            CurrentMusic = MusicOffers[currentIndex - 1];
            OnPropertyChanged(nameof(CurrentMusicQueue));
        }, () => MusicOffers.IndexOf(CurrentMusic) > 0);

        public ICommand DeleteGameOffer => new RelayCommand<object>(index =>
        {
            _gameOffers.RemoveAt((int)index);
            OnPropertyChanged(nameof(GameOffers));
        }, index => index is int);
    }
}