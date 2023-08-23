using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Core.Models
{
    [Table("WorkflowExecutionLogRecords", Schema = "Elsa")]
    public class Elsa_WorkflowExecutionLogRecord : TopBasePoco
    {
        [Key]
        [Column("Id")]
        [StringLength(450)]
        public new string ID { get; set; }

        [StringLength(450)]
        public string TenantId { get; set; }

        [StringLength(450)]
        [Required]
        public string WorkflowInstanceId { get; set; }
        [StringLength(450)]
        [Required]
        public string ActivityType { get; set; }

        [StringLength(450)]
        [Required]
        public string ActivityId { get; set; }

        [Required]
        public DateTimeOffset Timestamp { get; set; }

        public string EventName { get; set; }

        public string Message { get; set; }
        public string Source { get; set; }

        public string Data { get; set; }


    }
}
