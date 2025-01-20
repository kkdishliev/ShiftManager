using Microsoft.AspNetCore.Mvc;
using ShiftManager.Application.DTOs.Roles;
using ShiftManager.Application.Interfaces;

namespace ShiftManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Retrieves all roles in the system.
        /// </summary>
        /// <returns>A list of all roles.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RoleDto>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAllRoles(
            [FromQuery] int start = 0,
            [FromQuery] int size = 10,
            [FromQuery] string? globalFilter = null,
            [FromQuery] string? filters = null,
            [FromQuery] string? sorting = null)
        {
            try
            {
                var response = await _roleService.GetAllRolesAsync(start, size, globalFilter, filters, sorting);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", details = ex.Message });
            }
        }

        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<RoleDto>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAllRolesNoFilters()
        {
            try
            {
                var response = await _roleService.GetAllRolesAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", details = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a specific role by its ID.
        /// </summary>
        /// <param name="id">The ID of the role to retrieve.</param>
        /// <returns>The role with the given ID, or a 404 Not Found status if no role is found.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RoleDto), 200)]
        [ProducesResponseType(404)] 
        public async Task<IActionResult> GetRole(int id)
        {
            var result = await _roleService.GetRoleByIdAsync(id);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return NotFound(new
            {
                result.Message,
                result.Errors
            });
        }

        /// <summary>
        /// Adds a new role to the system.
        /// </summary>
        /// <param name="roleDto">The data transfer object containing the role's information.</param>
        /// <returns>A 201 Created status with the details of the created role, or a 400 Bad Request with error details if the operation fails.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(RoleDto), 201)] 
        [ProducesResponseType(400)] 
        public async Task<IActionResult> PostRole(RoleDto roleDto)
        {
            var result = await _roleService.AddRoleAsync(roleDto);
            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetRole), new { id = result.Data.Id }, result.Data);
            }

            return BadRequest(new
            {
                result.Message,
                result.Errors
            });
        }

        /// <summary>
        /// Updates an existing role's details.
        /// </summary>
        /// <param name="id">The ID of the role to update.</param>
        /// <param name="roleDto">The data transfer object containing the updated role details.</param>
        /// <returns>A 204 No Content status if the update is successful, or a 400 Bad Request if the IDs do not match.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)] 
        [ProducesResponseType(400)] 
        [ProducesResponseType(404)] 
        public async Task<IActionResult> PutRole(int id, RoleDto roleDto)
        {
            if (id != roleDto.Id)
            {
                return BadRequest("Role ID mismatch.");
            }

            var result = await _roleService.UpdateRoleAsync(roleDto);
            if (result.IsSuccess)
            {
                return NoContent();
            }

            return BadRequest(new
            {
                result.Message,
                result.Errors
            });
        }

        /// <summary>
        /// Deletes a role from the system.
        /// </summary>
        /// <param name="id">The ID of the role to delete.</param>
        /// <returns>A 204 No Content status if the deletion is successful, or a 404 Not Found if the role does not exist.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)] 
        [ProducesResponseType(404)] 
        public async Task<IActionResult> DeleteRole(int id)
        {
            var result = await _roleService.DeleteRoleAsync(id);
            if (result.IsSuccess)
            {
                return NoContent();
            }

            return BadRequest(new
            {
                result.Message,
                result.Errors
            });
        }

        /// <summary>
        /// Retrieves roles associated with a specific employee.
        /// </summary>
        /// <param name="employeeId">The ID of the employee.</param>
        /// <returns>A list of roles for the given employee.</returns>
        [HttpGet("employee")]
        [ProducesResponseType(typeof(IEnumerable<RoleDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetRolesByEmployeeId([FromQuery] int employeeId)
        {
            if (employeeId <= 0)
            {
                return BadRequest(new { message = "Invalid employee ID." });
            }

            try
            {
                var result = await _roleService.GetRolesByEmployeeIdAsync(employeeId);
                if (result.IsSuccess && result.Data != null)
                {
                    return Ok(result.Data);
                }

                return NotFound(new
                {
                    message = result.Message ?? "No roles found for the specified employee."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while fetching roles.",
                    details = ex.Message
                });
            }
        }
    }
}