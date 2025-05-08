using System.ComponentModel.DataAnnotations;

namespace GummyMeter.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public string Email { get;  set; } = string.Empty;
    }

    public class Favorite
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string MovieId { get; set; } = string.Empty;
        public string MovieTitle { get; set; } = string.Empty;
        public DateTime FavoritedAt { get; set; } = DateTime.UtcNow;
        public bool Watched { get; set; } = false;
        public int? Rating { get; set; }

        public User User { get; set; }
    }
}
