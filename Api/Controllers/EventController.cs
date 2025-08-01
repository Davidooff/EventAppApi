using Application.DTOs;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

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
}