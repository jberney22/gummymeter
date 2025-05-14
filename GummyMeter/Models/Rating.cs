using System.ComponentModel.DataAnnotations;

namespace GummyMeter.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int UserId { get; set; }
        [Range(1, 5)]
        public int Score { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
    }
}
