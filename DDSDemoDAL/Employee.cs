namespace DDSDemoDAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblEmployeeMaster")]
    public partial class Employee
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Employee()
        {
            TimeSheets = new HashSet<TimeSheet>();
        }

        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal ID { get; set; }


        [StringLength(50)]
        public string CompanyName { get; set; }

        public int EmpID { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        public bool? Sun { get; set; }

        public bool? Mon { get; set; }

        public bool? Tue { get; set; }

        public bool? Wed { get; set; }

        public bool? Thu { get; set; }

        public bool? Fri { get; set; }

        public bool? Sat { get; set; }

        [StringLength(50)]
        public string AvailNotes { get; set; }

        [DataType(DataType.Date)]
        public DateTime? AvailStart { get; set; }

        [DataType(DataType.Date)]
        public DateTime? AvailExpires { get; set; }

        public int? AvailDuration { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TimeSheet> TimeSheets { get; set; }

        //public virtual List<>
    }
}
