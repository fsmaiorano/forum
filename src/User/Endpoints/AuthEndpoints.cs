using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using User.Data;
using User.Data.Models;
using User.DTOs;
using User.Services;

namespace User.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");
        group.WithTags("Auth");

        group.MapPost("/register", Register)
            .Produces<AuthResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapPost("/login", Login)
            .Produces<AuthResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/refresh", Refresh)
            .Produces<AuthResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapPost("/revoke", Revoke)
            .Produces(StatusCodes.Status204NoContent);

        group.MapGet("/me", Me)
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK);
    }

    private static async Task<IResult> Register(
        RegisterRequest dto,
        UserDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ITokenService tokenService,
        HttpContext http)
    {
        var isInMemory = dbContext.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory";
        var transaction = isInMemory ? null : await dbContext.Database.BeginTransactionAsync();

        try
        {
            var existing = await userManager.FindByEmailAsync(dto.Email);
            if (existing is not null)
                return Results.BadRequest(new { message = "E-mail already exists." });

            var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email };

            var identityResult = await userManager.CreateAsync(user, dto.Password);
            if (!identityResult.Succeeded)
                return Results.BadRequest(identityResult.Errors);

            if (!await roleManager.RoleExistsAsync("Member"))
                await roleManager.CreateAsync(new IdentityRole("Member"));

            var addToRoleResult = await userManager.AddToRoleAsync(user, "Member");
            if (!addToRoleResult.Succeeded)
                return Results.BadRequest(new
                {
                    message = "Failed to assign role to user",
                    errors = addToRoleResult.Errors
                });

            if (transaction is not null)
                await transaction.CommitAsync();

            var tokens = await tokenService.CreateTokensAsync(user, GetIpAddress(http));
            return Results.Ok(tokens);
        }
        finally
        {
            if (transaction is not null)
                await transaction.DisposeAsync();
        }
    }

    private static async Task<IResult> Login(
        AuthRequest dto,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        HttpContext http)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return Results.Unauthorized();

        var res = await signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!res.Succeeded)
            return Results.Unauthorized();

        var tokens = await tokenService.CreateTokensAsync(user, GetIpAddress(http));
        return Results.Ok(tokens);
    }

    private static async Task<IResult> Refresh(
        RefreshRequest dto,
        ITokenService tokenService,
        HttpContext http)
    {
        var response = await tokenService.RefreshAsync(dto.AccessToken, dto.RefreshToken, GetIpAddress(http));
        return response is null ? Results.BadRequest(new { message = "Invalid Token" }) : Results.Ok(response);
    }

    private static async Task<IResult> Revoke(
        RefreshRequest dto,
        ITokenService tokenService,
        HttpContext http)
    {
        await tokenService.RevokeRefreshTokenAsync(dto.RefreshToken, GetIpAddress(http));
        return Results.NoContent();
    }

    private static IResult Me(ClaimsPrincipal user)
    {
        var userId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var email = user.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
        return Results.Ok(new { userId, email });
    }

    private static string GetIpAddress(HttpContext http)
    {
        if (http.Request.Headers.TryGetValue("X-Forwarded-For", out var value))
            return value.ToString();
        return http.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}