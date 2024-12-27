using AdventureWorks.Models.Identity;
using AdventureWorks.Models.Person;
using AdventureWorks.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using AdventureWorks.Services;

namespace AdventureWorks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class PersonController : ControllerBase
    {
        private readonly ILogger<PersonController> _logger;
        private readonly IPersonService _personService;

        public PersonController(ILogger<PersonController> logger, IPersonService personService)
        {
            _logger = logger;
            _personService = personService;
        }

        [HttpGet(Name = "GetPerson")]
        public async Task<IActionResult> GetPersonsAsync([FromQuery] PersonQuery query)
        {
            try
            {
                var list = await _personService.GetPersonsAsync(query);
                if (list.Data.Count == 0)
                {
                    return NotFound();
                }

                _logger.LogInformation("Returning {Count} people", list.Data.Count);

                return Ok(list);
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
                var person = await _personService.GetPersonAsync(id, true);
                if (person == null)
                {
                    return NotFound();
                }

                _logger.LogInformation("Returning person with ID {ID}", id);

                return Ok(person);
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
                personDto = await _personService.CreatePersonAsync(personDto);
                _logger.LogInformation("Person created: {} {}", personDto.FirstName, personDto.LastName);
                return CreatedAtRoute("GetPerson", new { id = personDto.BusinessEntityId });
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
                var result = await _personService.DeletePersonAsync(id);
                if (!result)
                {
                    return NotFound();
                }

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
                var result = await _personService.UpdatePersonAsync(id, personDto);
                if (!result)
                {
                    return NotFound();
                }

                _logger.LogInformation("Person with ID {ID} updated", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem(detail: "Internal Server Error", statusCode: 500);
            }
        }
    }
}