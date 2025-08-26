using MudBlazor;

namespace eProduccion.Utility
{
    public static class PageHelper
    {
        public static Color DefinirColorEstado(string estado)
        {
            return estado switch
            {
                "Pendiente" => Color.Default,
                "En proceso" => Color.Info,
                "Finalizado" => Color.Success,
                "Anulado" => Color.Dark,
                _ => Color.Transparent
            };
        }
    }
}
