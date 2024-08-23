using System.ComponentModel.DataAnnotations;

namespace RemindMe.Models
{
    public class Subscription
    {
        public int Id { get; set; }

        [Required]
        public string Endpoint { get; set; }

        [Required]
        public string P256dh { get; set; }

        [Required]
        public string Auth { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
