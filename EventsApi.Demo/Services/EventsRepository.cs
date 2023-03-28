namespace EventsApi.Demo.Services;

public class EventsRepository
{
    private readonly MinimalEventsDbContext _minimalEventsDbContext;

    public EventsRepository(MinimalEventsDbContext minimalEventsDbContext)
    {
        _minimalEventsDbContext = minimalEventsDbContext;
    }

    public List<MinimalEvent> GetMinimalEvents() => _minimalEventsDbContext.MinimalEvents.ToList();

    public MinimalEvent? GetById(int id) => FindMinimalEvent(id);

    public MinimalEvent AddEvent(MinimalEvent minimalEvent)
    {
        _minimalEventsDbContext.MinimalEvents.Add(minimalEvent);
        _minimalEventsDbContext.SaveChanges();
        return minimalEvent;
    }

    public int UpdateMinimalEvent(MinimalEvent minimalEvent)
    {
        var currentEvent = FindMinimalEvent(minimalEvent.Id);
        if (currentEvent == null)
        {
            return default;
        }
        currentEvent.Name = minimalEvent.Name;
        currentEvent.Description = minimalEvent.Description;
        currentEvent.EventDate = minimalEvent.EventDate;
        currentEvent.Location = minimalEvent.Location;
        currentEvent.EventContact = minimalEvent.EventContact;
        currentEvent.EventCode = minimalEvent.EventCode;

        return _minimalEventsDbContext.SaveChanges();
    }

    public void DeleteMinimalEvent(int id)
    {
        _minimalEventsDbContext.Remove(FindMinimalEvent(id));
        _minimalEventsDbContext.SaveChanges();
    }

    public MinimalEvent? FindMinimalEvent(int id)
    {
        return _minimalEventsDbContext.MinimalEvents.Find(id);
    }

    public bool CheckEventExists(int id)
    {
        return _minimalEventsDbContext.MinimalEvents.Any(m => m.Id == id);
    }

    public void UploadImage(int id, string imagePath)
    {
        var minimalEvent = GetById(id);
        if (minimalEvent != null)
        {
            minimalEvent.EventImage = imagePath;
            _minimalEventsDbContext.Entry(minimalEvent).State = EntityState.Modified;
            _minimalEventsDbContext.SaveChanges();
        }
    }

    public List<MinimalEvent> GetByIds(int[] ids) => _minimalEventsDbContext.MinimalEvents.Where(e => ids.Contains(e.Id)).ToList();

    public List<MinimalEvent> EventCodeSearch(string searchTerm) => _minimalEventsDbContext.MinimalEvents.Where(e => e.EventCode == searchTerm).ToList();
}