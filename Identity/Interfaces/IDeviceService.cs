using OrderFlow.Identity.Models.Request;
using OrderFlow.Identity.Models.Response;
using OrderFlow.Shared.Models;
using OrderFlow.Shared.Models.Identity.Devices;

namespace OrderFlow.Identity.Interfaces;

public interface IDeviceService
{
    Task<Device> Add(RegisterDeviceRequest request);
    Task Remove(int deviceId);
    Task<Device?> FindById(int id);

    Task<Device> GetById(int id);
    Task<PaginationResponse<List<Device>>> Get(PageQuery query);
}