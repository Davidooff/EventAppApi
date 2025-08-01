using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class EventService
{
    private readonly IDatabaseContext _context;
    public EventService(IDatabaseContext databaseContext)
    {
        _context = databaseContext;
    }

    public Task<Event[]> GetEventsAsync(int start, int count)
    {
        return _context.Events.AsNoTracking().Skip(start).Take(count).ToArrayAsync();
    }
    
    public async Task<Event[]> GetEventsByCategoryAsync(int categoryId, int start, int count)
    {
        var category = await _context.Categories.FindAsync(categoryId);
        if (category == null)
            throw new Exception("Category not found");
        
        return category.Events.Skip(start).Take(count).ToArray();
    }
    
    public Task CreateAsync(Event newEvent)
    {
        _context.Events.Add(newEvent);
        return _context.SaveChangesAsync();
    }
    
    public Task<Event?> GetEventInfo(int eventId)
    {
        return _context.Events.Include(e => e.Schedules)
            .Include(e => e.Category)
            .Include(e => e.Audiences)
            .Include(e => e.Speakers)
            .FirstOrDefaultAsync(e => e.Id == eventId);
    }

    public async Task<bool> DeleteEventAsync(int eventId)
    {
        var eventDb = await _context.Events.FindAsync(eventId);
        if (eventDb == null)
            return false;
        
        _context.Events.Remove(eventDb);
        await _context.SaveChangesAsync();
        return true;
    }

    public Task AddSchedule(Schedule schedule)
    {
        _context.Schedules.Add(schedule);
        return _context.SaveChangesAsync();
    }
    
    public Task UpdateSchedule(Schedule schedule)
    {
        _context.Schedules.Update(schedule);
        return _context.SaveChangesAsync();
    }
    
    public async Task<bool> DeleteSchedule(int id)
    {
        var schedule = await _context.Schedules.FindAsync(id);
        if (schedule == null)
            return false;
        _context.Schedules.Remove(schedule);
        await _context.SaveChangesAsync();
        return true;
    }
}