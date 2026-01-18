using AdventureWorks.Database;
using AdventureWorks.Models.Person;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AdventureWorks.Repositories;

public class PersonRepository(ILogger<PersonRepository> logger, ApplicationDbContext applicationDbContext) : IPersonRepository
{
    public async Task<int> GetTotalCount(PersonQuery query)
    {
        return await applicationDbContext.Persons.AsNoTracking().CountAsync();
    }

    public async Task<Person?> GetPersonAsync(Expression<Func<Person, bool>> predicate, bool includeDetails)
    {
        try
        {
            var query = applicationDbContext.Persons.AsNoTracking();
            if (includeDetails)
            {
                query = query
                    .Include(p => p.EmailAddresses)
                    .Include(p => p.PersonPhones)
                    .AsSplitQuery();
            }

            return await query.FirstOrDefaultAsync(predicate);
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to get people from database. {}", ex.Message);
            throw;
        }
    }

    public async Task<List<Person>> GetPersonsAsync(PersonQuery queryParam)
    {
        try
        {
            var query = applicationDbContext.Persons
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

            return await query.Select(p => new Person 
            { 
                BusinessEntityID = p.BusinessEntityID, 
                EmailAddresses = p.EmailAddresses, 
                FirstName = p.FirstName,
                LastName = p.LastName,
                PersonPhones = p.PersonPhones,
                Gender = p.Gender == null ? null : p.Gender.Trim(),
                MiddleName = p.MiddleName,
                Title = p.Title,
                PersonType = p.PersonType,
                AdditionalContactInfo = p.AdditionalContactInfo,
                Suffix = p.Suffix,
                EmailPromotion = p.EmailPromotion,
                ModifiedDate = p.ModifiedDate,
                Demographics = p.Demographics,
                NameStyle = p.NameStyle,
                Rowguid = p.Rowguid

            }).ToListAsync();
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to get people from database. {}", ex.Message);
            throw;
        }
    }

    public async Task<Person> CreatePersonAsync(Person person)
    {
        await applicationDbContext.Persons.AddAsync(person);
        await applicationDbContext.SaveChangesAsync();
        return person;
    }

    public async Task<bool> DeletePersonAsync(int id)
    {
        var person = await GetPersonAsync(x => x.BusinessEntityID == id, false);
        if (person == null)
        {
            return false;
        }

        applicationDbContext.Persons.Remove(person);
        int result = await applicationDbContext.SaveChangesAsync();
        if (result > 0)
        {
            return true;
        }

        return false;
    }

    public async Task<Person> UpdatePersonAsync(Person person)
    {
        applicationDbContext.Persons.Update(person);
        await applicationDbContext.SaveChangesAsync();
        return person;
    }

    public async Task<List<Person>> GetPersonFiltered(Expression<Func<Person, bool>> predicate)
    {
        return await applicationDbContext.Persons.Where(predicate).ToListAsync();
    }
}