using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CinemaApp.Data;
using CinemaApp.Models;

namespace CinemaApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TicketsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("book")]
        public async Task<IActionResult> BookTicket([FromBody] TicketBookingRequest request)
        {
            var showTime = await _context.ShowTimes
                .Include(s => s.Movie)
                .FirstOrDefaultAsync(s => s.ShowTimeId == request.ShowTimeId);

            if (showTime == null)
                return NotFound("Showtime not found.");

            if (DateTime.UtcNow > showTime.EndTime)
                return BadRequest("Cannot book ticket for an expired showtime.");

            var ticket = new Ticket
            {
                UserId = request.UserId,
                ShowTimeId = request.ShowTimeId,
                Status = TicketStatus.Active,
                BookingDate = DateTime.UtcNow
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return Ok(ticket);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTickets()
        {
            var tickets = await _context.Tickets
                .Include(t => t.User)
                .Include(t => t.ShowTime)
                .ThenInclude(st => st.Movie)
                .ToListAsync();

            return Ok(tickets);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicketById(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.User)
                .Include(t => t.ShowTime)
                .ThenInclude(st => st.Movie)
                .FirstOrDefaultAsync(t => t.TicketId == id);

            if (ticket == null)
                return NotFound("Ticket not found.");

            return Ok(ticket);
        }

        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelTicket(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
                return NotFound("Ticket not found.");

            ticket.Status = TicketStatus.Cancelled;
            await _context.SaveChangesAsync();

            return Ok("Ticket cancelled successfully.");
        }
    }

    public class TicketBookingRequest
    {
        public int UserId { get; set; }
        public int ShowTimeId { get; set; }
    }
}
