using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace GoTravel.API.Domain.Extensions;

public static class HttpContextExtensions
{
    public static string CurrentUserId(this ClaimsPrincipal context)
    {
        return context.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}