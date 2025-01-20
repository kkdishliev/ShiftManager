using Microsoft.AspNetCore.Mvc;
using ShiftManager.Application.DTOs.Shifts;
using ShiftManager.Application.Interfaces;
using ShiftManager.Domain.Entities;

namespace ShiftManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftController : ControllerBase
    {
        private readonly IShiftService _shiftService;

        public ShiftController(IShiftService shiftService)
        {
            _shiftService = shiftService;
        }

        /// <summary>
        /// Retrieves all shifts in the system.
        /// </summary>
        /// <returns>A list of all shifts.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Shift>), 200)] 
        [ProducesResponseType(400)] 
        public async Task<ActionResult<IEnumerable<Shift>>> GetShifts()
        {
            var shifts = await _shiftService.GetAllShiftsAsync();
            return Ok(shifts);
        }

        /// <summary>
        /// Retrieves a specific shift by its ID.
        /// </summary>
        /// <param name="id">The ID of the shift to retrieve.</param>
        /// <returns>The shift with the given ID, or a 404 Not Found status if no shift is found.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Shift), 200)]
        [ProducesResponseType(404)] 
        public async Task<ActionResult<Shift>> GetShift(int id)
        {
            var shift = await _shiftService.GetShiftByIdAsync(id);

            if (shift == null)
            {
                return NotFound();
            }

            return Ok(shift);
        }

        /// <summary>
        /// Adds a new shift to the system.
        /// </summary>
        /// <param name="shift">The data transfer object containing the shift's information.</param>
        /// <returns>A 201 Created status with the details of the created shift, or a 400 Bad Request with error details if the operation fails.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Shift), 201)] 
        [ProducesResponseType(400)] 
        public async Task<ActionResult<Shift>> PostShift(ShiftCreateDto shift)
        {
            var result = await _shiftService.AddShiftAsync(shift);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetShift), new { id = result.Data.Id }, result);
            }

            return BadRequest(new
            {
                result.Message,
                result.Errors
            });
        }

        /// <summary>
        /// Updates the details of an existing shift.
        /// </summary>
        /// <param name="id">The ID of the shift to update.</param>
        /// <param name="shift">The shift object containing the updated shift details.</param>
        /// <returns>A 204 No Content status if the update is successful, or a 400 Bad Request if the IDs do not match.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)] 
        [ProducesResponseType(400)]
        [ProducesResponseType(404)] 
        public async Task<IActionResult> PutShift(int id, ShiftUpdateDto shift)
        {
            if (id != shift.Id)
            {
                return BadRequest(new { message = "Shift ID does not match the ID in the URL." });
            }

            var result = await  _shiftService.UpdateShiftAsync(shift);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return StatusCode(500, new { message = "An error occurred while updating the shift.", details = result.Message });
        }

        /// <summary>
        /// Deletes a shift from the system.
        /// </summary>
        /// <param name="id">The ID of the shift to delete.</param>
        /// <returns>A 204 No Content status if the deletion is successful, or a 404 Not Found if the shift does not exist.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> DeleteShift(int id)
        {
            var result = await _shiftService.DeleteShiftAsync(id);

            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                if (result.Message == "Shift not found")
                {
                    return NotFound(result);
                }

                return BadRequest(result);
            }
        }

        [HttpGet("week")]
        public async Task<IActionResult> GetShiftsForWeek([FromQuery] DateTime startOfWeek, [FromQuery] DateTime endOfWeek)
        {
            try
            {
                DateOnly startOfWeekDateOnly = DateOnly.FromDateTime(startOfWeek);
                DateOnly endOfWeekDateOnly = DateOnly.FromDateTime(endOfWeek);

                var shifts = await _shiftService.GetShiftsForWeekAsync(startOfWeekDateOnly, endOfWeekDateOnly);
                return Ok(shifts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
