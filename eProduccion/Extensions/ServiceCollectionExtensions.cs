using eProduccion.Data;
using eProduccion.Data.GestionAccesos;
using eProduccion.Data.GestionUsuarios;
using eProduccion.Models;
using eProduccion.Utility;
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
            services.AddScoped<PermisoService>();
            services.AddScoped<UsuarioRolService>();
            services.AddScoped<RolService>();

            // Utility
            services.AddScoped<DbHelper>();

            return services;
        }
    }
}
