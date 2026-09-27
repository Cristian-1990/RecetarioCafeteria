using RecetarioCafeteria.Back.Models;
using RecetarioCafeteria.Back.Repositories.Presentaciones.Base;
using RecetarioCafeteria.Back.Repositories.Presentaciones.EfCore;
using RecetarioCafeteria.Back.Repositories.Recetas.Base;
using RecetarioCafeteria.Back.Repositories.Recetas.EfCore;
using RecetarioCafeteria.Back.Services.Presentaciones;
using RecetarioCafeteria.Back.Services.Recetas;
using RecetarioCafeteria.Back.Validators.Common;
using RecetarioCafeteria.Back.Validators.Presentaciones;
using RecetarioCafeteria.Back.Validators.Recetas;
using Microsoft.Extensions.DependencyInjection;

namespace RecetarioCafeteria.Back.Infrastructure;

public static class DependenciesProvider
{
    public static IServiceProvider BuildServiceProvider(string connectionString)
    {
        var services = new ServiceCollection();

        RegisterValidators(services);
        RegisterRepositories(services, connectionString);
        RegisterServices(services);

        return services.BuildServiceProvider();
    }

    private static void RegisterValidators(IServiceCollection services)
    {
        services.AddTransient<IValidador<Receta>, RecetaValidador>();
        services.AddTransient<IValidador<Presentacion>, PresentacionValidador>();
    }

    private static void RegisterRepositories(IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IRecetaRepository>(sp =>
            new RecetaEfRepository(connectionString));
        services.AddSingleton<IPresentacionRepository>(sp =>
            new PresentacionEfRepository(connectionString));
    }

    private static void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<IRecetaService, RecetaService>();
        services.AddScoped<IPresentacionService, PresentacionService>();
    }
}
