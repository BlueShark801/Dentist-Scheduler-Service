/// Written by Brendan Johnston
/// Reference used: https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-6.0&tabs=visual-studio

using DentistSchedulerWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using DentistSchedulerWebApi.Services;
using Microsoft.AspNetCore.Cors;
using System.Diagnostics;

namespace DentistSchedulerWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DentistSchedulerController : ControllerBase
    {

        private readonly IDentistSchedulerService _dentistService;

        public DentistSchedulerController(IDentistSchedulerService dentistService)
        {
            _dentistService = dentistService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentModel>> Get([FromRoute] string id)
        {
            var appointment = await _dentistService.GetAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }
            return appointment;
        }

        /// <summary>
        /// List to query on first or last name or both.
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns></returns>
        [HttpGet("appointments")]
        public async Task<List<AppointmentModel>> GetAll([FromQuery] string? firstName, [FromQuery] string? lastName, [FromQuery] string? startTime, [FromQuery] string? endTime)
        {

            List<AppointmentModel> returnedObject = new();
            if (firstName != null || lastName != null)
            {
                returnedObject = await _dentistService.ListByFirstAndLastName(firstName, lastName);
            }
            if (startTime != null && endTime != null)
            {
                returnedObject = await _dentistService.ListAppointmentsDuringTime(startTime, endTime);
            }
            if (firstName == null && lastName == null && startTime == null && endTime == null)
            {
                returnedObject = await _dentistService.ListAsync();
            }

            return returnedObject;
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateAppointment([FromBody] AppointmentRequest request)
        {
            AppointmentModel newAppointment = new(request);

            if (newAppointment is null)
            {
                return BadRequest("Invalid input to Web API, check your data and try again.");
            }

            try
            {
                if (_dentistService.CheckDuplicates(newAppointment).Result)
                {
                    return BadRequest("ERROR 400: Cannot create a duplicate appointment!");
                }

                await _dentistService.CreateAsync(newAppointment);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Invalid input to Web API, check your data and try again.\n" + ex);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] AppointmentRequest request)
        {

            AppointmentModel updatedAppointment = new(request);

            if (updatedAppointment is null)
            {
                return BadRequest("Invalid input to Web API, check your data and try again.");
            }

            var appointment = await _dentistService.GetAsync(id);

            if (appointment is null)
            {
                return NotFound();
            }

            updatedAppointment.Id = appointment.Id;

            await _dentistService.UpdateAsync(id, updatedAppointment);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            var appointment = await _dentistService.GetAsync(id);

            if (appointment is null)
            {
                return NotFound();
            }

            await _dentistService.RemoveAsync(appointment.Id);

            return Ok();
        }

        [HttpGet("available-times")]
        public async Task<List<AppointmentModel>> GetAvailableTimes([FromQuery] string startTime, [FromQuery] string endTime, [FromQuery] int duration)
        {
            return new List<AppointmentModel>();
            // List<AppointmentModel> returnedObject = new();

            // if (startTime != null && endTime != null)
            // {
            //     returnedObject = await _dentistService.ListAvailableTimes(startTime, endTime);
            // }

            // return returnedObject;
        }
    }
}