using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonosTwitch.Models
{
    public class GameOffer: Item
    {
        private string _username;
        private string _gameName;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string GameName
        {
            get => _gameName;
            set
            {
                _gameName = value;
                OnPropertyChanged(nameof(GameName));
            }
        }

        public GameOffer(string username, string gameName)
        {
            Username = username;
            GameName = gameName;
        }
    }
}
