namespace DentistSchedulerWebApi.Models
{
    public class DentistSchedulerDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string AppointmentsCollectionName { get; set; } = null!;

    }
}
