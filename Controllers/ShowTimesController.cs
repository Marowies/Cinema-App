using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CinemaApp.Data;
using CinemaApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShowTimesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ShowTimesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/showtimes
        [HttpGet]
        public async Task<IActionResult> GetAllShowTimes()
        {
            var showTimes = await _context.ShowTimes
                .Include(s => s.Movie)
                .ToListAsync();

            return Ok(showTimes);
        }

        // GET: api/showtimes/movie/5
        [HttpGet("movie/{movieId}")]
        public async Task<IActionResult> GetShowTimesByMovie(int movieId)
        {
            var showTimes = await _context.ShowTimes
                .Where(s => s.MovieId == movieId)
                .ToListAsync();

            if (!showTimes.Any())
                return NotFound($"No showtimes found for movie ID {movieId}");

            return Ok(showTimes);
        }

        // POST: api/showtimes
        [HttpPost]
        public async Task<IActionResult> AddShowTime([FromBody] ShowTime showTime)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if movie exists
            var movie = await _context.Movies.FindAsync(showTime.MovieId);
            if (movie == null)
                return BadRequest("Invalid MovieId: movie not found");

            // Check if showtime overlaps with movie date range
            if (showTime.StartTime < movie.StartDate || showTime.EndTime > movie.EndDate)
                return BadRequest("Showtime must be within the movie’s start and end dates");

            _context.ShowTimes.Add(showTime);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllShowTimes), new { id = showTime.ShowTimeId }, showTime);
        }

        // PUT: api/showtimes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShowTime(int id, [FromBody] ShowTime updatedShowTime)
        {
            var showTime = await _context.ShowTimes.FindAsync(id);
            if (showTime == null)
                return NotFound();

            showTime.StartTime = updatedShowTime.StartTime;
            showTime.EndTime = updatedShowTime.EndTime;
            showTime.HallNumber = updatedShowTime.HallNumber;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/showtimes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShowTime(int id)
        {
            var showTime = await _context.ShowTimes.FindAsync(id);
            if (showTime == null)
                return NotFound();

            _context.ShowTimes.Remove(showTime);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
