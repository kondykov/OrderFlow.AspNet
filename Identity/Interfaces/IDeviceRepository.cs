using OrderFlow.Identity.Models.Request;
using OrderFlow.Shared.Models;
using OrderFlow.Shared.Models.Identity.Devices;

namespace OrderFlow.Identity.Interfaces;

public interface IDeviceRepository
{
    Task Save(Device device);
    Task Update(Device device);
    Task Delete(Device device);
    Task<Device?> FindById(int deviceId);
    Task<PaginationResponse<List<Device>>> Get(PageQuery pageQuery);
}