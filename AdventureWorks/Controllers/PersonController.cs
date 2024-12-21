using AdventureWorks.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AdventureWorks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<IActionResult> GetPersonsAsync()
        {
            try
            {
                var people = await _personRepository.GetPersonsAsync();
                if (people.Count == 0)
                {
                    return NotFound();
                }

                _logger.LogInformation("Returning {Count} people", people.Count);

                return Ok(people);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem(detail: "Internal Server Error", statusCode: 500);

            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetPersonByIdAsync(int id)
        {
            try
            {
                var person = await _personRepository.GetPersonAsync(id);
                if (person == null)
                {
                    return NotFound();
                }
                _logger.LogInformation("Returning person with ID {ID}", id);
                //var serialized = JsonSerializer.Serialize(person);
                return Ok(person);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem(detail: "Internal Server Error", statusCode: 500);
            }
        }
    }
}
