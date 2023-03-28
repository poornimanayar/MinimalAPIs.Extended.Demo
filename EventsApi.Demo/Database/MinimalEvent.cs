using System.ComponentModel.DataAnnotations.Schema;

namespace EventsApi.Demo.Database;

public class MinimalEvent
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public DateTime EventDate { get; set; }
    public decimal Cost { get; set; }
    public string? EventContact { get; set; }
    public string? EventCode { get; set; }
    public string? EventImage { get; set; }
}
