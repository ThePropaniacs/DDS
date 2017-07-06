namespace DDSDemoDAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblTimeSheetMaster")]
    public partial class TimeSheet
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal ID { get; set; }

        [StringLength(50)]
        public string CompanyName { get; set; }

        [Column(TypeName = "numeric")]
        public decimal EmpID { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? AssocClientID { get; set; }
        
        public DateTime? StartTime { get; set; }
        
        public DateTime? StopTime { get; set; }

        [StringLength(150)]
        public string Note { get; set; }

        public bool? Approved { get; set; }

        public int ApprovedBy { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ApprovedDate { get; set; }

        public bool? Processed { get; set; }

        public virtual Client Client { get; set; }

        public virtual Employee Employee { get; set; }
        
    }
}
