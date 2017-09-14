namespace DDSDemoDAL
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Data.Entity.Infrastructure.Annotations;

    public partial class DDSContext : DbContext
    {
        public DDSContext()
            : base("name=DDSContext")
        {
        }

        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Placement> Placements { get; set; }
        public virtual DbSet<TimeSheet> TimeSheets { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            
        }
    }
}
