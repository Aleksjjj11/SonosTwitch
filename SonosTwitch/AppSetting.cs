using SonosTwitch.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SonosTwitch
{
    [Serializable]
    public class AppSetting : Item
    {
        private ObservableCollection<Sound> _dictionary;

        public AppSetting()
        {
            DictionaryCommands = new ObservableCollection<Sound>();
        }

        private string _twitchChannel;
        public string TwitchChannel
        {
            get => _twitchChannel;
            set
            {
                _twitchChannel = value;
                OnPropertyChanged(nameof(TwitchChannel));
            }
        }

        private string _prefix;
        public string Prefix
        {
            get => _prefix;
            set
            {
                _prefix = value;
                OnPropertyChanged(nameof(Prefix));
            }
        }

        private string _speechCommand;
        public string SpeechCommand
        {
            get => _speechCommand;
            set
            {
                _speechCommand = value;
                OnPropertyChanged(nameof(SpeechCommand));
            }
        }

        private int _volumeTextSpeech;
        public int VolumeTextSpeech 
        { 
            get => _volumeTextSpeech;
            set 
            {
                _volumeTextSpeech = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VolumeTextSpeech)));
            }
        }
        private bool _receiveFollower;
        public bool ReceiveFollower
        {
            get => _receiveFollower;
            set
            {
                _receiveFollower = value;
                //App.Setting.ReceiveFollower = value;
                OnPropertyChanged(nameof(ReceiveFollower));
            }
        }
        private bool _receiveSubscriber;
        public bool ReceiveSubscriber
        {
            get => _receiveSubscriber;
            set
            {
                _receiveSubscriber = value;
                //App.Setting.ReceiveSubscriber = value;
                OnPropertyChanged(nameof(ReceiveSubscriber));
            }
        }

        private bool _receiveEveryone;
        public bool ReceiveEveryone
        {
            get => _receiveEveryone;
            set
            {
                _receiveEveryone = value;
                //App.Setting.ReceiveEveryone = value;
                OnPropertyChanged(nameof(ReceiveEveryone));
            }
        }

        private bool _isGetOffer;
        public bool IsGetOffer
        {
            get => _isGetOffer;
            set
            {
                _isGetOffer = value;
                OnPropertyChanged(nameof(IsGetOffer));
            }
        }

        private string _twitchToken;
        public string TwitchToken
        {
            get => _twitchToken;
            set
            {
                _twitchToken = value;
                OnPropertyChanged(nameof(TwitchToken));
            }
        }

        private uint _timeout;
        public uint Timeout
        {
            get => _timeout;
            set
            {
                _timeout = value;
                OnPropertyChanged(nameof(Timeout));
            }
        }

        public string CurrentVersion { get; set; }
        public string CommandVideoReceiver { get; set; }
        public string CommandMusicReceiver { get; set; }
        private string _commandGameReceiver;

        public string CommandGameReceiver
        {
            get => _commandGameReceiver;
            set
            {
                _commandGameReceiver = value;
                OnPropertyChanged(nameof(CommandGameReceiver));
            }
        }

        public ObservableCollection<Sound> DictionaryCommands
        {
            get => _dictionary;
            set
            {
                _dictionary = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DictionaryCommands)));
            }
        }
        public DateTime TimeLastSaved { get; set; }
        [field: NonSerialized]
        public DateTime TimeLastLoaded;
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        
    }
}