using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace FeedbackSystemAPI.Models
{
    public partial class AssignTask
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string AssignId { get; set; }
        public string TaskId { get; set; }
        public string EmployeeId { get; set; }
        public string IsDelete { get; set; }
        public virtual User Employee { get; set; }
        public virtual Task Task { get; set; }
    }
}
