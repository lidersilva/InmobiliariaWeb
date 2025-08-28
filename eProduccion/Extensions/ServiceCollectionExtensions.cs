using eProduccion.Data;
using eProduccion.Data.Configuracion;
using eProduccion.Data.GestionAccesos;
using eProduccion.Data.Produccion;
using eProduccion.Integration;
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

            // Configuración
            services.AddScoped<ParametrizacionService>();

            // Producción
            services.AddScoped<PlanificacionOTService>();
            services.AddScoped<InyeccionService>();

            // Utility
            services.AddScoped<DbHelper>();

            // Integration
            services.AddScoped<SBOIntegration>();

            return services;
        }
    }
}
