using OrderFlow.Shared.Models.Identity;

namespace OrderFlow.Identity.Interfaces;

public interface ICurrentUserService
{
    Task<User> GetCurrentUserAsync();
    Task<User> GetCurrentUserInfoAsync();
}