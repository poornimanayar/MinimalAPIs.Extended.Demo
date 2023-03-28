using Microsoft.AspNetCore.Http.HttpResults;

namespace EventsApi.Structuring;

public static class RouteGroupBuilderExtensions
{
    public static RouteGroupBuilder MapEventsRouteGroup(this RouteGroupBuilder eventsGroup) 
    {
        //parameter binding inferred from IoCContainer
        eventsGroup.MapGet("/", (EventsRepository eventsRepository) =>
        {
            var events = eventsRepository.GetMinimalEvents();
            return Results.Ok(events); //returns an IResult, serialized to JSON by System.Text.Json
        });

        //inferred parameter binding for id
        eventsGroup.MapGet("/{id}", (int id, EventsRepository eventsRepository) =>
        {
            var minimalEvent = eventsRepository.GetById(id);
            return minimalEvent == null ? Results.NotFound() : Results.Ok(minimalEvent);

        }).WithName("geteventbyid");

        //parameter binding from request body
        //linkgenerator used to generate urls for your endpoints
        eventsGroup.MapPost("/", (MinimalEvent minimalEvent, EventsRepository eventsRepository, LinkGenerator linkGenerator) =>
        {
            var createdEvent = eventsRepository.AddEvent(minimalEvent);
            return createdEvent == null ? Results.StatusCode(StatusCodes.Status500InternalServerError)
            //use the linkgenerator to get endpoint url and pass in route values
            : Results.Created(linkGenerator.GetPathByName("geteventbyid", new RouteValueDictionary { { "id", createdEvent.Id } }), createdEvent);
        })
        .WithName("addevent");

        eventsGroup.MapPut("/{id}", (int id, MinimalEvent minimalEvent, EventsRepository eventsRepository) =>
        {
            var eventExist = eventsRepository.FindMinimalEvent(id);
            if (eventExist != null)
            {
                var updated = eventsRepository.UpdateMinimalEvent(minimalEvent);
                return Results.NoContent();
            }
            return Results.NotFound();
        })
        .WithName("updateevent");

        eventsGroup.MapDelete("/{id}", (int id, EventsRepository eventsRepository) =>
        {
            var eventExist = eventsRepository.CheckEventExists(id);
            if (eventExist)
            {
                eventsRepository.DeleteMinimalEvent(id);
                return Results.NoContent();
            }
            return Results.NotFound();
        })
        .WithName("deleteevent");

        //upload using IFormFile for single file, IFormFileCollection for multiple files
        //return typed results for testability, preserving endpoint metadata
        //generic union return for compile-time support
        eventsGroup.MapPost("/uploadimage/{id}", async Task<Results<NoContent, NotFound>>
            (int id, IFormFile imageFile, EventsRepository eventsRepository) =>
        {

            var eventExist = eventsRepository.FindMinimalEvent(id);
            if (eventExist != null)
            {
                await imageFile.CopyToAsync(File.OpenWrite(imageFile.FileName));
                var imagePath = $@"{Directory.GetCurrentDirectory()}\{imageFile.FileName}";
                eventsRepository.UploadImage(id, imagePath);
                return TypedResults.NoContent();
            }
            return TypedResults.NotFound();
        })
        .WithName("uploadeventimage");

        return eventsGroup;
    }


    public static RouteGroupBuilder MapPaidEventsRouteGroup(this RouteGroupBuilder paidGroup)
    {
        //events/filterbyid?ids=2&ids=4&ids=6
        paidGroup.MapGet("/filterbyid", (int[] ids, EventsRepository eventsRepository) =>
        {
            return TypedResults.Ok(eventsRepository.GetByIds(ids));
        })
        .WithName("filterbyid");

        paidGroup.MapGet("/eventcodesearch", Results<Ok<List<MinimalEvent>?>, NotFound> (string code, EventsRepository eventsRepository) =>
        {
            var events = eventsRepository.EventCodeSearch(code);
            return events != null && events.Any() ? TypedResults.Ok(events) : TypedResults.NotFound();
        })
        .WithName("eventcodesearch");

        return paidGroup;
    }
}
