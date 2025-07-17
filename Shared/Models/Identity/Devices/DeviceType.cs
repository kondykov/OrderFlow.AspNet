using System.ComponentModel;

namespace OrderFlow.Shared.Models.Identity.Devices;

public enum DeviceType
{
    [Description("Терминал")] Terminal,
    [Description("Киоск")] Kiosk,
}