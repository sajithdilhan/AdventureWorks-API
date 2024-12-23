using AdventureWorks.Database;
using AdventureWorks.Models.Person;
using Microsoft.EntityFrameworkCore;

namespace AdventureWorks.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly ILogger<PersonRepository> _logger;

        public PersonRepository(ApplicationDbContext applicationDbContext, ILogger<PersonRepository> logger)
        {
            _dbcontext = applicationDbContext;
            _logger = logger;
        }

        public async Task<Person?> GetPersonAsync(int id)
        {
            try
            {
                return await _dbcontext.Person
                     .AsNoTracking()
                     .Include(p => p.EmailAddresses)
                     .Include(p => p.PersonPhones)
                     .AsSplitQuery()
                     .FirstOrDefaultAsync(p => p.BusinessEntityID == id);
            }
            catch (Exception)
            {
                _logger.LogError($"Failed to get people from database with ID {id}");
                throw;
            }
        }

        public async Task<List<Person>> GetPersonsAsync(PersonQuery queryParam)
        {
            try
            {
                var query =  _dbcontext.Person
                     .AsNoTrackingWithIdentityResolution()
                     .Include(p => p.EmailAddresses)
                     .Include(p => p.PersonPhones)
                     .AsSplitQuery();

                if (queryParam?.SortBy == "FirstName" )
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
            catch (Exception)
            {
                _logger.LogError("Failed to get people from database");
                throw;
            }
        }

        async Task<Person> IPersonRepository.CreatePersonAsync(Person person)
        {
            await _dbcontext.Person.AddAsync(person);
            await _dbcontext.SaveChangesAsync();
            return person;
        }
    }
}
