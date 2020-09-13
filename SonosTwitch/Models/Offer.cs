using System;

namespace SonosTwitch.Models
{
    public class Offer : Item
    {
        private string message;
        public string Message
        {
            get => message;
            set
            {
                message = value;
                OnPropertyChanged("Message");
            }
        }
        private DateTime time;
        public DateTime Time
        {
            get => time;
            set
            {
                time = value;
                OnPropertyChanged("Time");
            }
        }
        private string username;
        public string Username
        {
            get => username;
            set
            {
                username = value;
                OnPropertyChanged("Username");
            }
        }
        private string id;
        public string Id
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }

        public Offer(DateTime time, string message, string username)
        {
            Time = time;
            Message = message;
            Username = username;
            Id = time + message + username;
        }
    }
}