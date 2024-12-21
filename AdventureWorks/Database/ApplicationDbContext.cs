using AdventureWorks.Models.Person;
using Microsoft.EntityFrameworkCore;

namespace AdventureWorks.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Person>()
            .HasMany(p => p.EmailAddresses);


            modelBuilder.Entity<Person>()
                .HasMany(p => p.PersonPhones);

            modelBuilder.Entity<PersonPhone>()
           .HasKey(pp => new { pp.BusinessEntityID, pp.PhoneNumberTypeID });

            modelBuilder.Entity<EmailAddressModel>()
                .HasKey(e => new { e.BusinessEntityID, e.EmailAddressID });
        }

        public DbSet<Person?> Person { get; set; }
        public DbSet<EmailAddressModel> EmailAddress { get; set; }
        public DbSet<PersonPhone> PersonPhone { get; set; }
        public DbSet<PhoneNumberType> PhoneNumberType { get; set; }
    }
}
