using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using OrderFlow.Shared.Models.Ordering;

namespace OrderFlow.Shared.Models.Identity.Devices;

public class Device 
{
    [Key] public int Id { get; set; }
    public string Name { get; set; }
    public DeviceType DeviceType { get; set; } = DeviceType.Terminal;
}