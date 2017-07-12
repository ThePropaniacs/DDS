using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDSDemoDAL
{
    public partial class Employee
    {
        public string FullName
        {
            get
            {
                return this.FirstName + " " + this.LastName;
            }
        }

        [Required]
        public string Email { get; set; }
    }
}
