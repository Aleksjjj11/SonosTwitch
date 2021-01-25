using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SonosTwitch.Models;

namespace SonosTwitch.Interfaces
{
    public interface IAppSetting
    {
        public string TwitchChannel { get; set; }
        public string Prefix { get; set; }
        public string SpeechCommand { get; set; }
        public int VolumeTextSpeech { get; set; }
        public bool ReceiveFollower { get; set; }
        public bool ReceiveSubscriber { get; set; }
        public bool ReceiveEveryone { get; set; }
        public bool IsGetOffer { get; set; }
        public string TwitchToken { get; set; }
        public uint Timeout { get; set; }
        public string CurrentVersion { get; set; }
        public string CommandVideoReceiver { get; set; }
        public string CommandMusicReceiver { get; set; }
        public string CommandGameReceiver { get; set; }
        public ObservableCollection<Sound> DictionaryCommands { get; set; }
        public DateTime TimeLastSaved { get; set; }
        public void Save();
    }
}
