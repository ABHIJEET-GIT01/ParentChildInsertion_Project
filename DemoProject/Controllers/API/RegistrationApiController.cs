using Microsoft.AspNetCore.Mvc;
using DemoProject.Models;
using DemoProject.Services.Implimentation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoProject.Controllers
{
    [ApiController]
    [Route("api/home/[action]")]
    public class RegistrationApiController : ControllerBase
    {
        private readonly ILogger<RegistrationApiController> _logger;
        private readonly IRegistrationServices _registrationServices;
        private const int PAGE_SIZE = 5;

        public RegistrationApiController(ILogger<RegistrationApiController> logger, IRegistrationServices registrationServices)
        {
            _logger = logger;
            _registrationServices = registrationServices;
        }

        /// <summary>
        /// Get all registrations with pagination
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: PAGE_SIZE)</param>
        /// <returns>List of registrations for the specified page</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RegistrationModel>>> GetAllRegistrations(int page = 1, int pageSize = PAGE_SIZE)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = PAGE_SIZE;

                var regData = await _registrationServices.GetAllRegistration(page - 1, pageSize);
                var totalCount = await _registrationServices.GetTotalRegistrationCount();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                return Ok(new
                {
                    data = regData,
                    currentPage = page,
                    totalPages = totalPages,
                    totalRecords = totalCount,
                    pageSize = pageSize
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all registrations");
                return StatusCode(500, new { response = false, result = "An error occurred while fetching registrations." });
            }
        }

        /// <summary>
        /// Get a specific registration by ID
        /// </summary>
        /// <param name="id">Registration ID</param>
        /// <returns>Registration details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<RegistrationModel>> GetRegistrationById(int id)
        {
            try
            {
                var registration = await _registrationServices.GetRegistrationById(id);
                if (registration == null)
                {
                    return NotFound(new { response = false, result = "Registration not found." });
                }

                return Ok(registration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching registration by ID: {id}", id);
                return StatusCode(500, new { response = false, result = "An error occurred while fetching registration." });
            }
        }

        /// <summary>
        /// Create a new registration
        /// </summary>
        /// <param name="registrationModel">Registration details</param>
        /// <returns>Created registration</returns>
        [HttpPost]
        public async Task<ActionResult<object>> CreateRegistration([FromBody] RegistrationModel registrationModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { response = false, result = "Invalid model state." });
            }

            try
            {
                var result = await _registrationServices.InsertRegistration(registrationModel);
                
                if (result.Item1)
                {
                    return CreatedAtAction(nameof(GetRegistrationById), new { id = registrationModel.Id }, 
                        new { response = result.Item1, result = result.Item2 });
                }
                else
                {
                    return BadRequest(new { response = result.Item1, result = result.Item2 });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating registration.");
                return StatusCode(500, new { response = false, result = "An error occurred while processing registration." });
            }
        }

        /// <summary>
        /// Update an existing registration
        /// </summary>
        /// <param name="id">Registration ID</param>
        /// <param name="registrationModel">Updated registration details</param>
        /// <returns>Update result</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<object>> UpdateRegistration(int id, [FromBody] RegistrationModel registrationModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { response = false, result = "Invalid model state." });
            }

            if (registrationModel.Id != id)
            {
                return BadRequest(new { response = false, result = "ID mismatch in request." });
            }

            try
            {
                var result = await _registrationServices.UpdateRegistration(registrationModel);
                
                if (result.Item1)
                {
                    return Ok(new { response = result.Item1, result = result.Item2 });
                }
                else
                {
                    return BadRequest(new { response = result.Item1, result = result.Item2 });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating registration with ID: {id}", id);
                return StatusCode(500, new { response = false, result = "An error occurred while updating registration." });
            }
        }

        /// <summary>
        /// Delete a registration
        /// </summary>
        /// <param name="id">Registration ID</param>
        /// <returns>Deletion result</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<object>> DeleteRegistration(int id)
        {
            try
            {
                var result = await _registrationServices.DeleteRegistration(id);
                
                if (result.Item1)
                {
                    return Ok(new { response = result.Item1, result = result.Item2 });
                }
                else
                {
                    return BadRequest(new { response = result.Item1, result = result.Item2 });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting registration with ID: {id}", id);
                return StatusCode(500, new { response = false, result = "An error occurred while deleting registration." });
            }
        }
    }
}
