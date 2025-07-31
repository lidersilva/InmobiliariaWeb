using eProduccion.Data;
using eProduccion.Data.GestionUsuarios;
using eProduccion.Models;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace eProduccion.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddScoped<ProtectedSessionStorage>();

            services.AddScoped<ConnectionService>();
            services.AddScoped<UserSession>();
            services.AddScoped<EstructuraService>();

            // Gestión Usuarios
            services.AddScoped<UsuarioSistemaService>();

            return services;
        }
    }
}
