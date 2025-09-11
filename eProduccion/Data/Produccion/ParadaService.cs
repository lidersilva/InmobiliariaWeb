using eProduccion.Models;
using RestSharp;
using System.Data.Odbc;
using System.Globalization;

namespace eProduccion.Data.Produccion
{
    public class ParadaService(ConnectionService connectionService)
    {
        private readonly ConnectionService _connectionService = connectionService;

        public async Task RegistrarParada(Parada parada, string estacion, int docEntryOT, int lineId, string operario1, string operario2)
        {
            var fechaActual = DateTime.Now;
            var estado = parada.DocEntry == 0 ? "Iniciado" : "Detenido";
            var horaInicio = estado == "Iniciado" ? fechaActual.ToString("HHmm") : parada.HoraInicio.ToString("HHmm");
            var horaFin = estado == "Detenido" ? fechaActual.ToString("HHmm") : parada.HoraFin.ToString("HHmm");

            var method = estado == "Iniciado" ? Method.Post : Method.Patch;
            var entity = estado == "Iniciado" ? $"EEP_PARADAS" : $"EEP_PARADAS({parada.DocEntry})";

            var body = new
            {
                U_ESTACION = estacion,
                U_OT = docEntryOT,
                U_LINEIDOT = lineId,
                U_FECHA = parada.Fecha,
                U_TIPOPARO = parada.TipoParada,
                U_TURNO = parada.Turno,
                U_OPERADOR1 = operario1,
                U_OPERADOR2 = operario2,
                U_HORAINI = horaInicio,
                U_HORAFIN = horaFin,
                U_NROMAQUI = parada.NroMaquina,
                U_ESTADO = estado,
            };

            _connectionService.SetEntitySL(method, entity, body);
        }

        public Task<List<Parada>> ObtenerRegistroParadas(int docEntryOT, int lineIdOT, string estacion)
        {
            var list = new List<Parada>();

            var query = $@"
                SELECT 
                    TP.""DocEntry"", 
                    TP.""U_FECHA"", 
                    TP.""U_TIPOPARO"", 
                    TP.""U_TURNO"", 
                    TP.""U_HORAINI"", 
                    TP.""U_HORAFIN"", 
                    TP.""U_NROMAQUI"",
                    TP.""U_ESTADO""
                FROM ""{_connectionService.DataBase}"".""@EEP_PARADAS"" TP
                WHERE TP.""U_ESTACION""='{estacion}'
                AND TP.""U_OT""={docEntryOT}
                AND TP.""U_LINEIDOT""={lineIdOT}
                ORDER BY TP.""DocEntry"" ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new Parada();
                che.DocEntry = int.Parse(reader["DocEntry"].ToString());
                che.Fecha = DateTime.Parse(reader["U_FECHA"].ToString());
                che.TipoParada = reader["U_TIPOPARO"].ToString();
                che.Turno = reader["U_TURNO"].ToString();
                var horaInicioString = reader["U_HORAINI"].ToString().PadLeft(4, '0');
                if (!string.IsNullOrWhiteSpace(horaInicioString) && horaInicioString.Length == 4)
                    che.HoraInicio = DateTime.ParseExact(horaInicioString, "HHmm", CultureInfo.InvariantCulture);
                var horaFinString = reader["U_HORAFIN"].ToString().PadLeft(4, '0');
                if (!string.IsNullOrWhiteSpace(horaFinString) && horaFinString.Length == 4)
                    che.HoraFin = DateTime.ParseExact(horaFinString, "HHmm", CultureInfo.InvariantCulture);
                che.NroMaquina = reader["U_NROMAQUI"].ToString();
                che.Estado = reader["U_ESTADO"].ToString();
                list.Add(che);
            }

            _connectionService.DisconnectODBC();

            return Task.FromResult(list);
        }
    }
}
