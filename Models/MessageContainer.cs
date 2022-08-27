using System;

namespace Hello_Travellers.Models
{
    public class MessageContainer
    {
        public string SenderUsername { set; get; }
        public string SenderName { set; get; }
        public string Content { set; get; }
        public System.DateTime SentTime { set; get; }
    }
}