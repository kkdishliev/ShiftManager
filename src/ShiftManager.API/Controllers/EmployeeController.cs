using Microsoft.AspNetCore.Mvc;
using ShiftManager.Application.DTOs.Employees;
using ShiftManager.Application.Interfaces;
using ShiftManager.Domain.Entities;

namespace ShiftManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        /// <summary>
        /// Retrieves all employees in the system.
        /// </summary>
        /// <returns>A list of all employees along with metadata for pagination.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(EmployeeApiResponse<EmployeeDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<EmployeeApiResponse<EmployeeDto>>> GetEmployees(
            [FromQuery] int start = 0,
            [FromQuery] int size = 10,
            [FromQuery] string globalFilter = "",
            [FromQuery] string? filters = null,
            [FromQuery] string? sorting = null)
        {
            var response = await _employeeService.GetAllEmployeesAsync(start, size, globalFilter, filters, sorting);

            return Ok(response);
        }

        /// <summary>
        /// Retrieves a specific employee by their ID.
        /// </summary>
        /// <param name="id">The ID of the employee to retrieve.</param>
        /// <returns>The employee with the given ID, or a 404 Not Found status if no employee is found.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Employee), 200)] 
        [ProducesResponseType(404)] 
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        /// <summary>
        /// Adds a new employee to the system.
        /// </summary>
        /// <param name="employeeDto">The data transfer object containing the employee's information.</param>
        /// <returns>A 201 Created status with the details of the created employee, or a 400 Bad Request with error details if the operation fails.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Employee), 201)] 
        [ProducesResponseType(400)] 
        public async Task<IActionResult> PostEmployee(EmployeeDto employeeDto)
        {
            var result = await _employeeService.AddEmployeeAsync(employeeDto);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetEmployee), new { id = result.Data.Id }, result);
            }

            return BadRequest(new
            {
                result.Message,
                result.Errors
            });
        }

        /// <summary>
        /// Updates the details of an existing employee.
        /// </summary>
        /// <param name="id">The ID of the employee to update.</param>
        /// <param name="employee">The data transfer object containing the updated employee details.</param>
        /// <returns>A 204 No Content status if the update is successful, or a 400 Bad Request if the IDs do not match.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)] 
        public async Task<IActionResult> PutEmployee(int id, EmployeeDto employee)
        {
            if (id != employee.Id)
            {
                return BadRequest(new { message = "Employee ID does not match the ID in the URL." });
            }

            var result = await _employeeService.UpdateEmployeeAsync(employee);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return StatusCode(500, new { message = "An error occurred while updating the employee.", details = result.Message });
        }


        /// <summary>
        /// Deletes an employee from the system.
        /// </summary>
        /// <param name="id">The ID of the employee to delete.</param>
        /// <returns>A 204 No Content status if the deletion is successful, or a 404 Not Found if the employee does not exist.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var result = await _employeeService.DeleteEmployeeAsync(id);

            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                if (result.Message == "Employee not found")
                {
                    return NotFound(result);
                }

                return BadRequest(result);
            }
        }

        [HttpPost("{employeeId}/roles")]
        public async Task<IActionResult> AddRolesToEmployee(int employeeId, [FromBody] List<int> roleIds)
        {
            if (roleIds == null || !roleIds.Any())
            {
                return BadRequest("Role IDs cannot be null or empty.");
            }

            var result = await _employeeService.AddRolesToEmployeeAsync(employeeId, roleIds);

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(new
            {
                result.Message,
                result.Errors
            });
        }

        /// <summary>
        /// Retrieves all employees for populating dropdown lists.
        /// </summary>
        /// <returns>A list of employees for the dropdown.</returns>
        [HttpGet("dropdown")]
        [ProducesResponseType(typeof(List<EmployeeDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<List<EmployeeDto>>> GetEmployeesForDropdown()
        {
            var employees = await _employeeService.GetAllEmployeesForDropdownAsync();

            if (employees == null || !employees.Any())
            {
                return NotFound("No employees found.");
            }

            return Ok(employees);
        }

    }
}