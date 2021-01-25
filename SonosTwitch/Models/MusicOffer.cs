using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonosTwitch.Models
{
    public class MusicOffer: Item
    {
        public string Username { get; set; }
        public string NameSong { get; set; }

        public MusicOffer(string username, string nameSong)
        {
            Username = username;
            NameSong = nameSong;
        }
    }
}
