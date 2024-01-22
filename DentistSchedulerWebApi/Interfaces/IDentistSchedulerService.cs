

using DentistSchedulerWebApi.Models;

public interface IDentistSchedulerService
{
    Task CreateAsync(AppointmentModel newAppointment);
    Task<AppointmentModel?> GetAsync(string id);
    Task<List<AppointmentModel>> ListAsync();
    Task<List<AppointmentModel>> ListByFirstAndLastName(string firstName, string lastName);
    Task<List<AppointmentModel>> ListAppointmentsDuringTime(string startTime, string endTime);
    Task<bool> CheckDuplicates(AppointmentModel newAppointment);
    Task UpdateAsync(string id, AppointmentModel updatedAppointment);
    Task RemoveAsync(string id);
}