using AdventureWorks.Models.Person;
using System.Linq.Expressions;

namespace AdventureWorks.Repositories
{
    public interface IPersonRepository
    {
        Task<List<Person>> GetPersonsAsync(PersonQuery query);
        Task<int> GetTotalCount(PersonQuery query);
        Task<Person?> GetPersonAsync(Expression<Func<Person, bool>> predicate, bool includeDetails);
        Task<Person> CreatePersonAsync(Person person);
        Task<Person> UpdatePersonAsync(Person person);
        Task<bool> DeletePersonAsync(int id);
        Task<List<Person>> GetPersonFiltered(Expression<Func<Person, bool>> predicate);
    }
}
