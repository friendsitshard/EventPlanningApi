using EventPlanningApi.DTOs;
using EventPlanningApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Cors;

namespace EventPlanningApi.Controllers
{
    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "*")]
    [Authorize]
    public class EventParticipantsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/EventParticipants
        [AllowAnonymous]
        [HttpGet]
        [Route("api/EventParticipants")]
        public IHttpActionResult GetEventParticipants(int pageNumber = 1, int pageSize = 5)
        {
            var totalParticipants = db.EventParticipants.Count();
            var totalPages = (int)Math.Ceiling(totalParticipants / (double)pageSize);

            var participants = db.EventParticipants
                .OrderBy(ep => ep.ParticipantId) // Optional: Order by ParticipantId or any other property
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(ep => new EventParticipantDto
                {
                    ParticipantId = ep.ParticipantId,
                    EventId = ep.EventId,
                    UserId = ep.UserId
                })
                .ToList();

            var paginationInfo = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalParticipants = totalParticipants,
                Participants = participants
            };

            return Ok(paginationInfo);
        }

        // GET: api/EventParticipants/5
        [ResponseType(typeof(EventParticipantDto))]
        [AllowAnonymous]
        [HttpGet]
        [Route("api/EventParticipants/{id:int}")]
        public async Task<IHttpActionResult> GetEventParticipant(int id)
        {
            var participant = await db.EventParticipants
                .Where(ep => ep.ParticipantId == id)
                .Select(ep => new EventParticipantDto
                {
                    ParticipantId = ep.ParticipantId,
                    EventId = ep.EventId,
                    UserId = ep.UserId
                })
                .SingleOrDefaultAsync();

            if (participant == null)
            {
                return NotFound();
            }

            return Ok(participant);
        }


        [ResponseType(typeof(IQueryable<UserDto>))]
        [AllowAnonymous]
        [HttpGet]
        [Route("api/EventParticipants/ByEvent/{eventId:int}")]
        public IHttpActionResult GetUsersByEvent(int eventId)
        {
            var users = db.EventParticipants
                .Where(ep => ep.EventId == eventId)
                .Select(ep => ep.User)
                .Select(u => new UserDto
                {
                    UserId = u.Id,
                    UserName = u.UserName,
                    Email = u.Email
                })
                .ToList();

            return Ok(users);
        }

        // PUT: api/EventParticipants/5
        [ResponseType(typeof(void))]
        [HttpPut]
        [Route("api/EventParticipants/{id:int}")]
        public async Task<IHttpActionResult> PutEventParticipant(int id, EventParticipantDto participantDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var participant = await db.EventParticipants.FindAsync(id);
            if (participant == null)
            {
                return NotFound();
            }

            participant.EventId = participantDto.EventId;
            participant.UserId = participantDto.UserId;

            db.Entry(participant).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventParticipantExists(id))
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

        // POST: api/EventParticipants
        [ResponseType(typeof(EventParticipant))]
        [HttpPost]
        [Route("api/EventParticipants")]
        public async Task<IHttpActionResult> PostEventParticipant(EventParticipantDto participantDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var participant = new EventParticipant
            {
                EventId = participantDto.EventId,
                UserId = participantDto.UserId
            };

            db.EventParticipants.Add(participant);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = participant.ParticipantId }, participant);
        }

        // DELETE: api/EventParticipants/5
        [HttpDelete]
        [Route("api/EventParticipants/{id:int}")]
        public async Task<IHttpActionResult> DeleteEventParticipant(int id)
        {
            var participant = await db.EventParticipants.FindAsync(id);
            if (participant == null)
            {
                return NotFound();
            }

            db.EventParticipants.Remove(participant);
            await db.SaveChangesAsync();

            return Ok("deleted" + participant);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EventParticipantExists(int id)
        {
            return db.EventParticipants.Count(ep => ep.ParticipantId == id) > 0;
        }
    }
}
