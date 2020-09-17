using SonosTwitch.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SonosTwitch
{
    [Serializable]
    public class AppSetting : INotifyPropertyChanged
    {
        //private Dictionary<string, string> _dictionary;
        private List<Sound> _dictionary;
        public string TwitchChannel { get; set; }
        public string Prefix { get; set; }
        public string SpeechCommand { get; set; }
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
        public bool ReceiveFollower { get; set; }
        public bool ReceiveSubscriber { get; set; }
        public bool ReceiveEveryone { get; set; }
        public bool IsGetOffer { get; set; }
        public string TwitchToken { get; set; }
        public uint Timeout { get; set; }
        public string CurrentVersion { get; set; }
        public List<Sound> DictionaryCommands
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
        
        public AppSetting(List<Sound> dictionary)
        {
            _dictionary = dictionary;
        }
    }
}