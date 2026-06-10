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
                 new UserTablesMD() { TableName = "EMUSUA", Descr = "Usuarios", ObjectType = 5 }
            };

            foreach (var i in tablas)
            {
                _estructuraService.AgregarTablas(i);
            }

            // Crear campos Usuario
            var valoresVacios = new List<ValidValuesMD>(); // Lista vacía para campos sin valores válidos
            var valoresSuperusuario = new List<ValidValuesMD>
            {
                new ValidValuesMD() { Value = "SI", Description = "Sí" },
                new ValidValuesMD() { Value = "NO", Description = "No" }
            };

            List<UserFieldsMD> campos = new()
            {
                new UserFieldsMD() { Name = "EXUCODE", Type = TipoCampo.Alpha, Size = 10, Description = "Cód. Usuario", SubType = SubTipoCampo.None, TableName = "@EMUSUA", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresVacios, LinkedSystemObject = null },
                new UserFieldsMD() { Name = "EXNCODE", Type = TipoCampo.Alpha, Size = 50, Description = "Nombre Usuario", SubType = SubTipoCampo.None, TableName = "@EMUSUA", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresVacios, LinkedSystemObject = null },
                new UserFieldsMD() { Name = "EXCSUC", Type = TipoCampo.Numeric, Size = 11, Description = "Cód. Sucursal", SubType = SubTipoCampo.None, TableName = "@EMUSUA", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresVacios, LinkedSystemObject = null },
                new UserFieldsMD() { Name = "EXNSUC", Type = TipoCampo.Alpha, Size = 30, Description = "Nombre Sucursal", SubType = SubTipoCampo.None, TableName = "@EMUSUA", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresVacios, LinkedSystemObject = null },
                new UserFieldsMD() { Name = "EXTUSU", Type = TipoCampo.Alpha, Size = 2, Description = "Superusuario", SubType = SubTipoCampo.None, TableName = "@EMUSUA", LinkedTable = null, DefaultValue = "NO", ValidValuesMD = valoresSuperusuario, LinkedSystemObject = null },
                //new UserFieldsMD() { Name = "ACTI", Type = TipoCampo.Alpha, Size = 1, Description = "Usuario Activo", SubType = SubTipoCampo.None, TableName = "@EMUSUA", LinkedTable = null, DefaultValue = "SI", ValidValuesMD = valoresSuperusuario, LinkedSystemObject = null } ,
                //new UserFieldsMD() { Name = "MAIL", Type = TipoCampo.Alpha, Size = 50, Description = "Correo Usuario", SubType = SubTipoCampo.None, TableName = "@EMUSUA", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresVacios, LinkedSystemObject = null },
                new UserFieldsMD() { Name = "EXCLAVE", Type = TipoCampo.Alpha, Size = 100, Description = "Clave de acceso", SubType = SubTipoCampo.None, TableName = "@EMUSUA", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresVacios, LinkedSystemObject = null }
            };

            foreach (var i in campos)
            {
                _estructuraService.AgregarCampos(i);
            }

            // Agregar registro del primer usuario administrador del sistema
            if (!ExisteUsuario("manager"))
            {
                var method = Method.Post;
                var entity = $"U_EMUSUA";   // nombre correcto de la tabla
                var fechaActual = DateTime.Now;

                dynamic body = new
                {
                    U_EXUCODE = "manager",                // Código de Usuario
                    U_EXNCODE = "Usuario Administrador",    // Nombre Usuario
                    U_EXCSUC = 0,                           // Código Sucursal (ejemplo vacío)
                    U_EXNSUC = "",                          // Nombre Sucursal (ejemplo vacío)
                    U_EXTUSU = "SI",                        // Superusuario
                    U_EXCLAVE = "hEyrr2banueW67WOuteQI4AmHS0PpolV", // Clave de acceso (encriptada)
                    //U_ACTI = "SI",
                    //U_MAIL = "lider.silva@exxis-group.com"

                };

                var jObject = _connectionService.SetEntitySL(method, entity, body);
            }
        }

        public bool ExisteUsuario(string codeUsuario)
        {
            bool existe = false;

            var query = $"SELECT \"U_EXNCODE\" FROM \"{_connectionService.DataBase}\".\"@EMUSUA\" WHERE \"U_EXNCODE\"='{codeUsuario}' ";

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

            var query = $"SELECT \"U_EXNCODE\" FROM \"{_connectionService.DataBase}\".\"@EMUSUA\" WHERE \"U_EXUCODE\"='{_connectionService.UserName}' AND \"U_EXCLAVE\"='{_connectionService.PassSecure}' AND \"U_EXTUSU\"='SI' ";

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

            var query = $"SELECT \"Code\", \"U_EXUCODE\", \"U_EXNCODE\", \"U_MAIL\", \"U_EXNSUC\", \"U_EXTUSU\" FROM \"{_connectionService.DataBase}\".\"@EMUSUA\" ORDER BY \"U_EXUCODE\" ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new Usuario
                {
                    Code = int.Parse(reader["Code"].ToString()),
                    CodigoUsuario = reader["U_EXUCODE"].ToString(),
                    NombreUsuario = reader["U_EXNCODE"].ToString(),
                    Email = reader["U_MAIL"].ToString(),
                    Estado = reader["U_EXTUSU"].ToString(), //hay que crear campo para usuario actibvo
                    TipoUsuario = reader["U_EXTUSU"].ToString(),
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
                    U_TIPO = usuario.TipoUsuario,
                };

                _connectionService.SetEntitySL(method, entity, body);
            }
        }

        public async Task CambiarPassUsuario(Usuario usuario)
        {
            var method = Method.Patch;
            var entity = $"U_EMUSUA({usuario.Code})";
            //var fechaActual = DateTime.Now;

            var body = new
            {
                U_EXCLAVE = Encryption.EncryptString(usuario.Password)
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
            var entity = $"U_EMUSUA({usuario.Code})";
            var fechaActual = DateTime.Now;

            var body = new
            {
                Name = usuario.NombreUsuario,
                U_MAIL = usuario.Email,
                U_FACT = fechaActual.ToString("yyyyMMdd"),
                U_HACT = fechaActual.ToString("HHmm"),
                U_ACTI = usuario.Estado,
                U_TIPO = usuario.TipoUsuario,
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
