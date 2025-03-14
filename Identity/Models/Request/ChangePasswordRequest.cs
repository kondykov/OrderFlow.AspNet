using System.ComponentModel.DataAnnotations;

namespace OrderFlow.Identity.Models.Request;

public class ChangePasswordRequest
{
    public string OldPassword { get; set; }
    public string Password { get; set; }
    [Compare("Password")]
    public string ConfirmPassword { get; set; }
}