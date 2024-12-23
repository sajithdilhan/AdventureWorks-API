using AdventureWorks.Models.Person;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using AdventureWorks.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

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

            modelBuilder.Entity<EmailAddressModel>(entity =>
            {
                entity.Property(e => e.EmailAddressID)
                      .ValueGeneratedOnAdd(); 
            });
        }

        public DbSet<Person> Person { get; set; }
        public DbSet<EmailAddressModel> EmailAddress { get; set; }
        public DbSet<PersonPhone> PersonPhone { get; set; }
        public DbSet<PhoneNumberType> PhoneNumberType { get; set; }
    }
}
