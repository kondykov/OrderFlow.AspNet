using OrderFlow.Identity.Interfaces;
using OrderFlow.Identity.Models.Request;
using OrderFlow.Shared.Exceptions;
using OrderFlow.Shared.Models;
using OrderFlow.Shared.Models.Identity.Devices;

namespace OrderFlow.Identity.Services;

public class DeviceService(
    IDeviceRepository repository
) : IDeviceService
{
    public async Task<Device> Add(RegisterDeviceRequest request)
    {
        var device = new Device
        {
            Name = request.Title,
            DeviceType = DeviceType.Terminal
        };

        await repository.Save(device);
        return device;
    }

    public async Task Remove(int deviceId)
    {
        var device = await GetById(deviceId);
        await repository.Delete(device);
    }

    public async Task<Device?> FindById(int id)
    {
        return await repository.FindById(id);
    }

    public async Task<Device> GetById(int id)
    {
        var device = await repository.FindById(id);
        if (device is null) throw new EntityNotFoundException("Устройство не найдено");
        return device;
    }

    public async Task<PaginationResponse<List<Device>>> Get(PageQuery query)
    {
        return await repository.Get(query);
    }
}