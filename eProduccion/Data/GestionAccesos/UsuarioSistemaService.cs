using RestSharp;
using System.Data.Odbc;
using eProduccion.Models;
using eProduccion.Utility;
using static eProduccion.Models.UserFieldsMD;

namespace eProduccion.Data.GestionAccesos
{
    public class UsuarioSistemaService
    {
        private readonly ConnectionService _connectionService;
        private readonly EstructuraService _estructuraService;

        public UsuarioSistemaService(ConnectionService connectionService, EstructuraService estructuraService)
        {
            _connectionService = connectionService;
            _estructuraService = estructuraService;
        }

        public void VerificarUsuarioExistente()
        {
            var appSettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var verificarEstructuraIni = appSettings["AppSettings:verificarEstructuraInicial"];

            if (verificarEstructuraIni != "N")
                // La primera vez que el usuario inice sesión no existirá la estructura correspondiente, por lo que se debe crear en el momento
                CrearEstructuraUsuario();

            VerificarCredenciales();
        }

        public void CrearEstructuraUsuario()
        {
            // Crear tabla Usuario
            List<UserTablesMD> tablas = new()
            {
                new UserTablesMD() {TableName = "EEP_USUA", Descr = "Usuarios eProduccion", ObjectType = 5}
            };

            foreach (var i in tablas)
            {
                _estructuraService.AgregarTablas(i);
            }

            // Crear campos Usuario
            var valoresValidos = new List<ValidValuesMD>();// Lista vacía

            List<UserFieldsMD> campos = new()
            {
                new UserFieldsMD() { Name = "CODE", Type = TipoCampo.Alpha, Size = 25, Description = "Codigo Usuario", SubType = SubTipoCampo.None, TableName = "@EEP_USUA", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidos, LinkedSystemObject = null },
                new UserFieldsMD() { Name = "PASS", Type = TipoCampo.Alpha, Size = 254, Description = "Contraseña Usuario", SubType = SubTipoCampo.None, TableName = "@EEP_USUA", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidos, LinkedSystemObject = null },
                new UserFieldsMD() { Name = "MAIL", Type = TipoCampo.Alpha, Size = 50, Description = "Correo Usuario", SubType = SubTipoCampo.None, TableName = "@EEP_USUA", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidos, LinkedSystemObject = null },
                new UserFieldsMD() { Name = "FCRE", Type = TipoCampo.Date, Size = 10, Description = "Fecha creación Usuario", SubType = SubTipoCampo.None, TableName = "@EEP_USUA", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidos, LinkedSystemObject = null },
                new UserFieldsMD() { Name = "FACT", Type = TipoCampo.Date, Size = 10, Description = "Fecha actualización Usuario", SubType = SubTipoCampo.None, TableName = "@EEP_USUA", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidos, LinkedSystemObject = null },
                new UserFieldsMD() { Name = "HCRE", Type = TipoCampo.Date, Size = 10, Description = "Hora creación Usuario", SubType = SubTipoCampo.Time, TableName = "@EEP_USUA", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidos, LinkedSystemObject = null },
                new UserFieldsMD() { Name = "HACT", Type = TipoCampo.Date, Size = 10, Description = "Hora actualización Usuario", SubType = SubTipoCampo.Time, TableName = "@EEP_USUA", LinkedTable = null, DefaultValue = null , ValidValuesMD = valoresValidos, LinkedSystemObject = null },
                new UserFieldsMD() { Name = "ACTI", Type = TipoCampo.Alpha, Size = 1, Description = "Usuario Activo", SubType = SubTipoCampo.None, TableName = "@EEP_USUA", LinkedTable = null, DefaultValue = "Y", ValidValuesMD = valoresValidos, LinkedSystemObject = null }
            };

            foreach (var i in campos)
            {
                _estructuraService.AgregarCampos(i);
            }

            // Agregar registro del primer usuario del sistema
            if (!ExisteUsuario("EEP_ADMIN"))
            {
                var method = Method.Post;
                var entity = $"U_EEP_USUA";
                var fechaActual = DateTime.Now;

                dynamic body = new
                {
                    Name = "Usuario Administrador EEP",
                    U_CODE = "EEP_ADMIN",
                    U_PASS = "P5pkPtOkFmQDniv3CxEzWw==",
                    U_MAIL = "",
                    U_FCRE = fechaActual.ToString("yyyyMMdd"),
                    U_FACT = fechaActual.ToString("yyyyMMdd"),
                    U_HCRE = fechaActual.ToString("HHmm"),
                    U_HACT = fechaActual.ToString("HHmm")
                };

                var jObject = _connectionService.SetEntitySL(method, entity, body);
            }
        }

        public bool ExisteUsuario(string codeUsuario)
        {
            bool existe = false;

            var query = $"SELECT \"U_CODE\" FROM \"{_connectionService.DataBase}\".\"@EEP_USUA\" WHERE \"U_CODE\"='{codeUsuario}' ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                existe = true;
            }

            _connectionService.DisconnectODBC();

            return existe;
        }

        public void VerificarCredenciales()
        {
            bool existe = false;

            var query = $"SELECT \"U_CODE\" FROM \"{_connectionService.DataBase}\".\"@EEP_USUA\" WHERE \"U_CODE\"='{_connectionService.UserName}' AND \"U_PASS\"='{_connectionService.PassSecure}' AND \"U_ACTI\"='Y' ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                existe = true;
            }

            _connectionService.DisconnectODBC();

            if (!existe)
                throw new Exception("Credenciales incorrectas. Verifique.");
        }

        public Task<Usuario[]> ObtenerUsuarios()
        {
            var list = new List<Usuario>();

            var query = $"SELECT \"Code\", \"U_CODE\", \"Name\", \"U_MAIL\", \"U_ACTI\" FROM \"{_connectionService.DataBase}\".\"@EEP_USUA\" WHERE \"U_CODE\"!='EEP_ADMIN' ORDER BY \"U_CODE\" ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new Usuario
                {
                    Code = int.Parse(reader["Code"].ToString()),
                    CodigoUsuario = reader["U_CODE"].ToString(),
                    NombreUsuario = reader["Name"].ToString(),
                    Email = reader["U_MAIL"].ToString(),
                    Estado = reader["U_ACTI"].ToString()
                };

                list.Add(che);
            }

            _connectionService.DisconnectODBC();

            return Task.FromResult(list.ToArray());
        }

        public async Task GuardarUsuario(Usuario usuario)
        {
            var existeusuario = ExisteUsuario(usuario.CodigoUsuario);
            if (existeusuario)
            {
                throw new Exception($"El código de usuario {usuario.CodigoUsuario} ya existe");
            }
            else
            {
                var method = Method.Post;
                var entity = $"U_EEP_USUA";
                var fechaActual = DateTime.Now;

                var body = new
                {
                    Name = usuario.NombreUsuario,
                    U_CODE = usuario.CodigoUsuario,
                    U_PASS = Encryption.EncryptString(usuario.Password),
                    U_MAIL = usuario.Email,
                    U_FCRE = fechaActual.ToString("yyyyMMdd"),
                    U_FACT = fechaActual.ToString("yyyyMMdd"),
                    U_HCRE = fechaActual.ToString("HHmm"),
                    U_HACT = fechaActual.ToString("HHmm"),
                };

                _connectionService.SetEntitySL(method, entity, body);
            }
        }

        public void CambiarPassUsuario(Usuario usuario)
        {
            var method = Method.Patch;
            var entity = $"U_EEP_USUA({usuario.Code})";
            var fechaActual = DateTime.Now;

            var body = new
            {
                U_PASS = Encryption.EncryptString(usuario.Password),
                U_FACT = fechaActual.ToString("yyyyMMdd"),
                U_HACT = fechaActual.ToString("HHmm")
            };

            _connectionService.SetEntitySL(method, entity, body);
        }

        public void CambiarPassUsuarioLayout(Usuario usuario)
        {
            if (usuario.Password != Encryption.DecryptString(_connectionService.PassSecure))
                throw new Exception("La contraseña ingresada no es válida. Verifique.");

            int codeUsuario = ObtenerCodeUsuario(_connectionService.UserName);

            var method = Method.Patch;
            var entity = $"U_EEP_USUA({codeUsuario})";
            var fechaActual = DateTime.Now;

            var body = new
            {
                U_PASS = Encryption.EncryptString(usuario.NewPassword),
                U_FACT = fechaActual.ToString("yyyyMMdd"),
                U_HACT = fechaActual.ToString("HHmm")
            };

            _connectionService.SetEntitySL(method, entity, body);
        }

        public async Task EditarUsuario(Usuario usuario)
        {
            var method = Method.Patch;
            var entity = $"U_EEP_USUA({usuario.Code})";
            var fechaActual = DateTime.Now;

            var body = new
            {
                Name = usuario.NombreUsuario,
                U_MAIL = usuario.Email,
                U_FACT = fechaActual.ToString("yyyyMMdd"),
                U_HACT = fechaActual.ToString("HHmm"),
                U_ACTI = usuario.Estado
            };

            _connectionService.SetEntitySL(method, entity, body);
        }

        public async Task EliminarUsuario(int code)
        {
            var method = Method.Delete;
            var entity = $"U_EEP_USUA({code})";

            var body = new { };

            _connectionService.SetEntitySL(method, entity, body);
        }

        public int ObtenerCodeUsuario(string user)
        {
            int code = 0;

            var query = $"SELECT \"Code\" FROM \"{_connectionService.DataBase}\".\"@EEP_USUA\" WHERE \"U_CODE\"='{user}' AND \"U_ACTI\"='Y' ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                code = int.Parse(reader["Code"].ToString());
            }

            _connectionService.DisconnectODBC();

            return code;
        }
    }
}
