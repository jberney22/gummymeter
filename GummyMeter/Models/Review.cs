using System.ComponentModel.DataAnnotations;

namespace GummyMeter.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string MovieId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        [Range(1, 5)]
        public int Score { get; set; }        // 1–5 stars
        public bool IsCritic { get; set; } = false;
        public string? Subject { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; }
    }
}
