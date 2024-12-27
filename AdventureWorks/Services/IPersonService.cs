using AdventureWorks.Models.Person;
using AdventureWorks.Models.Shared;

namespace AdventureWorks.Services;

public interface IPersonService
{
    Task<PaginatedList<PersonDto>> GetPersonsAsync(PersonQuery query);
    Task<PersonDto?> GetPersonAsync(int id, bool includeDetails);
    Task<PersonDto> CreatePersonAsync(PersonDto person);
    Task<bool> UpdatePersonAsync(int id, PersonDto person);
    Task<bool> DeletePersonAsync(int id);
}