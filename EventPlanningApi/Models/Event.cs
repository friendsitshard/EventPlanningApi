using EventPlanningApi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Event
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int EventId { get; set; }

    [StringLength(100)]
    public string EventName { get; set; }

    public int EventCapacity { get; set; }

    public DateTime EventDate { get; set; }

    [StringLength(200)]
    public string EventLocation { get; set; }

    [ForeignKey("User")]
    public string UserId { get; set; }

    public virtual ApplicationUser User { get; set; }

    public virtual ICollection<EventParticipant> EventParticipants { get; set; }
}
