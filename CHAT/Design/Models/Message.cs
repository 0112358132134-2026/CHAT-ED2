using System;

namespace Design.Models
{
    public class Message
    {
        public string _id { get; set; }
        public string emisor { get; set; }
        public string receptor { get; set; }
        public string message { get; set; }
        public DateTime date { get; set; }
        public string type { get; set; }
    }
}