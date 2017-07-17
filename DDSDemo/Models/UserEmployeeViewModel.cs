using DDSDemoDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DDSDemo.Models
{
    public class UserEmployeeViewModel
    {
        public Employee Employee { get; set; }
        public ApplicationUser User { get; set; }
    }
}