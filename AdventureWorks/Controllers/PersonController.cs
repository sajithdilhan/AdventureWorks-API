using AdventureWorks.Models.Identity;
using AdventureWorks.Models.Person;
using AdventureWorks.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace AdventureWorks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService personService;
        private readonly ILogger<PersonController> logger;
        private static readonly ActivitySource ActivitySource =
    new("AdventureWorks");

        public PersonController(IPersonService personService, ILogger<PersonController> logger)
        {
            this.personService = personService;
            this.logger = logger;
        }


        [HttpGet(Name = "GetPerson")]
        public async Task<IActionResult> GetPersonsAsync([FromQuery] PersonQuery query)
        {
            try
            {
                var list = await personService.GetPersonsAsync(query);
                if (list.Data.Count == 0)
                {
                    return NotFound();
                }

                logger.LogInformation("Returning {Count} people", list.Data.Count);

                return Ok(list);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return Problem(detail: "Internal Server Error", statusCode: 500);
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetPersonByIdAsync([FromRoute] int id)
        {
            try
            {
                using var activity = ActivitySource.StartActivity("GetPersonById");

                activity?.SetTag("person.id", id);

                var person = await personService.GetPersonByIdAsync(id, true);
                if (person == null)
                {
                    return NotFound();
                }

                logger.LogInformation("Returning person with ID {ID}", id);

                return Ok(person);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return Problem(detail: "Internal Server Error", statusCode: 500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePersonAsync([FromBody] PersonDto personDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                logger.LogInformation("Creating person: {}", JsonSerializer.Serialize<PersonDto>(personDto));
                personDto = await personService.CreatePersonAsync(personDto);
                logger.LogInformation("Person created: {} {}", personDto.FirstName, personDto.LastName);
                return CreatedAtRoute("GetPerson", new { id = personDto.BusinessEntityId });
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
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
                logger.LogInformation("Deleting person with ID {ID}", id);
                var result = await personService.DeletePersonAsync(id);
                if (!result)
                {
                    return NotFound();
                }

                logger.LogInformation("Person with ID {ID} deleted", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return Problem(detail: "Internal Server Error", statusCode: 500);
            }
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdatePersonAsync([FromRoute] int id, [FromBody] PersonDto personDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await personService.UpdatePersonAsync(id, personDto);
                if (!result)
                {
                    return NotFound();
                }

                logger.LogInformation("Person with ID {ID} updated", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return Problem(detail: "Internal Server Error", statusCode: 500);
            }
        }
    }
}