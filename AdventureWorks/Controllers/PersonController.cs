using AdventureWorks.Models.Person;
using AdventureWorks.Repositories;
using AdventureWorks.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
                var person = await _personRepository.GetPersonAsync(id);
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
                var person = personDto.ToPerson();
                await _personRepository.CreatePersonAsync(person);
                return CreatedAtRoute("GetPerson", new { id = person.BusinessEntityID }, person.ToPersonDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem(detail: "Internal Server Error", statusCode: 500);
            }
        }
    }
}
