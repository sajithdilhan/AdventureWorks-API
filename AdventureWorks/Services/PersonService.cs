using AdventureWorks.Models.Person;
using AdventureWorks.Models.Shared;
using AdventureWorks.Repositories;
using AdventureWorks.Utility;

namespace AdventureWorks.Services;

public class PersonService : IPersonService
{
    private readonly IPersonRepository _personRepository;

    public PersonService(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }

    public async Task<PaginatedList<PersonDto>> GetPersonsAsync(PersonQuery query)
    {
        PaginatedList<PersonDto> list = new();
        list.Data = new();
        int total = await _personRepository.GetTotalCount(query);
        var result = await _personRepository.GetPersonsAsync(query);
        list.Data = result.ToPersonDto();

        int totalPages = (int)Math.Ceiling((double)total / query.PageSize);

        list.MetaData = new MetaData(total, query.PageNumber, query.PageSize, totalPages);

        return list;
    }

    public async Task<PersonDto?> GetPersonByIdAsync(int id, bool includeDetails)
    {
        var person = await _personRepository.GetPersonAsync(p => p.BusinessEntityID == id, includeDetails);
        return person?.ToPersonDto();
    }

    public async Task<PersonDto> CreatePersonAsync(PersonDto personDto)
    {
        var person = personDto.ToPerson();
        person = await _personRepository.CreatePersonAsync(person);
        return person.ToPersonDto();
    }

    public async Task<bool> UpdatePersonAsync(int id, PersonDto personDto)
    {
        var person = await _personRepository.GetPersonAsync(p => p.BusinessEntityID == id, true);
        if (person == null)
        {
            return false;
        }

        SetPersonData(personDto, person);
        await _personRepository.UpdatePersonAsync(person);
        return true;
    }

    public async Task<bool> DeletePersonAsync(int id)
    {
        var person = await _personRepository.GetPersonAsync(p => p.BusinessEntityID == id, true);
        if (person == null)
        {
            return false;
        }

        await _personRepository.DeletePersonAsync(id);
        return true;
    }

    private static void SetPersonData(PersonDto personDto, Person person)
    {
        person.FirstName = personDto.FirstName;
        person.LastName = personDto.LastName;
        person.MiddleName = personDto.MiddleName;
        person.PersonType = personDto.PersonType;
        person.Title = personDto.Title;
        person.ModifiedDate = DateTime.UtcNow;
        person.EmailPromotion = personDto.EmailPromotion;
        person.Suffix = personDto.Suffix;

        if (personDto.EmailAddresses.Any())
        {
            var emailAddresses = personDto.EmailAddresses.Select(e => new EmailAddressModel
            {
                BusinessEntityID = person.BusinessEntityID,
                EmailAddress = e,
                Rowguid = Guid.NewGuid(),
                ModifiedDate = DateTime.UtcNow
            }).ToList();
            person.EmailAddresses = emailAddresses;
        }

        if (personDto.PhoneNumbers.Any())
        {
            var phoneNumbers = personDto.PhoneNumbers.Select(p => new PersonPhone
            {
                BusinessEntityID = person.BusinessEntityID,
                PhoneNumber = p,
                ModifiedDate = DateTime.UtcNow,
                PhoneNumberType = new PhoneNumberType
                    { ModifiedDate = DateTime.UtcNow, Name = "Home", PhoneNumberTypeID = 2 },
                PhoneNumberTypeID = 2
            }).ToList();
            person.PersonPhones = phoneNumbers;
        }
    }
}