using RecetarioCafeteria.Back.Entity;
using RecetarioCafeteria.Back.Infrastructure;
using RecetarioCafeteria.Back.Models;
using RecetarioCafeteria.Back.Repositories.Recetas.Base;
using RecetarioCafeteria.Back.Seed;
using RecetarioCafeteria.Back.Services.Recetas;
using RecetarioCafeteria.Back.Validators.Common;
using RecetarioCafeteria.Back.Validators.Recetas;
using RecetarioCafeteria.Blazor.Components;
using MudBlazor.Services;
using Serilog;

// Logger de arranque: captura también los errores que puedan ocurrir antes de
// que WebApplication.CreateBuilder termine de construir el host.
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        Path.Combine(AppContext.BaseDirectory, "logs", "log-.txt"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30)
    .CreateLogger();

try
{
    Log.Information("Arrancando RecetarioCafeteria");

    var builder = WebApplication.CreateBuilder(args);

    // Serilog sustituye al logging por defecto; captura también los mensajes
    // de arranque/parada que ya emite ASP.NET Core (Microsoft.Hosting.Lifetime).
    builder.Host.UseSerilog();

    // Add services to the container.
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();
    builder.Services.AddMudServices();

    // Configurar la base de datos
    var connectionString = Environment.ExpandEnvironmentVariables(
        builder.Configuration.GetConnectionString("RecetarioDb")!);

    // Registrar dependencias del Back
    var provider = DependenciesProvider.BuildServiceProvider(connectionString);
    builder.Services.AddSingleton(provider.GetService<IRecetaRepository>()!);
    builder.Services.AddScoped<IRecetaService, RecetaService>();
    builder.Services.AddScoped<IValidador<Receta>, RecetaValidador>();

    var app = builder.Build();

    // Sembrar recetas de ejemplo si la base está vacía (no debe tumbar el arranque si falla)
    try
    {
        using var seedContext = new AppDbContext(connectionString);
        await RecetaSeeder.SeedAsync(seedContext);
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "No se pudo sembrar las recetas de ejemplo");
    }

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }
    app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
    app.UseHttpsRedirection();

    app.UseAntiforgery();

    app.MapStaticAssets();
    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "RecetarioCafeteria ha terminado de forma inesperada durante el arranque");
}
finally
{
    Log.Information("Cerrando RecetarioCafeteria");
    Log.CloseAndFlush();
}
