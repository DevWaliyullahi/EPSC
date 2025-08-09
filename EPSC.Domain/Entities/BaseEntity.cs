namespace EPSC.Domain.Entities
{
    public abstract class BaseEntity
    {
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid CreatedBy { get; set; } 
        public Guid? UpdatedBy { get; set; } = null;
        public Guid? DeletedBy { get; set; } = null;

    }
}
