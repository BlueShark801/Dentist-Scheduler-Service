/// Service written by:  Brendan Johnston
/// Reference Used: https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-6.0&tabs=visual-studio

using System.Collections;
using DentistSchedulerWebApi.Interfaces;
using DentistSchedulerWebApi.Models;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using MongoDB.Driver;

namespace DentistSchedulerWebApi.Services
{
    public class DentistSchedulerTestService : IDentistSchedulerService
    {
        List<AppointmentModel> _appointmentsCollection;
        public DentistSchedulerTestService()
        {
            _appointmentsCollection = new List<AppointmentModel>
            {
                new AppointmentModel
                {
                    Id = "507f1f77bcf86cd799439011",
                    FirstName = "John",
                    LastName = "Doe",
                    StartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
                    EndTime = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeMilliseconds().ToString(),
                    Phone = "+11234567890"
                },
                new AppointmentModel
                {
                    Id = "507f1f77bcf86cd799439012",
                    FirstName = "Jane",
                    LastName = "Smith",
                    StartTime = DateTimeOffset.UtcNow.AddHours(2).ToUnixTimeMilliseconds().ToString(),
                    EndTime = DateTimeOffset.UtcNow.AddHours(3).ToUnixTimeMilliseconds().ToString(),
                    Phone = "+19876543210"
                },
                new AppointmentModel
                {
                     Id = "507f1f77bcf86cd799439013",
                    FirstName = "Alice",
                    LastName = "Johnson",
                    StartTime = DateTimeOffset.UtcNow.AddHours(4).ToUnixTimeMilliseconds().ToString(),
                    EndTime = DateTimeOffset.UtcNow.AddHours(5).ToUnixTimeMilliseconds().ToString(),
                    Phone = "+15555555555"
                },
                new AppointmentModel
                {
                     Id = "507f1f77bcf86cd799439014",
                    FirstName = "Bob",
                    LastName = "Williams",
                    StartTime = DateTimeOffset.UtcNow.AddHours(6).ToUnixTimeMilliseconds().ToString(),
                    EndTime = DateTimeOffset.UtcNow.AddHours(7).ToUnixTimeMilliseconds().ToString(),
                    Phone = "+19999999999"
                },
                new AppointmentModel
                {
                    Id = "507f1f77bcf86cd799439015",
                    FirstName = "Emily",
                    LastName = "Davis",
                    StartTime = DateTimeOffset.UtcNow.AddHours(8).ToUnixTimeMilliseconds().ToString(),
                    EndTime = DateTimeOffset.UtcNow.AddHours(9).ToUnixTimeMilliseconds().ToString(),
                    Phone = "+11111111111"
                }
            };
        }

        public Task CreateAsync(AppointmentModel newAppointment)
        {
            _appointmentsCollection.Add(newAppointment);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Helper method to find duplicates in the database.
        /// </summary>
        /// <param name="newAppointment">Appointment being created.</param>
        /// <returns>true if duplicate found</returns>
        public Task<bool> CheckDuplicates(AppointmentModel newAppointment)
        {
            var foundAppointmentDuplicate = _appointmentsCollection.FindAll(x =>
            x.FirstName == newAppointment.FirstName &&
            x.LastName == newAppointment.LastName &&
            x.StartTime == newAppointment.StartTime &&
            x.EndTime == newAppointment.EndTime &&
            x.Phone == newAppointment.Phone);

            if (foundAppointmentDuplicate.ToList().Count > 0)
            {
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }

        }

        /// <summary>
        /// Helper method to find duplicates in the database.
        /// </summary>
        /// <param name="newAppointment">Appointment being created.</param>
        /// <returns>true if duplicate found</returns>
        public Task<List<AppointmentModel>> ListAppointmentsDuringTime(string startTime, string endTime)
        {
            List<AppointmentModel> appointmentsInTimeframe = new();
            appointmentsInTimeframe = _appointmentsCollection.FindAll(x =>
            x.StartTime == startTime &&
            x.EndTime == endTime);

            return Task.FromResult(appointmentsInTimeframe);
        }

        public static DateTime MilisecondTimeStampToDateTime(long millisecondTimeStamp)
        {
            // Unix timestamp is milliseconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddMilliseconds(millisecondTimeStamp).ToLocalTime();
            return dateTime;
        }

        public Task UpdateAsync(string id, AppointmentModel updatedAppointment)
        {
            var appointmentToUpdate = _appointmentsCollection.FirstOrDefault(x => x.Id == id);
            if (appointmentToUpdate == null)
            {
                return Task.CompletedTask;
            }
            _appointmentsCollection.Remove(appointmentToUpdate);
            _appointmentsCollection.Add(updatedAppointment);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Deletes a single document matching the provided search criteria.
        /// </summary>
        public Task RemoveAsync(string id)
        {
            var appointmentToUpdate = _appointmentsCollection.FirstOrDefault(x => x.Id == id);
            if (appointmentToUpdate == null)
            {
                return Task.CompletedTask;
            }
            _appointmentsCollection.Remove(appointmentToUpdate);
            return Task.CompletedTask;
        }

        public Task<AppointmentModel?> GetAsync(string id)
        {
            return Task.FromResult(_appointmentsCollection.FirstOrDefault(x => x.Id == id));
        }

        public Task<List<AppointmentModel>> ListAsync()
        {
            return Task.FromResult(_appointmentsCollection);
        }

        public Task<List<AppointmentModel>> ListByFirstAndLastName(string firstName, string lastName)
        {
            var firstNameList = new List<AppointmentModel>();
            var lastNameList = new List<AppointmentModel>();

            if (firstName != null)
            {
                firstNameList = _appointmentsCollection.FindAll(x => x.FirstName == firstName);
            }
            if (lastName != null)
            {
                lastNameList = _appointmentsCollection.FindAll(x => x.LastName == lastName);
            }
            return Task.FromResult(firstNameList.Union(lastNameList).ToList());
        }
    }
}
