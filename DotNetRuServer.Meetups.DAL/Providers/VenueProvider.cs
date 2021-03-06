using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetRuServer.Meetups.BL.Entities;
using DotNetRuServer.Meetups.BL.Interfaces;
using DotNetRuServer.Meetups.DAL.Database;
using Microsoft.EntityFrameworkCore;

namespace DotNetRuServer.Meetups.DAL.Providers
{
    public class VenueProvider :  IVenueProvider
    {
        private readonly DotNetRuServerContext _context;

        public VenueProvider(DotNetRuServerContext context)
        {
            _context = context;
        }

        public Task<List<Venue>> GetAllVenuesAsync()
            => _context.Venues.ToListAsync();

        public Task<Venue> GetVenueOrDefaultAsync(string venueId)
            => _context.Venues.FirstOrDefaultAsync(x => x.ExportId == venueId);

        public async Task<Venue> SaveVenueAsync(Venue venue)
        {
            await _context.Venues.AddAsync(venue);
            await _context.SaveChangesAsync();
            return venue;
        }
    }
}