using System.ComponentModel.DataAnnotations.Schema;

namespace EPSC.Model.Domain
{
    public class BaseObject
    {
        public BaseObject()
        {
            CreatedDate = DateTime.UtcNow;
            IsActive = true;
        }
        [Column("last_modified")]
        public DateTime? LastModified { get; set; }
        [Column("modified_by")]
        public string ModifiedBy { get; set; }
        [Column("created_date")]
        public DateTime CreatedDate { get; set; }
        [Column("created_by")]
        public string CreatedBy { get; set; }
        [Column("is_active")]
        public bool IsActive { get; set; }
        [Column("ip_address", TypeName = "nvarchar(50)")]
        public string IPAddress { get; set; }
        [Column("tenant_id", TypeName = "nvarchar(100)")]
        public string TenantId { get; set; }
    }
}
