using System.ComponentModel.DataAnnotations;

namespace AdventureWorks.Models.Person;

public class PersonDto
{
    public int BusinessEntityId { get; set; }
    [Required]
    public string PersonType { get; set; } = string.Empty;
    [StringLength(8, MinimumLength = 1)]
    public string? Title { get; set; }
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string FirstName { get; set; } = string.Empty;
    [StringLength(50)]
    public string? MiddleName { get; set; }
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string LastName { get; set; } = string.Empty;
    [StringLength(10, MinimumLength = 1)]
    public string? Suffix { get; set; }
    [Required]
    public int EmailPromotion { get; set; }

    public string? AdditionalContactInfo { get; set; } 

    public string? Demographics { get; set; } 

    public DateTime ModifiedDate { get; set; }

    public List<string> EmailAddresses { get; set; } = new List<string>();

    public List<string> PhoneNumbers { get; set; } = new List<string>();
}