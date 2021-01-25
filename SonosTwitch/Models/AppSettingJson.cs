using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SonosTwitch.Interfaces;

namespace SonosTwitch.Models
{
    class AppSettingJson : Item, IAppSetting
    {
        private readonly string _fileName;
        private string _twitchChannel;
        private string _prefix;
        private string _speechCommand;
        private int _volumeTextSpeech;
        private bool _receiveFollower;
        private bool _receiveSubscriber;
        private bool _receiveEveryone;
        private bool _isGetOffer;
        private string _twitchToken;
        private uint _timeout;
        private string _currentVersion;
        private string _commandVideoReceiver;
        private string _commandMusicReceiver;
        private string _commandGameReceiver;
        private DateTime _timeLastSaved;

        public AppSettingJson(string filename)
        {
            _fileName = filename;
            if(File.Exists(_fileName) == false)
            {
                DictionaryCommands = DictionaryCommands ?? new ObservableCollection<Sound>();
                DictionaryCommands.CollectionChanged += (sender, args) => Save();
                return;
            }
            var jsonText = File.ReadAllText(_fileName);
            var result = JsonConvert.DeserializeObject<AppSettingJson>(jsonText);
            Copy(result);
            DictionaryCommands.CollectionChanged += (sender, args) => Save();
        }

        public string TwitchChannel
        {
            get => _twitchChannel;
            set
            {
                _twitchChannel = value;
                OnPropertyChanged(nameof(TwitchChannel));
            }
        }

        public string Prefix
        {
            get => _prefix;
            set
            {
                _prefix = value;
                OnPropertyChanged();
            }
        }

        public string SpeechCommand
        {
            get => _speechCommand;
            set
            {
                _speechCommand = value;
                OnPropertyChanged();
            }
        }

        public int VolumeTextSpeech
        {
            get => _volumeTextSpeech;
            set
            {
                _volumeTextSpeech = value;
                OnPropertyChanged();
            }
        }

        public bool ReceiveFollower
        {
            get => _receiveFollower;
            set
            {
                _receiveFollower = value;
                OnPropertyChanged();
            }
        }

        public bool ReceiveSubscriber
        {
            get => _receiveSubscriber;
            set
            {
                _receiveSubscriber = value;
                OnPropertyChanged();
            }
        }

        public bool ReceiveEveryone
        {
            get => _receiveEveryone;
            set
            {
                _receiveEveryone = value;
                OnPropertyChanged();
            }
        }

        public bool IsGetOffer
        {
            get => _isGetOffer;
            set
            {
                _isGetOffer = value;
                OnPropertyChanged();
            }
        }

        public string TwitchToken
        {
            get => _twitchToken;
            set
            {
                _twitchToken = value;
                OnPropertyChanged();
            }
        }

        public uint Timeout
        {
            get => _timeout;
            set
            {
                _timeout = value;
                OnPropertyChanged();
            }
        }

        public string CurrentVersion
        {
            get => _currentVersion;
            set
            {
                _currentVersion = value; 
                OnPropertyChanged();
            }
        }

        public string CommandVideoReceiver
        {
            get => _commandVideoReceiver;
            set
            {
                _commandVideoReceiver = value;
                OnPropertyChanged();
            }
        }

        public string CommandMusicReceiver
        {
            get => _commandMusicReceiver;
            set
            {
                _commandMusicReceiver = value;
                OnPropertyChanged();
            }
        }

        public string CommandGameReceiver
        {
            get => _commandGameReceiver;
            set
            {
                _commandGameReceiver = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Sound> DictionaryCommands { get; set; }

        public DateTime TimeLastSaved
        {
            get => _timeLastSaved;
            set
            {
                _timeLastSaved = value;
                OnPropertyChanged();
            }
        }

        public void Save()
        {
            var textToSave = JsonConvert.SerializeObject(this);
            File.WriteAllText(_fileName, textToSave);
        }

        private void Copy(IAppSetting setting)
        {
            this.TwitchChannel = setting.TwitchChannel;
            this.ReceiveEveryone = setting.ReceiveEveryone;
            this.ReceiveSubscriber = setting.ReceiveSubscriber;
            this.ReceiveFollower = setting.ReceiveFollower;
            this.Prefix = setting.Prefix;
            this.SpeechCommand = setting.SpeechCommand;
            this.IsGetOffer = setting.IsGetOffer;
            this.TwitchToken = setting.TwitchToken;
            this.DictionaryCommands = setting.DictionaryCommands;
            this.Timeout = setting.Timeout;
            this.CommandGameReceiver = setting.CommandGameReceiver;
            this.CommandMusicReceiver = setting.CommandMusicReceiver;
            this.CommandVideoReceiver = setting.CommandVideoReceiver;
            this.TimeLastSaved = setting.TimeLastSaved;
            this.CurrentVersion = setting.CurrentVersion;
            this.VolumeTextSpeech = setting.VolumeTextSpeech;
        }
    }
}
