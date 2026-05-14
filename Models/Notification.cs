using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Notification
    {
        [Key]
        // Database-generated identity (auto-increment)
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NotificationId { get; set; }

        public int UserId { get; set; }

        // Not stored in the database; used only in application code
        [NotMapped]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Message { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Type { get; set; } = string.Empty;

        [Required]
        public DateTime SentDate { get; set; } = DateTime.Now;

        // Navigation property: related User entity (many-to-one)
        public User? User { get; set; }

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
