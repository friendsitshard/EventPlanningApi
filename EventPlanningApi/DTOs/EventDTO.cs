using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventPlanningApi.DTOs
{
    public class EventDto
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public int EventCapacity { get; set; }
        public DateTime EventDate { get; set; }
        public string EventLocation { get; set; }
        public string UserId { get; set; }
        public ICollection<EventParticipantDto> EventParticipants { get; set; }
    }
}