using System.Reflection;
using System.Security.Claims;

namespace OrderFlow.Shared.Models.Identity;

public static class SystemClaims
{
    // ordering
    public static Claim CanEditOrder { get; } = new("Permission", "CanEditOrder");
    public static Claim CanGetOrder { get; } = new("Permission", "CanGetOrder");
    
    public static Claim CanCreateProduct { get; } = new("Permission", "CanCreateProduct");
    public static Claim CanEditProduct { get; } = new("Permission", "CanEditProduct");
    public static Claim CanDeleteProduct { get; } = new("Permission", "CanDeleteProduct");
    
    // identity
    public static Claim CanChangeRole { get; } = new("Permission", "CanChangeRole");
    public static bool TryGet(string claimType, out Claim? claim)
    {
        claim = null;
        var propertyName = claimType;
        var staticClaimsType = typeof(SystemClaims);
        var propertyInfo = staticClaimsType
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(p => p.Name == propertyName);

        if (propertyInfo == null) return false;
        claim = (Claim)propertyInfo.GetValue(null)!;
        return true;
    }
}