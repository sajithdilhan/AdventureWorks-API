using AdventureWorks.Models.Identity;
using AdventureWorks.Models.Person;
using AdventureWorks.Repositories;
using AdventureWorks.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AdventureWorks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PersonController : ControllerBase
    {
        private readonly ILogger<PersonController> _logger;
        private readonly IPersonRepository _personRepository;

        public PersonController(ILogger<PersonController> logger, IPersonRepository personRepository)
        {
            _logger = logger;
            _personRepository = personRepository;
        }

        [HttpGet(Name = "GetPerson")]
        public async Task<IActionResult> GetPersonsAsync([FromQuery] PersonQuery query)
        {
            try
            {
                var people = await _personRepository.GetPersonsAsync(query);
                if (people.Count == 0)
                {
                    return NotFound();
                }

                _logger.LogInformation("Returning {Count} people", people.Count);

                return Ok(people.ToPersonDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem(detail: "Internal Server Error", statusCode: 500);

            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetPersonByIdAsync([FromRoute] int id)
        {
            try
            {
                var person = await _personRepository.GetPersonAsync(id, false);
                if (person == null)
                {
                    return NotFound();
                }
                _logger.LogInformation("Returning person with ID {ID}", id);

                return Ok(person.ToPersonDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem(detail: "Internal Server Error", statusCode: 500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePersonAsync([FromBody] PersonDto personDto)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            try
            {
                _logger.LogInformation("Creating person: {}", JsonSerializer.Serialize<PersonDto>(personDto));
                var person = personDto.ToPerson();
                await _personRepository.CreatePersonAsync(person);
                _logger.LogInformation("Person created: {} {}", person.FirstName, person.LastName);
                return CreatedAtRoute("GetPerson", new { id = person.BusinessEntityID }, person.ToPersonDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem(detail: "Internal Server Error", statusCode: 500);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = IdentityData.AdminUserRoleName)]
        public async Task<IActionResult> DeletePersonAsync([FromRoute] int id)
        {
            try
            {
                _logger.LogInformation("Deleting person with ID {ID}", id);
                var person = await _personRepository.GetPersonAsync(id, true);
                if (person == null)
                {
                    return NotFound();
                }
                await _personRepository.DeletePersonAsync(person.BusinessEntityID);
                _logger.LogInformation("Person with ID {ID} deleted", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem(detail: "Internal Server Error", statusCode: 500);
            }
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdatePersonAsync([FromRoute] int id, [FromBody] PersonDto personDto)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var person = await _personRepository.GetPersonAsync(id, true);
                if (person == null)
                {
                    return NotFound();
                }
                SetPersonData(personDto, person);
                await _personRepository.UpdatePersonAsync(person);
                _logger.LogInformation("Person with ID {ID} updated", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem(detail: "Internal Server Error", statusCode: 500);
            }
        }

        private static void SetPersonData(PersonDto personDto, Person person)
        {
            person.FirstName = personDto.FirstName;
            person.LastName = personDto.LastName;
            person.MiddleName = personDto.MiddleName;
            person.PersonType = personDto.PersonType;
            person.Title = personDto.Title;
            person.ModifiedDate = DateTime.UtcNow;
            person.EmailPromotion = personDto.EmailPromotion;
            person.Suffix = personDto.Suffix;

            if (personDto.EmailAddresses != null)
            {
                var emailAddresses = personDto.EmailAddresses.Select(e => new EmailAddressModel
                {
                    BusinessEntityID = person.BusinessEntityID,
                    EmailAddress = e,
                    Rowguid = Guid.NewGuid(),
                    ModifiedDate = DateTime.UtcNow
                }).ToList();
                person.EmailAddresses = emailAddresses;
            }

            if (personDto.PhoneNumbers != null)
            {
                var phoneNumbers = personDto.PhoneNumbers.Select(p => new PersonPhone
                {
                    BusinessEntityID = person.BusinessEntityID,
                    PhoneNumber = p,
                    ModifiedDate = DateTime.UtcNow,
                    PhoneNumberType = new PhoneNumberType { ModifiedDate = DateTime.UtcNow, Name = "Home", PhoneNumberTypeID = 2 },
                    PhoneNumberTypeID = 2
                }).ToList();
                person.PersonPhones = phoneNumbers;
            }
        }
    }
}