using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Core.Models
{
    [Table("Bookmarks",Schema = "Elsa")]
    
    public class Elsa_Bookmark:TopBasePoco
    {
        [Key]
        [Column("Id")]
        [StringLength(450)]
        public new string ID { get; set; }

        [StringLength(450)]
        public string TenantId { get; set; }

        [StringLength(450)]
        [Required]
        public string Hash { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        public string ModelType { get; set; }

        [StringLength(450)]
        [Required]
        public string ActivityType { get; set; }

        [StringLength(450)]
        [Required]
        public string ActivityId { get; set; }

        [StringLength(450)]
        [Required]
        public string WorkflowInstanceId { get; set; }

        [StringLength(450)]
        [Required]
        public string CorrelationId { get; set; }

    }
}
