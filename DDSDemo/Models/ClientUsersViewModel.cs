using DDSDemoDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DDSDemo.Models
{
    public class ClientUsersViewModel
    {
        public Client Client { get; set; }
        public IEnumerable<ApplicationUser> Users { get; set; }
    }
}