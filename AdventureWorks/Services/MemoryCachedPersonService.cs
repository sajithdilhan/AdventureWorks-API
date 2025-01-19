using AdventureWorks.Models.Person;
using AdventureWorks.Models.Shared;
using Microsoft.Extensions.Caching.Memory;

namespace AdventureWorks.Services
{
    public class MemoryCachedPersonService(PersonService personService, IMemoryCache memoryCache) : IPersonService
    {
        public async Task<PersonDto> CreatePersonAsync(PersonDto person)
        {
            return await personService.CreatePersonAsync(person);
        }

        public async Task<bool> DeletePersonAsync(int id)
        {
            return await personService.DeletePersonAsync(id);
        }

        public Task<PersonDto?> GetPersonByIdAsync(int id, bool includeDetails)
        {
            string key = $"person-{id}";

            return  memoryCache.GetOrCreateAsync(key, async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
                return await personService.GetPersonByIdAsync(id, includeDetails);

            });
        }

        public async Task<PaginatedList<PersonDto>> GetPersonsAsync(PersonQuery query)
        {
            return await personService.GetPersonsAsync(query);
        }

        public async Task<bool> UpdatePersonAsync(int id, PersonDto person)
        {
            return await personService.UpdatePersonAsync(id, person);
        }
    }
}
