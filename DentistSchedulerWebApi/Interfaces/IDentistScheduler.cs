using DentistSchedulerWebApi.Models;

namespace DentistSchedulerWebApi.Interfaces
{
    public interface IDentistScheduler
    {
        public List<AppointmentModel> GetAppointmentsByName(string name);

        public List<AppointmentModel> GetAppointmentsByDate(string date);

        public List<AppointmentModel> PostNewAppointment(AppointmentModel input);

        public bool PostDeleteAppointment(AppointmentModel appointmentToDelete);

        public List<AppointmentModel> ListByFirstAndLastName(string firstName, string lastName);
    }
}
