using Microsoft.Extensions.DependencyInjection;
using eProduccion.Data;
using eProduccion.Models;
using eProduccion.Data.GestionUsuarios;

namespace eProduccion.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddScoped<ConnectionService>();
            services.AddScoped<UserSession>();
            services.AddScoped<EstructuraService>();

            // Gestión Usuarios
            services.AddScoped<UsuarioSistemaService>();

            return services;
        }
    }
}
