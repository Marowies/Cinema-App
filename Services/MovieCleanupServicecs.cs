using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CinemaApp.Data;
using CinemaApp.Models;
using System.Linq;

namespace CinemaApp.Services
{
    // Background service that runs periodically to remove expired movies and expire tickets
    public class MovieCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(30); // adjust as desired

        public MovieCleanupService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await DoWork(stoppingToken);
                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task DoWork(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var now = DateTime.UtcNow;

                // 1. Find movies that have EndDate earlier than now
                var expiredMovies = await db.Movies
                    .Where(m => m.EndDate < now)
                    .Include(m => m.ShowTimes)
                    .ThenInclude(s => s.Tickets)
                    .ToListAsync(cancellationToken);

                if (expiredMovies.Any())
                {
                    foreach (var movie in expiredMovies)
                    {
                        // Mark related tickets as Expired
                        var showTimes = movie.ShowTimes ?? new System.Collections.Generic.List<ShowTime>();
                        foreach (var st in showTimes)
                        {
                            if (st.Tickets != null)
                            {
                                foreach (var ticket in st.Tickets)
                                {
                                    ticket.Status = TicketStatus.Expired;
                                }
                            }
                        }

                        // Optionally remove showtimes and movie from DB
                        db.Movies.Remove(movie);
                    }

                    await db.SaveChangesAsync(cancellationToken);
                }

                // 2. Also expire tickets for showtimes that are in the past (if not covered above)
                var expiredTickets = await db.Tickets
                    .Include(t => t.ShowTime)
                    .Where(t => t.Status == TicketStatus.Active && t.ShowTime.EndTime < now)
                    .ToListAsync(cancellationToken);

                if (expiredTickets.Any())
                {
                    foreach (var t in expiredTickets)
                    {
                        t.Status = TicketStatus.Expired;
                    }
                    await db.SaveChangesAsync(cancellationToken);
                }
            }
        }
    }
}