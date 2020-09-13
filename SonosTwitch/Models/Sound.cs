namespace SonosTwitch.Models
{
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

        public Sound() { }

        public Sound(string vCommand, string vPathSound)
        {
            command = vCommand;
            pathSound = vPathSound;
        }
        
    }
}