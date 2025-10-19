using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaApp.Models
{
    public enum TicketStatus
    {
        Active,
        Cancelled,
        Expired
    }

    public class Ticket
    {
        [Key]
        public int TicketId { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        [ForeignKey("ShowTime")]
        public int ShowTimeId { get; set; }
        public ShowTime ShowTime { get; set; }

        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        [Required]
        public TicketStatus Status { get; set; } = TicketStatus.Active;
    }
}
