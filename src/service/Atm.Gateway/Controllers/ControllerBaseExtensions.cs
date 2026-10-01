using Microsoft.AspNetCore.Mvc;

namespace Atm.Gateway.Controllers;

public static class ControllerBaseExtensions
{
    public static string GetUserId(this ControllerBase controller)
    {
        return controller.User.FindFirst("sub")?.Value
            ?? throw new InvalidOperationException("The request is missing the 'sub' claim");
    }
}
