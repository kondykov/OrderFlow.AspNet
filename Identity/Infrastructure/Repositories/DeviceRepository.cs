using Microsoft.EntityFrameworkCore;
using OrderFlow.Identity.Interfaces;
using OrderFlow.Shared.Exceptions;
using OrderFlow.Shared.Infrastructure.Data;
using OrderFlow.Shared.Models;
using OrderFlow.Shared.Models.Identity.Devices;

namespace OrderFlow.Identity.Infrastructure.Repositories;

public class DeviceRepository(DataContext context) : IDeviceRepository
{
    public async Task Save(Device device)
    {
        context.Devices.Add(device);
        await context.SaveChangesAsync();
    }

    public async Task Update(Device device)
    {
        var deviceExists = context.Devices.FirstOrDefault(o => o.Id == device.Id);
        if (deviceExists == null) throw new EntityNotFoundException($"Устройство с идентификатором {device.Id} не найден");
        context.Entry(deviceExists).CurrentValues.SetValues(device);
        await context.SaveChangesAsync();
    }

    public async Task Delete(Device device)
    {
        if (!context.Devices.Any(d => d.Id == device.Id)) throw new EntityNotFoundException("Устройство не существует");
        context.Devices.Remove(device);
        await context.SaveChangesAsync();
    }

    public async Task<Device?> FindById(int deviceId)
    {
        return await context.Devices.FindAsync(deviceId);
    }

    public async Task<PaginationResponse<List<Device>>> Get(PageQuery pageQuery)
    {
        var query = context.Devices.AsQueryable();
        var totalCount = await query.CountAsync();
        var devices = await query
            .OrderByDescending(p => p.Id)
            .Skip((pageQuery.PageNumber - 1) * pageQuery.PageSize)
            .Take(pageQuery.PageSize)
            .ToListAsync();

        return  new PaginationResponse<List<Device>>
            {
                Data = devices,
                Page = pageQuery.PageNumber,
                PageSize = pageQuery.PageSize,
                Pages = totalCount,
        };
    }
}