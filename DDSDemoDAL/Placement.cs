namespace DDSDemoDAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Placement
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal ID { get; set; }

        [StringLength(50)]
        public string CompanyName { get; set; }

        public int? PlacementID { get; set; }

        public int? ClientID { get; set; }

        public int? EmployeeID { get; set; }

        [Column(TypeName = "date")]
        public DateTime? PlacementDate { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        [StringLength(50)]
        public string Position { get; set; }
    }
}
