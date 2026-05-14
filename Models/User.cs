using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public partial class User
    {
        //encapsulation - no direct access to fields, only through properties
        private string _name = string.Empty;
        private string _email = string.Empty;
        private string _phone = string.Empty;

        [Key]
        // Database-generated identity (auto-increment)
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name
        {
            get{return _name;}
            set{_name = value?.Trim() ?? string.Empty;}
        }

        [Required]
        [StringLength(150)]
        [EmailAddress]
        public string Email
        {
            get{return _email;}
            set{_email = value?.Trim().ToLowerInvariant() ?? string.Empty;}
        }

        [Required]
        [StringLength(20)]
        [Phone]
        public string Phone
        {
            get{return _phone;}
            set{_phone = value?.Trim() ?? string.Empty;}
        }

        // Navigation property: user's notifications (one-to-many). InverseProperty links to Notification.User
        [InverseProperty("User")]
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
