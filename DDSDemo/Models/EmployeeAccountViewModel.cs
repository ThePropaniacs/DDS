using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DDSDemo.Models
{
    public class EmployeeAccountViewModel
    {
        [StringLength(50)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Required]
        public string Email { get; set; }

        public int Id { get; set; }

        public int EmployeeId { get; set; }

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

        public DateTime? AvailStart { get; set; }

        public DateTime? AvailExpires { get; set; }

        public int? AvailDuration { get; set; }
    }
}