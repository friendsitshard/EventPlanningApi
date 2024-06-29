using EventPlanningApi.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class EventParticipant
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ParticipantId { get; set; }

    [ForeignKey("Event")]
    public int EventId { get; set; }

    [ForeignKey("User")]
    public string UserId { get; set; }

    public virtual Event Event { get; set; }
    public virtual ApplicationUser User { get; set; }
}
