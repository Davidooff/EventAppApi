using Application.DTOs;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Authorization.RequirementsData;

namespace WebApplication1.Controllers;

[ApiController]
[Route("/events")]
public class EventController : ControllerBase
{
     private readonly EventService _eventService;

     public EventController(EventService eventService)
     {
          _eventService = eventService;
     }
     
     public async Task<IActionResult> Get(GetEventsDto getEventsDto)
     {
          Event[] events;
          if (getEventsDto.CategoryId is { } categoryId)
               events = await _eventService
                    .GetEventsByCategoryAsync(
                         categoryId, getEventsDto.Skip, getEventsDto.Count);
          else
               events = await _eventService
                    .GetEventsAsync(getEventsDto.Skip, getEventsDto.Count);

          return Ok(events);
     }
     
     [AdminLevelAuth(EUserPermissions.Host)]
     public async Task<IActionResult> Path(Event updatedEvent)
     {
          await _eventService.UpdateAsync(updatedEvent);
          return Ok();
     }
     
     [AdminLevelAuth(EUserPermissions.Host)]
     public async Task<IActionResult> Delete([FromQuery] int id)
     {
          await _eventService.DeleteEventAsync(id);
          return Ok();
     }
     
     [HttpPost("schedule")]
     [AdminLevelAuth(EUserPermissions.Host)]
     public async Task<IActionResult> AddSchedule(Schedule schedule)
     {
          await _eventService.AddSchedule(schedule);
          return Ok();
     }
     
     [HttpPatch("schedule")]
     [AdminLevelAuth(EUserPermissions.Host)]
     public async Task<IActionResult> UpdateSchedule(Schedule schedule)
     {
          await _eventService.UpdateSchedule(schedule);
          return Ok();
     }
     
     [HttpDelete("schedule")]
     [AdminLevelAuth(EUserPermissions.Host)]
     public async Task<IActionResult> RemoveSchedule([FromQuery] int id)
     {
          await _eventService.DeleteSchedule(id);
          return Ok();
     }
}