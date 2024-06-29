using EventPlanningApi.DTOs;
using EventPlanningApi.Models;
using Microsoft.AspNet.Identity;
using System.Web.Http;
using System.Web.Http.Cors;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http.Description;

namespace EventPlanningApi.Controllers
{
    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "*")]
    public class EventsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Events
        [HttpGet]
        [Route("api/Events")]
        public IHttpActionResult GetEvents(int pageNumber = 1, int pageSize = 5)
        {
            var totalEvents = db.Events.Count();
            var totalPages = (int)Math.Ceiling(totalEvents / (double)pageSize);

            var events = db.Events
                .OrderBy(e => e.EventId) 
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new EventDto
                {
                    EventId = e.EventId,
                    EventName = e.EventName,
                    EventCapacity = e.EventCapacity,
                    EventDate = e.EventDate,
                    EventLocation = e.EventLocation
                })
                .ToList();

            var paginationInfo = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalEvents = totalEvents,
                Events = events
            };

            return Ok(paginationInfo);
        }


        // GET: api/Events/5
        [ResponseType(typeof(EventDto))]
        public async Task<IHttpActionResult> GetEvent(int id)
        {
            var @event = await db.Events
                .Where(e => e.EventId == id)
                .Select(e => new EventDto
                {
                    EventId = e.EventId,
                    EventName = e.EventName,
                    EventCapacity = e.EventCapacity,
                    EventDate = e.EventDate,
                    EventLocation = e.EventLocation
                })
                .SingleOrDefaultAsync();

            if (@event == null)
            {
                return NotFound();
            }

            return Ok(@event);
        }

        // PUT: api/Events/5
        [Authorize(Roles = "Organizer, Admin")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutEvent(int id, EventDto eventDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var @event = await db.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }

            @event.EventName = eventDto.EventName;
            @event.EventCapacity = eventDto.EventCapacity;
            @event.EventDate = eventDto.EventDate;
            @event.EventLocation = eventDto.EventLocation;

            db.Entry(@event).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Updated");
        }

        // POST: api/Events/FilterByYear
        [AllowAnonymous]
        [HttpPost]
        [Route("api/Events/FilterByYear")]
        public IHttpActionResult FilterEventsByYear(int year, int pageNumber = 1, int pageSize = 5)
        {
            var query = db.Events.Where(e => e.EventDate.Year == year);

            var totalEvents = query.Count();
            var totalPages = (int)Math.Ceiling(totalEvents / (double)pageSize);

            var events = query
                .OrderBy(e => e.EventId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new EventDto
                {
                    EventId = e.EventId,
                    EventName = e.EventName,
                    EventCapacity = e.EventCapacity,
                    EventDate = e.EventDate,
                    EventLocation = e.EventLocation
                })
                .ToList();

            var paginationInfo = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalEvents = totalEvents,
                Events = events
            };

            return Ok(paginationInfo);
        }

        // POST: api/Events/FilterByCapacity
        [AllowAnonymous]
        [HttpPost]
        [Route("api/Events/FilterByCapacity")]
        public IHttpActionResult FilterEventsByCapacity(int capacity, int pageNumber = 1, int pageSize = 5)
        {
            var query = db.Events.Where(e => e.EventCapacity == capacity);

            var totalEvents = query.Count();
            var totalPages = (int)Math.Ceiling(totalEvents / (double)pageSize);

            var events = query
                .OrderBy(e => e.EventId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new EventDto
                {
                    EventId = e.EventId,
                    EventName = e.EventName,
                    EventCapacity = e.EventCapacity,
                    EventDate = e.EventDate,
                    EventLocation = e.EventLocation
                })
                .ToList();

            var paginationInfo = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalEvents = totalEvents,
                Events = events
            };

            return Ok(paginationInfo);
        }


        // POST: api/Events
        [ResponseType(typeof(Event))]
        [HttpPost]
        [Route("api/Events/post")]
        public async Task<IHttpActionResult> PostEvent(EventDto eventDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (eventDto.EventDate < SqlDateTime.MinValue.Value || eventDto.EventDate > SqlDateTime.MaxValue.Value)
            {
                return BadRequest("EventDate is out of range. Please provide a date within the valid range.");
            }

            var userId = User.Identity.GetUserId();

            var @event = new Event
            {
                EventName = eventDto.EventName,
                EventCapacity = eventDto.EventCapacity,
                EventDate = eventDto.EventDate,
                EventLocation = eventDto.EventLocation,
                UserId = userId
            };

            db.Events.Add(@event);
            await db.SaveChangesAsync();

            return Ok(@event);
        }

        // DELETE: api/Events/5
        public async Task<IHttpActionResult> DeleteEvent(int id)
        {
            var @event = await db.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }

            // Delete related EventParticipants
            var eventParticipants = db.EventParticipants.Where(ep => ep.EventId == id);
            db.EventParticipants.RemoveRange(eventParticipants);

            // Now delete the event
            db.Events.Remove(@event);
            await db.SaveChangesAsync();

            return Ok("deleted" + @event);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EventExists(int id)
        {
            return db.Events.Count(e => e.EventId == id) > 0;
        }
    }
}
