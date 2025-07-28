using eProduccion.Models;
using eProduccion.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Data.Odbc;
using System.Net;

namespace eProduccion.Data
{
    public class ConnectionService
    {
        public OdbcConnection OdbcConn;
        public static NumberFormat NumberFormat;
        private readonly UserSession _userSession;

        public static string Integration;
        public static string SQLType;
        public static string B1Version;
        public static string Server;
        public static string Host;
        public static string Port;
        public static int SessionTimeout;

        public static string UserDB;
        public static string PassDB;

        public string DataBase => _userSession.DataBase;

        public ConnectionService(UserSession userSession)
        {
            _userSession = userSession;
        }

        public void GetAppSettings()
        {
            var appSettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            if (string.IsNullOrEmpty(appSettings["AppSettings:integration"]) || string.IsNullOrEmpty(appSettings["AppSettings:sqlType"]) ||
                string.IsNullOrEmpty(appSettings["AppSettings:b1version"]) || string.IsNullOrEmpty(appSettings["AppSettings:server"]) ||
                string.IsNullOrEmpty(appSettings["AppSettings:host"]) || string.IsNullOrEmpty(appSettings["AppSettings:port"]) ||
                string.IsNullOrEmpty(appSettings["AppSettings:sessiontimeout"]))
            {
                throw new Exception("No se encuentran datos en el archivo appsettings.");
            }

            Integration = appSettings["AppSettings:integration"];
            SQLType = appSettings["AppSettings:sqlType"];
            B1Version = appSettings["AppSettings:b1version"];
            Server = appSettings["AppSettings:server"];
            Host = appSettings["AppSettings:host"];
            Port = appSettings["AppSettings:port"];
            SessionTimeout = int.Parse(appSettings["AppSettings:sessiontimeout"]);

            UserDB = GetUserDB();
            PassDB = GetPassDB();
            _userSession.CompanyName = GetCompanyName();
        }

        public void GetNumberFormat()
        {
            NumberFormat = new NumberFormat();
            NumberFormat.DateToday = DateTime.Today.ToString("dd/MM/yyyy");
            NumberFormat.DateFormat = "dd/MM/yyyy";
            NumberFormat.SeparadorMiles = ".";
            NumberFormat.SeparadorDecimal = ",";
            NumberFormat.SeparadorMilesLocal = string.Format("{0:n2}", 123456.99);
            NumberFormat.SeparadorDecimalLocal = string.Format("{0:n2}", 123456.99);
            NumberFormat.DecimalesImporte = 0;
            NumberFormat.DecimalesCantidad = GetCantidadDecimales();
        }

        public string GetUserDB()
        {
            var userDB = string.Empty;

            var method = Method.Get;
            var entity = $"EPY_PLPY?$filter=Code eq '01'";
            var jObject = SetEntitySL(method, entity);

            if (jObject["value"].Count() > 0)
                userDB = jObject["value"][0]["U_USDB"].ToString();

            if (string.IsNullOrEmpty(userDB))
                throw new Exception("No se encuentra usuario de base de datos registrado, asegurese de crear estructura y actualizar credenciales.");

            var secureUserDB = string.Empty;

            try
            {
                secureUserDB = Encryption.DecryptString(userDB);
            }
            catch
            {
                throw new Exception("Verificar encriptación de usuario de base de datos registrado.");
            }

            return secureUserDB;
        }

        public string GetPassDB()
        {
            var passDB = string.Empty;

            var method = Method.Get;
            var entity = $"EPY_PLPY?$filter=Code eq '01'";
            var jObject = SetEntitySL(method, entity);

            if (jObject["value"].Count() > 0)
                passDB = jObject["value"][0]["U_PWDB"].ToString();

            if (string.IsNullOrEmpty(passDB))
                throw new Exception("No se encuentra password de base de datos registrado, asegurese de crear estructura y actualizar credenciales.");

            var securePassDB = string.Empty;

            try
            {
                securePassDB = Encryption.DecryptString(passDB);
            }
            catch
            {
                throw new Exception("Verificar encriptación de password de datos registrado.");
            }

            return securePassDB;
        }

        public string GetCompanyName()
        {
            var companyName = string.Empty;

            var method = Method.Post;
            var entity = $"CompanyService_GetAdminInfo";
            var jObject = SetEntitySL(method, entity);

            if (jObject.Count > 0)
                companyName = jObject["CompanyName"].ToString();

            return companyName;
        }

        public int GetCantidadDecimales()
        {
            var cantidadDecimales = 0;

            var method = Method.Post;
            var entity = $"CompanyService_GetAdminInfo";
            var jObject = SetEntitySL(method, entity);

            if (jObject.Count > 0)
                cantidadDecimales = int.Parse(jObject["AccuracyofQuantities"].ToString());

            return cantidadDecimales;
        }

        public void ConnectSAP(string usuario, string contrasena, string baseDatos)
        {
            _userSession.UserName = usuario;
            _userSession.PassSecure = Encryption.EncryptString(contrasena);
            _userSession.DataBase = baseDatos;

            GetAppSettings();

            GetNumberFormat();

            GetSessionSL();

            VerifyODBC();
        }

        public void GetSessionSL()
        {
            var entity = string.Empty;
            var url = string.Empty;
            var jsonBody = string.Empty;
            RestClient restClient;
            RestRequest restRequest;
            RestResponse restResponse;
            JObject jObject;

            if (!SessionExists())
            {
                var body = new
                {
                    CompanyDB = _userSession.DataBase,
                    UserName = _userSession.UserName,
                    Password = Encryption.DecryptString(_userSession.PassSecure),
                    SessionTimeout = SessionTimeout,
                    Language = 25
                };

                entity = "Login";
                url = $"https://{Host}:{Port}/b1s/v1/{entity}";
                jsonBody = Utility.Utils.JsonSerializeObject(body);

                var options = new RestClientOptions(url)
                {
                    RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
                    MaxTimeout = (int)decimal.MinusOne
                };

                restClient = new RestClient(options);
                restRequest = new RestRequest("", Method.Post);
                restRequest.AddHeader("Accept", "application/json");
                restRequest.RequestFormat = DataFormat.Json;
                restRequest.AddJsonBody(jsonBody);

                restResponse = restClient.Execute(restRequest);

                if (restResponse.StatusCode != HttpStatusCode.OK)
                {
                    if (restResponse.ErrorMessage != null)
                    {
                        throw new Exception(restResponse.ErrorMessage);
                    }
                    else
                    {
                        if (restResponse.ContentType.Contains("application/json"))
                        {
                            jObject = JsonConvert.DeserializeObject<JObject>(restResponse.Content);
                            throw new Exception(jObject["error"]["message"]["value"].ToString());
                        }
                        else
                        {
                            throw new Exception(restResponse.Content);
                        }
                    }
                }
                else
                {
                    foreach (var item in restResponse.Cookies)
                    {
                        if (((System.Net.Cookie)item).Name == "B1SESSION")
                        {
                            _userSession.SapSession = new Session
                            {
                                B1Session = ((System.Net.Cookie)item).Value,
                                CompanyDB = _userSession.DataBase,
                                SessionDateTime = ((System.Net.Cookie)item).TimeStamp
                            };
                        }
                    }
                }
            }
            else
            {
                if (DateTime.Now.Subtract(_userSession.SapSession.SessionDateTime).TotalMinutes > SessionTimeout)
                {
                    _userSession.SapSession = null;
                    GetSessionSL();
                }
            }
        }

        private bool SessionExists()
        {
            try
            {
                return _userSession.SapSession != null;
            }
            catch
            {
                return false;
            }
        }

        public void DisconnectSAP()
        {
            if (!SessionExists())
                return;

            CloseSessionSL();

            _userSession.Clear();
        }

        public void CloseSessionSL()
        {
            var entity = string.Empty;
            var url = string.Empty;
            RestClient restClient;
            RestRequest restRequest;
            RestResponse restResponse;
            JObject jObject;

            entity = "Logout";
            url = $"https://{Host}:{Port}/b1s/v1/{entity}";
            Uri target = new Uri(url);

            var options = new RestClientOptions(url)
            {
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
                MaxTimeout = (int)decimal.MinusOne
            };

            restClient = new RestClient(options);
            restRequest = new RestRequest("", Method.Post);
            restRequest.CookieContainer = new CookieContainer();
            restRequest.CookieContainer.Add(new Cookie("B1SESSION", _userSession.SapSession.B1Session) { Domain = target.Host });
            restRequest.CookieContainer.Add(new Cookie("CompanyDB", _userSession.SapSession.CompanyDB) { Domain = target.Host });

            restResponse = restClient.Execute(restRequest);

            if (restResponse.StatusCode != HttpStatusCode.NoContent)
            {
                if (restResponse.ErrorMessage != null)
                {
                    throw new Exception(restResponse.ErrorMessage);
                }
            }
            else
            {
                jObject = JsonConvert.DeserializeObject<JObject>(restResponse.Content);
                _userSession.SapSession = null;
            }
        }

        public JObject SetEntitySL(Method metodo, string entity, dynamic body = null, bool replace = false)
        {
            var url = string.Empty;
            var jsonBody = string.Empty;
            RestClient restClient;
            RestRequest restRequest;
            RestResponse restResponse;
            JObject jObject;

            GetSessionSL();

            url = $"https://{Host}:{Port}/b1s/v1/{entity}";
            Uri target = new Uri(url);

            var options = new RestClientOptions(url)
            {
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
                MaxTimeout = (int)decimal.MinusOne
            };

            restClient = new RestClient(options);
            restRequest = new RestRequest("", metodo);
            restRequest.CookieContainer = new CookieContainer();
            restRequest.CookieContainer.Add(new Cookie("B1SESSION", _userSession.SapSession.B1Session) { Domain = target.Host });
            restRequest.CookieContainer.Add(new Cookie("CompanyDB", _userSession.SapSession.CompanyDB) { Domain = target.Host });
            restRequest.AddHeader("Accept", "application/json;odata=minimalmetadata;charset=utf8");
            restRequest.AddHeader("Prefer", "odata.maxpagesize=0");

            if (metodo == Method.Patch)
                restRequest.AddHeader("B1S-ReplaceCollectionsOnPatch", replace.ToString());

            if (body != null)
            {
                jsonBody = Utility.Utils.JsonSerializeObject(body);
                restRequest.RequestFormat = DataFormat.Json;
                restRequest.AddJsonBody(jsonBody);
            }

            restResponse = restClient.Execute(restRequest);

            if (restResponse.StatusCode != HttpStatusCode.OK &&
                restResponse.StatusCode != HttpStatusCode.Created &&
                restResponse.StatusCode != HttpStatusCode.NoContent)
            {
                if (restResponse.ErrorMessage != null)
                {
                    throw new Exception(restResponse.ErrorMessage);
                }
                else
                {
                    if (restResponse.ContentType.Contains("application/json"))
                    {
                        jObject = JsonConvert.DeserializeObject<JObject>(restResponse.Content);
                        throw new Exception(jObject["error"]["message"]["value"].ToString());
                    }
                    else
                    {
                        throw new Exception(restResponse.Content);
                    }
                }
            }
            else
            {
                _userSession.SapSession = new Session
                {
                    B1Session = _userSession.SapSession.B1Session,
                    CompanyDB = _userSession.SapSession.CompanyDB,
                    SessionDateTime = DateTime.Now
                };

                jObject = JsonConvert.DeserializeObject<JObject>(restResponse.Content);
                return jObject;
            }
        }

        public string SetEntitySLBatch(Method metodo, string entity, dynamic body = null)
        {
            var url = string.Empty;
            RestClient restClient;
            RestRequest restRequest;
            RestResponse restResponse;
            var strObject = string.Empty;
            JObject jObject;

            GetSessionSL();

            url = $"https://{Host}:{Port}/b1s/v1/{entity}";
            Uri target = new Uri(url);

            var options = new RestClientOptions(url)
            {
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
                MaxTimeout = (int)decimal.MinusOne
            };

            restClient = new RestClient(options);
            restRequest = new RestRequest("", metodo);
            restRequest.CookieContainer = new CookieContainer();
            restRequest.CookieContainer.Add(new Cookie("B1SESSION", _userSession.SapSession.B1Session) { Domain = target.Host });
            restRequest.CookieContainer.Add(new Cookie("CompanyDB", _userSession.SapSession.CompanyDB) { Domain = target.Host });
            restRequest.AddHeader("Accept", "application/json;odata=minimalmetadata;charset=utf8");
            restRequest.AddHeader("Content-Type", $"multipart/mixed;boundary=Batch_BoundaryPPW");

            if (body != null)
            {
                restRequest.RequestFormat = DataFormat.Json;
                string bodyToString = Convert.ToString(body);
                restRequest.AddParameter("text/plain", bodyToString);
            }

            restResponse = restClient.Execute(restRequest);

            var jResponse = restResponse.Content;

            if (restResponse.Content.Contains("HTTP/1.1 400 Bad Request"))
            {
                jResponse = restResponse.Content.Substring(restResponse.Content.IndexOf('{'), restResponse.Content.LastIndexOf('}') + 1 - restResponse.Content.IndexOf('{'));
                jObject = JsonConvert.DeserializeObject<JObject>(jResponse);
                throw new Exception(jObject["error"]["message"]["value"].ToString());
            }

            return jResponse;
        }

        public OdbcConnection ConnectODBC()
        {
            var strConnectionString = string.Empty;

            if (SQLType == Consts.SQLType.Hana)
            {
                var driver = IntPtr.Size == 8 ? "{HDBODBC}" : "{HDBODBC32}";
                var serverNode = Server.Contains("@") ? Server.Split("@").GetValue(1) : Server;
                var dataBaseName = Server.Contains("@") ? Server.Split("@").GetValue(0) : string.Empty;

                strConnectionString = string.Concat(strConnectionString, "DRIVER=", driver, ";");
                strConnectionString = string.Concat(strConnectionString, "SERVERNODE=", serverNode, ";");
                strConnectionString = string.Concat(strConnectionString, "UID=", UserDB, ";");
                strConnectionString = string.Concat(strConnectionString, "PWD=", PassDB, ";");

                if (Server.Contains("@"))
                {
                    strConnectionString = string.Concat(strConnectionString, "DATABASE=", "Multitenant", ";");
                    strConnectionString = string.Concat(strConnectionString, "DATABASENAME=", dataBaseName, ";");
                }
            }

            OdbcConn = new OdbcConnection(strConnectionString.ToString());
            OdbcConn.Open();

            return OdbcConn;
        }

        public void DisconnectODBC()
        {
            OdbcConn.Close();
            OdbcConn.Dispose();
        }

        public void VerifyODBC()
        {
            try
            {
                ConnectODBC();
                DisconnectODBC();
            }
            catch
            {
                throw new Exception("No se puede conectar a ODBC, verifique credenciales de base de datos.");
            }
        }
    }
}
