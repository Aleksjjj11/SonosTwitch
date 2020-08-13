using System;

namespace SonosTwitch
{
    public class Offer
    {
        public string Message { get; set; }
        public DateTime Time { get; set; }
        public string Username { get; set; }
        public string Id { get; set; }

        public Offer(DateTime time, string message, string username)
        {
            Time = time;
            Message = message;
            Username = username;
            Id = time + message + username;
        }
    }
}