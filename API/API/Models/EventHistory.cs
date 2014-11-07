using System;

namespace API.Models
{
    public class HistoryEvent
    {
        public DateTime Time { get; set; }
        public string Username { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string AvatarUrl { get; set; }
        public string Type { get; set; }
    }
    

}