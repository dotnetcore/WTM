using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Core.Models
{
    [Table("WorkflowInstances", Schema = "Elsa")]
    public class Elsa_WorkflowInstance:TopBasePoco
    {
        [Key]
        [Column("Id")]
        [StringLength(450)]
        public new string ID { get; set; }

        [Required]
        [StringLength(450)]
        public string DefinitionId { get; set; }

        [StringLength(450)]
        public string TenantId { get; set; }

        public int Version { get; set; }

        public int WorkflowStatus { get; set; }

        [StringLength(450)]
        [Required]
        public string CorrelationId { get; set; }

        [StringLength(450)]
        public string ContextType { get; set; }

        [StringLength(450)]
        public string ContextId { get; set; }

        [StringLength(450)]
        public string Name { get; set; }

        [Required]
        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset LastExecutedAt { get; set; }

        public DateTimeOffset FinishedAt { get; set; }
        public DateTimeOffset CancelledAt { get; set; }
        public DateTimeOffset FaultedAt { get; set; }
        public string Data { get; set; }
        public string LastExecutedActivityId { get; set; }
        [StringLength(450)]
        [Required]
        public string DefinitionVersionId { get; set; }

    }
}
