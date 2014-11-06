using System;

namespace API.Models
{
    public class CardEvent
    {
        public DateTime Time { get; set; }
        public string Username { get; set; }
        public int UserId { get; set; }
        public string CardId { get; set; }
        public Status Status { get; set; }
    }

    
}