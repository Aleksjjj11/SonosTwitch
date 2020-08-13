﻿using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SonosTwitch
{
    [Serializable]
    public class AppSetting : INotifyPropertyChanged
    {
        private Dictionary<string, string> _dictionary;
        public string TwitchChannel { get; set; }
        public string Prefix { get; set; }
        public bool ReceiveFollower { get; set; }
        public bool ReceiveSubscriber { get; set; }
        public bool ReceiveEveryone { get; set; }
        public bool IsGetOffer { get; set; }
        public string TwitchToken { get; set; }
        public uint Timeout { get; set; }
        public string CurrentVersion { get; set; }
        public Dictionary<string, string> DictionaryCommands
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
        
        public AppSetting(Dictionary<string,string> dictionary)
        {
            _dictionary = dictionary;
        }
    }
}