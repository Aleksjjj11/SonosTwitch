namespace SonosTwitch
{
    public class Sound
    {
        public string Command { get; set; }
        public string PathSound { get; set; }

        public Sound() { }

        public Sound(string command, string pathSound)
        {
            Command = command;
            PathSound = pathSound;
        }
        
    }
}