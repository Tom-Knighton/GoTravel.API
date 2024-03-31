using Microsoft.AspNetCore.Http;
using StackExchange.Redis;

namespace GoTravel.API.Domain.Middleware;

public class IdempotencyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IDatabase _cache;
    private readonly double _ttl;

    public IdempotencyMiddleware(RequestDelegate next, IDatabase db, double ttl)
    {
        _next = next;
        _cache = db;
        _ttl = ttl;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("Idempotency-Key", out var idempotencyKey))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Supply an idempotency key in the header 'Idempotency-Key'");
            return;
        }

        var keyExists = await _cache.KeyExistsAsync(idempotencyKey.FirstOrDefault());
        if (keyExists)
        {
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            await context.Response.WriteAsync("Request with this Idempotency key has already been processed.");
            return;
        }

        await _cache.StringSetAsync(idempotencyKey.FirstOrDefault(), "", TimeSpan.FromMinutes(10)); // Setting a TTL for key
        await _next(context);
    }
}