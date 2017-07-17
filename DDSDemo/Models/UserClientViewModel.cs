using DDSDemoDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DDSDemo.Models
{
    public class UserClientViewModel
    {
        public Client Client { get; set; }
        public ApplicationUser User { get; set; }
    }
}