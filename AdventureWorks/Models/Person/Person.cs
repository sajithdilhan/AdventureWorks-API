using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdventureWorks.Models.Person;

[Table("Person", Schema = "Person")] 
public class Person
{
    [Key]
    public int BusinessEntityID { get; set; }

    [Column(TypeName = "nchar(2)")] // Specify SQL type
    public string PersonType { get; set; } = string.Empty;

    [Column(TypeName = "bit")] // Correctly map NameStyle
    public bool NameStyle { get; set; }

    [MaxLength(8)]
    public string? Title { get; set; } // Nullable as per SQL definition

    [Required]
    [MaxLength(50)] // Use [dbo].[Name] equivalent max length
    public string FirstName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? MiddleName { get; set; } // Nullable as per SQL definition

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(10)]
    public string? Suffix { get; set; } // Nullable as per SQL definition

    [MaxLength(10)]
    public string? Gender { get; set; } // Nullable as per SQL definition

    public int EmailPromotion { get; set; }

    public string? AdditionalContactInfo { get; set; } // Nullable for XML

    public string? Demographics { get; set; } // Nullable for XML

    [Column(name: "rowguid")]
    public Guid Rowguid { get; set; }

    public DateTime ModifiedDate { get; set; }

    // Navigation properties
    public virtual ICollection<EmailAddressModel> EmailAddresses { get; set; } = new List<EmailAddressModel>();
    public virtual ICollection<PersonPhone> PersonPhones { get; set; } = new List<PersonPhone>();
}

[Table("EmailAddress", Schema = "Person")]
public class EmailAddressModel
{
    [Key]
    public int EmailAddressID { get; set; }
    [Key]
    public int BusinessEntityID { get; set; }
    [Column(TypeName = "nvarchar(50)")]
    public string EmailAddress { get; set; } = string.Empty;
    [Column(name: "rowguid")]
    public Guid Rowguid { get; set; }
    public DateTime ModifiedDate { get; set; }
}

[Table("PersonPhone", Schema = "Person")]
public class PersonPhone
{
    [Key]
    public int BusinessEntityID { get; set; }
    [Key]
    public int PhoneNumberTypeID { get; set; }
    [Key]
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime ModifiedDate { get; set; }

    [ForeignKey("PhoneNumberTypeID")]
    public virtual PhoneNumberType? PhoneNumberType { get; set; } = null!; 
}

[Table("PhoneNumberType", Schema = "Person")]
public class PhoneNumberType
{
    [Key]
    public int PhoneNumberTypeID { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime ModifiedDate { get; set; }
}