namespace JobTracker.Domain.Common
{
    public abstract class BaseEntity
    {
        public long Id { get; set; }

        public DateTime CreatedAt { get; protected set; }

        public long? CreatedBy { get; protected set; }

        public DateTime? UpdatedAt { get; protected set; }

        public long? UpdatedBy { get; protected set; }


        public virtual void SetCreated(
            DateTime createdAt,
            long? createdBy)
        {
            CreatedAt = createdAt;
            CreatedBy = createdBy;
        }

        public virtual void SetUpdated(
            DateTime updatedAt,
            long? updatedBy)
        {
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }
    }
}