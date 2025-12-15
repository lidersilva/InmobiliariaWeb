using MudBlazor;

namespace eProduccion.Utility
{
    public static class PageHelper
    {
        public static MudBlazor.Color DefinirColorEstado(string estado)
        {
            return estado switch
            {
                "Pendiente" => MudBlazor.Color.Default,
                "En proceso" => MudBlazor.Color.Info,
                "Finalizado" => MudBlazor.Color.Success,
                "Anulado" => MudBlazor.Color.Dark,
                _ => MudBlazor.Color.Transparent
            };
        }
    }
}
