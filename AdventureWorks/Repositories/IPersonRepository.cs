using AdventureWorks.Models.Person;

namespace AdventureWorks.Repositories
{
    public interface IPersonRepository
    {
        Task<List<Person>> GetPersonsAsync();
        Task<Person?> GetPersonAsync(int id);
    }
}
