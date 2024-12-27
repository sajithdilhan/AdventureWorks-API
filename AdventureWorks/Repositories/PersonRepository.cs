using AdventureWorks.Database;
using AdventureWorks.Models.Person;
using Microsoft.EntityFrameworkCore;

namespace AdventureWorks.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly ILogger<PersonRepository> _logger;

        public PersonRepository(ILogger<PersonRepository> logger, ApplicationDbContext applicationDbContext)
        {
            _dbcontext = applicationDbContext;
            _logger = logger;
        }

        public async Task<int> GetTotalCount(PersonQuery query)
        {
            return await _dbcontext.Persons.AsNoTracking().CountAsync();
        }

        public async Task<Person?> GetPersonAsync(int id, bool includeDetails)
        {
            try
            {
                var query = _dbcontext.Persons.AsNoTracking();
                if (includeDetails)
                {
                    query = query
                        .Include(p => p.EmailAddresses)
                        .Include(p => p.PersonPhones)
                        .AsSplitQuery();
                }

                return await query.FirstOrDefaultAsync(p => p.BusinessEntityID == id);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get people from database with ID {}. {}", id, ex.Message);
                throw;
            }
        }

        public async Task<List<Person>> GetPersonsAsync(PersonQuery queryParam)
        {
            try
            {
                var query = _dbcontext.Persons
                    .AsNoTrackingWithIdentityResolution()
                    .Include(p => p.EmailAddresses)
                    .Include(p => p.PersonPhones)
                    .AsSplitQuery();

                if (queryParam?.SortBy == "FirstName")
                {
                    if (queryParam?.SortOrder == "desc")
                    {
                        query = query.OrderByDescending(p => p.FirstName);
                    }
                    else
                    {
                        query = query.OrderBy(p => p.FirstName);
                    }
                }
                else if (queryParam?.SortBy == "LastName")
                {
                    if (queryParam?.SortOrder == "desc")
                    {
                        query = query.OrderByDescending(p => p.FirstName);
                    }
                    else
                    {
                        query = query.OrderBy(p => p.FirstName);
                    }
                }
                else
                {
                    query = query.OrderBy(p => p.BusinessEntityID);
                }

                if (queryParam?.PageSize > 0)
                {
                    query = query.Skip(queryParam.PageNumber * queryParam.PageSize).Take(queryParam.PageSize);
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get people from database. {}", ex.Message);
                throw;
            }
        }

        async Task<Person> IPersonRepository.CreatePersonAsync(Person person)
        {
            await _dbcontext.Persons.AddAsync(person);
            await _dbcontext.SaveChangesAsync();
            return person;
        }

        public async Task<bool> DeletePersonAsync(int id)
        {
            var person = await GetPersonAsync(id, true);
            if (person == null)
            {
                return false;
            }

            _dbcontext.Persons.Remove(person);
            int result = await _dbcontext.SaveChangesAsync();
            if (result > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<Person> UpdatePersonAsync(Person person)
        {
            _dbcontext.Persons.Update(person);
            await _dbcontext.SaveChangesAsync();
            return person;
        }
    }
}