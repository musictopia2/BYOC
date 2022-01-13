
using BYOC.Server.Data;
using BYOC.ServerLibrary.Areas.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;

namespace BYOC.ServerLibrary;
public static class Extensions
{
    public static IServiceCollection RegisterBYOCBasicServices(this IServiceCollection services)
    {
        // Add services to the container.
        services.AddDatabaseDeveloperPageExceptionFilter()
            .AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationDbContext>();
        services.AddRazorPages();
        services.AddServerSideBlazor();
        services
            .AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
        return services;
    }
    public static IServiceCollection RegisterBYOCDataServices(this IServiceCollection services)
    {
        // Placeholder for adding game stuff

        services.AddSingleton<IWorld, World>(e => new World())
            .AddSingleton<ITileRepository, TileRepository>()
            .AddSingleton<IUnitRepository, UnitRepository>()

        // Data access and logic
        .AddSingleton<IWorldService, WorldService>()
        .AddSingleton<ITileService, TileService>()
        .AddSingleton<IUnitService, UnitService>()

        // commands - typically player specific
        .AddSingleton<IUnitController, UnitController>()
        .AddSingleton<IGameController, GameController>()

        // current user information, singleton for testing until hooked into the lifecycle
        .AddSingleton<ISessionService, SessionService>()

        // ~~~~~~~~~~~~~~

        .AddResponseCompression(opts =>
        {
            opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                new[] { "application/octet-stream" });
        });
        return services;
    }
    
    public static WebApplication RegisterBYOCServices(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapBlazorHub();

        app.MapHub<DemoHub>("/demo");

        app.MapFallbackToPage("/_Host");

        app.Services.GetService<ITileService>()?.Seed(30, 30);
        var testplayer = app.Services.GetService<IWorldService>()?.AddPlayer(new Player());
        Unit? unit = app.Services.GetService<IUnitRepository>()?.AddUnit(testplayer.Id, 5, 5)!;
        MapVisualizer.DrawToConsole(app.Services.GetService<IWorld>()!);
        app.Services.GetService<IGameController>()?.Start();
        app.Services.GetService<IUnitController>()?.TryMoveUnit(unit.Id, 29, 29);
        return app;
    }
}