using System;

namespace SonosTwitch.Models
{
    [Serializable]
    public class Sound : Item
    {
        private string command;
        public string Command
        {
            get => command;
            set
            {
                command = value;
                OnPropertyChanged("Command");
            }
        }
        private string pathSound;
        public string PathSound 
        { 
            get => pathSound; 
            set
            {
                pathSound = value;
                OnPropertyChanged("PathSound");
            }
        }

        private double volume;
        public double Volume
        {
            get => volume;
            set
            {
                volume = value;
                OnPropertyChanged(nameof(Volume));
            }
        }

        public Sound() { }

        public Sound(string vCommand, string vPathSound)
        {
            Command = vCommand;
            PathSound = vPathSound;
            Volume = 0.5;
        }
        
    }
}