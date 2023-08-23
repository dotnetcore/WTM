using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Core.Models
{
    [Table("WorkflowDefinitions", Schema = "Elsa")]
    public class Elsa_WorkflowDefinition:TopBasePoco
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

        [StringLength(450)]
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public int Version { get; set; }

        public bool IsSingleton { get; set; }

        public int PersistenceBehavior { get; set; }

        public bool DeleteCompletedInstances { get; set; }

        public bool IsPublished { get; set; }
        public bool IsLatest { get; set; }
        [StringLength(450)]
        public string Tag { get; set; }
        public string Data { get; set; }
        [Required]
        public DateTimeOffset CreatedAt { get; set; }
    }
}
