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

var builder = WebApplication.CreateBuilder(args);

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
