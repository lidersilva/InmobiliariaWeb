using Microsoft.AspNetCore.Components;
using eProduccion.Models;

namespace eProduccion.Components.Base
{
    public abstract class BasePermisoPage : ComponentBase
    {
        [Inject] protected UserSession UserSession { get; set; }
        [Inject] protected NavigationManager Navigation { get; set; }

        protected abstract string PermisoRequerido { get; }

        protected override void OnInitialized()
        {
            if (!UserSession.TienePermiso(PermisoRequerido))
            {
                Navigation.NavigateTo("/no-autorizado");
                return;
            }

            base.OnInitialized();
        }
    }
}
