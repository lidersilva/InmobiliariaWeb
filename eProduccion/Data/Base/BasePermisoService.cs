using eProduccion.Models;

namespace eProduccion.Data.Base
{
    public abstract class BasePermisoService(UserSession userSession)
    {
        protected readonly UserSession _userSession = userSession;

        protected void RequierePermiso(string permiso)
        {
            if (!userSession.TienePermiso(permiso))
                throw new UnauthorizedAccessException("Acceso no autorizado.");
        }
    }
}
