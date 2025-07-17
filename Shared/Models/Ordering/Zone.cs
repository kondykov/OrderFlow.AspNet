using System.ComponentModel.DataAnnotations;

namespace OrderFlow.Shared.Models.Ordering;

public class Zone
{
    [Key] public int Id { get; set; }
    public string Name { get; set; }
}