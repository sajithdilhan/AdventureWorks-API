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
              return await  _dbcontext.Person
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

        public async Task<List<Person>> GetPersonsAsync()
        {
            try
            {
                return await _dbcontext.Person
                    .AsNoTracking()
                    .Include(p => p.EmailAddresses)
                    .Include(p => p.PersonPhones)
                    .AsSplitQuery().Take(50)
                    .ToListAsync();
            }
            catch (Exception)
            {
                _logger.LogError("Failed to get people from database");
                throw;
            }
        }
    }
}
