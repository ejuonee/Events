using Events.API.Data;
using Events.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Events.API.Data
{
    public class Seed
    {
        private readonly DataContext _context;
        private readonly ILogger<Seed> _logger;

        public Seed(DataContext context, ILogger<Seed> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        public async Task ClearDatabaseAsync()
        {
            _context.Invitations.RemoveRange(_context.Invitations);
            _context.Participants.RemoveRange(_context.Participants);
            _context.Events.RemoveRange(_context.Events);
            _context.Users.RemoveRange(_context.Users);

            await _context.SaveChangesAsync();
        }

        public async Task SeedAsync()
        {
            try
            {
                await _context.Database.MigrateAsync();
                _logger.LogInformation("Database migration completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during database migration.");
                return;
            }

            //clear the database;
            //await ClearDatabaseAsync();
            
            if (!_context.Users.Any())
            {
                _logger.LogInformation("Seeding initial data...");

                try
                {
                    var testUsers = DataGenerator.GenerateUsers(20);
                    await _context.Users.AddRangeAsync(testUsers);
                    await _context.SaveChangesAsync();
                    var testEvents = DataGenerator.GenerateEvents(testUsers);
                    await _context.Events.AddRangeAsync(testEvents);
                    await _context.SaveChangesAsync();
                    var testParticipants = DataGenerator.GenerateParticipants(testEvents, testUsers);
                    await _context.Participants.AddRangeAsync(testParticipants);
                    await _context.SaveChangesAsync();
                    var testInvitations = DataGenerator.GenerateInvitations(testEvents, testUsers, testParticipants);
                    await _context.Invitations.AddRangeAsync(testInvitations);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Initial data seeded successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }
            else
            {
                _logger.LogInformation("Data already exists, skipping seeding.");
            }
        }
    }
}
