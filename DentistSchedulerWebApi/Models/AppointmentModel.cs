using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DentistSchedulerWebApi.Models
{
    public class AppointmentModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }

        public AppointmentModel(AppointmentRequest request){
            StartTime = request.StartTime;
            EndTime = request.EndTime;
            FirstName = request.FirstName;
            LastName = request.LastName;
            Phone = request.Phone;
        }

        public AppointmentModel()
        {
            
        }
    }
}
