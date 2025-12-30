using Microsoft.AspNetCore.Components;
using eProduccion.Models;

namespace eProduccion.Base
{
    public abstract class PageBasePermiso : ComponentBase
    {
        [Inject] protected UserSession UserSession { get; set; }
        [Inject] protected NavigationManager Navigation { get; set; }

        protected abstract string PermisoRequerido { get; }

        protected override void OnInitialized()
        {
            if (!UserSession.TienePermiso(PermisoRequerido))
            {
                Navigation.NavigateTo("/no-autorizado", true);
                return;
            }

            base.OnInitialized();
        }
    }
}
