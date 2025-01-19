using AdventureWorks.Models.Identity;
using AdventureWorks.Models.Person;
using Microsoft.EntityFrameworkCore;

namespace AdventureWorks.Database;

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

        modelBuilder.Entity<Person>()
    .ToTable(tb => tb.UseSqlOutputClause(false));
    }

    public DbSet<Person> Persons { get; set; }
    public DbSet<EmailAddressModel> EmailAddresses { get; set; }
    public DbSet<PersonPhone> PersonPhones { get; set; }
    public DbSet<PhoneNumberType> PhoneNumberTypes { get; set; }
    public DbSet<User> AppUsers { get; set; }
}