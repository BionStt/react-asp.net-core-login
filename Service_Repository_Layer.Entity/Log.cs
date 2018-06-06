using Service_Repository_Layer.Enums;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Repository_Layer.Entity
{
    [Table("Log")]
    public class Log : EntityBase
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public ArchLevel ArchLevel { get; set; }
        public string SerializedObject { get; set; }
        public string Method { get; set; }
        public User User { get; set; }
        public string Description { get; set; }
    }
}
