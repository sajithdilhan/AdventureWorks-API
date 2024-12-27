using AdventureWorks.Models.Person;

namespace AdventureWorks.Repositories
{
    public interface IPersonRepository
    {
        Task<List<Person>> GetPersonsAsync(PersonQuery query);
        Task<int> GetTotalCount(PersonQuery query);
        Task<Person?> GetPersonAsync(int id, bool includeDetails);
        Task<Person> CreatePersonAsync(Person person);
        Task<Person> UpdatePersonAsync(Person person);
        Task<bool> DeletePersonAsync(int id);
    }
}
