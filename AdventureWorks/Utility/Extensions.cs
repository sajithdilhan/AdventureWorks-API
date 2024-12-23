using AdventureWorks.Models.Person;

namespace AdventureWorks.Utility;

public static class Extensions
{
    public static PersonDto ToPersonDto(this Person person)
    {
        return new PersonDto()
        {
            BusinessEntityId = person.BusinessEntityID,
            Title = person.Title,
            AdditionalContactInfo = person.AdditionalContactInfo,
            Demographics = person.Demographics,
            EmailPromotion = person.EmailPromotion,
            FirstName = person.FirstName,
            LastName = person.LastName,
            MiddleName = person.MiddleName,
            ModifiedDate = person.ModifiedDate,
            PersonType = person.PersonType,
            Suffix = person.Suffix,
            EmailAddresses = person.EmailAddresses.Any() ? 
                person.EmailAddresses.Select(e => e.EmailAddress).ToList() : [],
            PhoneNumbers = person.PersonPhones.Any() ?
                person.PersonPhones.Select(p => p.PhoneNumber).ToList() : []
        };
    }
    
    public static List<PersonDto> ToPersonDto(this List<Person> people)
    {
        return people.Select(p => p.ToPersonDto()).ToList();
    }

    public static Person ToPerson(this PersonDto personDto)
    {
        return new Person()
        {
            BusinessEntityID = personDto.BusinessEntityId,
            Title = personDto.Title,
            AdditionalContactInfo = personDto.AdditionalContactInfo,
            Demographics = personDto.Demographics,
            EmailPromotion = personDto.EmailPromotion,
            FirstName = personDto.FirstName,
            LastName = personDto.LastName,
            MiddleName = personDto.MiddleName,
            PersonType = personDto.PersonType,
            Suffix = personDto.Suffix,
            EmailAddresses = personDto.EmailAddresses.Select(e => new EmailAddressModel()
            { EmailAddress = e, BusinessEntityID = personDto.BusinessEntityId, ModifiedDate = DateTime.Now }).ToList(),
            PersonPhones = personDto.PhoneNumbers.Select(p => new PersonPhone()
            { PhoneNumber = p, BusinessEntityID = personDto.BusinessEntityId, ModifiedDate = DateTime.Now, PhoneNumberTypeID =1 }).ToList(),
            NameStyle = false,
            ModifiedDate = DateTime.Now,
        };
    }
}