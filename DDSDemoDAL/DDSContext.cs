namespace DDSDemoDAL
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

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
            modelBuilder.Entity<Client>()
                .Property(e => e.ID)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Client>()
                .Property(e => e.CompanyName)
                .IsUnicode(false);

            modelBuilder.Entity<Client>()
                .Property(e => e.EmployerName)
                .IsUnicode(false);

            modelBuilder.Entity<Client>()
                .Property(e => e.Address1)
                .IsUnicode(false);

            modelBuilder.Entity<Client>()
                .Property(e => e.Address2)
                .IsUnicode(false);

            modelBuilder.Entity<Client>()
                .Property(e => e.City)
                .IsUnicode(false);

            modelBuilder.Entity<Client>()
                .Property(e => e.State)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Client>()
                .Property(e => e.Zip)
                .IsUnicode(false);

            modelBuilder.Entity<Client>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<Client>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<Client>()
                .HasMany(e => e.TimeSheets)
                .WithOptional(e => e.Client)
                .HasForeignKey(e => e.AssocClientID);

            modelBuilder.Entity<Employee>()
                .Property(e => e.ID)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Employee>()
                .Property(e => e.CompanyName)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.FirstName)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.LastName)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.AvailNotes)
                .IsUnicode(false);

            //modelBuilder.Entity<Employee>()
            //    .Ignore(e => e.Email);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.TimeSheets)
                .WithRequired(e => e.Employee)
                .HasForeignKey(e => e.EmpID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Placement>()
                .Property(e => e.ID)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Placement>()
                .Property(e => e.CompanyName)
                .IsUnicode(false);

            modelBuilder.Entity<Placement>()
                .Property(e => e.Position)
                .IsUnicode(false);

            modelBuilder.Entity<TimeSheet>()
                .Property(e => e.ID)
                .HasPrecision(18, 0);

            modelBuilder.Entity<TimeSheet>()
                .Property(e => e.CompanyName)
                .IsUnicode(false);

            modelBuilder.Entity<TimeSheet>()
                .Property(e => e.EmpID)
                .HasPrecision(18, 0);
               

            modelBuilder.Entity<TimeSheet>()
                .Property(e => e.AssocClientID)
                .HasPrecision(18, 0);

            modelBuilder.Entity<TimeSheet>()
                .Property(e => e.Note)
                .IsUnicode(false);
        }
    }
}
