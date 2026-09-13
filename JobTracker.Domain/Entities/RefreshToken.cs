using JobTracker.Domain.Common;

namespace JobTracker.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public long UserId { get; set; }
        public string TokenHash { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string? ReplacedByTokenHash { get; set; }
        public User User { get; set; } = null!;
    }
}