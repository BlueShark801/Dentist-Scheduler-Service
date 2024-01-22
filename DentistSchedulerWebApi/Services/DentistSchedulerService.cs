/// Service written by:  Brendan Johnston
/// Reference Used: https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-6.0&tabs=visual-studio

using DentistSchedulerWebApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DentistSchedulerWebApi.Services
{
    public class DentistSchedulerService : IDentistSchedulerService
    {
        private readonly IMongoCollection<AppointmentModel> _appointmentsCollection;

        public DentistSchedulerService(IOptions<DentistSchedulerDatabaseSettings> dentistSchedulerDatabaseSettings)
        {
            var mongoClient = new MongoClient(dentistSchedulerDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(dentistSchedulerDatabaseSettings.Value.DatabaseName);

            _appointmentsCollection = mongoDatabase.GetCollection<AppointmentModel>(dentistSchedulerDatabaseSettings.Value.AppointmentsCollectionName);
        }

        public async Task<List<AppointmentModel>> ListAsync() =>
            await _appointmentsCollection.Find(_ => true).ToListAsync();

        public async Task<AppointmentModel?> GetAsync(string id)
        {
            return await _appointmentsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(AppointmentModel newAppointment)
        {
            await _appointmentsCollection.InsertOneAsync(newAppointment);
        }

        /// <summary>
        /// Helper method to find duplicates in the database.
        /// </summary>
        /// <param name="newAppointment">Appointment being created.</param>
        /// <returns>true if duplicate found</returns>
        public async Task<bool> CheckDuplicates(AppointmentModel newAppointment)
        {
            var foundAppointmentDuplicate = await _appointmentsCollection.FindAsync(x =>
            x.FirstName == newAppointment.FirstName &&
            x.LastName == newAppointment.LastName &&
            x.StartTime == newAppointment.StartTime &&
            x.EndTime == newAppointment.EndTime &&
            x.Phone == newAppointment.Phone);

            if (foundAppointmentDuplicate.ToList().Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public static DateTime MilisecondTimeStampToDateTime(long millisecondTimeStamp)
        {
            // Unix timestamp is milliseconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddMilliseconds(millisecondTimeStamp).ToLocalTime();
            return dateTime;
        }

        public async Task UpdateAsync(string id, AppointmentModel updatedAppointment) =>
            await _appointmentsCollection.ReplaceOneAsync(x => x.Id == id, updatedAppointment);

        /// <summary>
        /// Deletes a single document matching the provided search criteria.
        /// </summary>
        public async Task RemoveAsync(string id) =>
            await _appointmentsCollection.DeleteOneAsync(x => x.Id == id);

        public async Task<List<AppointmentModel>> ListByFirstNameAsync(string firstname)
        {
            var firstNameCheckedAppointments = await _appointmentsCollection.FindAsync(x => x.FirstName == firstname);
            return firstNameCheckedAppointments.ToList();
        }

        public async Task<List<AppointmentModel>> ListByLastNameAsync(string lastName)
        {
            var firstNameCheckedAppointments = await _appointmentsCollection.FindAsync(x => x.LastName == lastName);
            return firstNameCheckedAppointments.ToList();
        }

        public async Task<List<AppointmentModel>> ListByFirstAndLastName(string firstName, string lastName)
        {
            var firstNameList = new List<AppointmentModel>();
            var lastNameList = new List<AppointmentModel>();

            if (firstName != null)
            {
                firstNameList = (await _appointmentsCollection.FindAsync(x => x.FirstName == firstName)).ToList();
            }
            if (lastName != null)
            {
                lastNameList = (await _appointmentsCollection.FindAsync(x => x.LastName == lastName)).ToList();
            }
            return firstNameList.Union(lastNameList).ToList();
        }

        public async Task<List<AppointmentModel>> ListAppointmentsDuringTime(string startTime, string endTime)
        {
            var appointmentsInTimeframe = await _appointmentsCollection.FindAsync(x =>
            x.StartTime == startTime &&
            x.EndTime == endTime);

            return appointmentsInTimeframe.ToList();
        }

        /// <summary>
        /// Helper method to find the time between two DateTime objects.
        /// </summary>
        /// <param name="newStart">Unix timestamp of the new appointment start</param>
        /// <param name="newEnd">Unix timestamp of the new appointment end</param>
        /// <param name="startString">Unix timestamp of the existing appointment start</param>
        /// <param name="endString">Unix timestamp of the existing appointment end</param>
        /// <returns>true if appointment exists in that time. False if appointment does not exist in that time.</returns>
        private bool NewAppointmentTimeIsBetween(string newStart, string newEnd, string startString, string endString)
        {
            try
            {
                long newStartUnix = long.Parse(newStart);
                long newEndUnix = long.Parse(newEnd);
                long appointmentStartUnix = long.Parse(startString);
                long appointmentEndUnix = long.Parse(endString);

                if ((newStartUnix >= appointmentStartUnix && newStartUnix <= appointmentEndUnix) ||
                    (newEndUnix >= appointmentStartUnix && newEndUnix <= appointmentEndUnix))
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}