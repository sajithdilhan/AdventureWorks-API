using AdventureWorks.Models.Person;

namespace AdventureWorks.Repositories
{
    public interface IPersonRepository
    {
        Task<List<Person>> GetPersonsAsync(PersonQuery query);
        Task<Person?> GetPersonAsync(int id);
        Task<Person> CreatePersonAsync(Person person);
    }
}
