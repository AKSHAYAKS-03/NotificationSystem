using System;

namespace Models
{
    public class Notification
    {
        public string? UserId { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public DateTime SentDate { get; set; }
        public string Type { get; set; }

        public Notification()
        {
            UserName = string.Empty;
            Message = string.Empty;
            Type = string.Empty;
            SentDate = DateTime.Now;
        }

        public Notification(string message, string type)
        {
            UserName = string.Empty;
            Message = message?.Trim() ?? string.Empty;
            Type = type;
            SentDate = DateTime.Now;
        }
    }
}