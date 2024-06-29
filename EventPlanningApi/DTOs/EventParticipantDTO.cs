using EventPlanningApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventPlanningApi.DTOs
{
    public class EventParticipantDto
    {
        public int ParticipantId { get; set; }
        public int EventId { get; set; }
        public string UserId { get; set; }
    }
}