using System.Security.Cryptography;
using System.Text;

namespace eProduccion.Utility
{
    public class Encryption
    {
        private static string _encryption_key = "2$9E168o";

        public static string EncryptString(string original_string, string encryption_key = "")
        {
            if (string.IsNullOrEmpty(encryption_key))
            {
                encryption_key = _encryption_key;
            }

            return Encrypt(original_string, encryption_key);
        }

        public static string DecryptString(string encrypted_string, string encryption_key = "")
        {
            if (string.IsNullOrEmpty(encryption_key))
            {
                encryption_key = _encryption_key;
            }

            return Decrypt(encrypted_string, encryption_key);
        }

        #region Encrypt Functions

        private static string Encrypt(string strMessage, string strKey)
        {
            var objProvider = GetInstance(strKey);
            var objCrypto = objProvider.CreateEncryptor();
            byte[] arrBytBuffer = Encoding.Unicode.GetBytes(strMessage);

            // Devuelve la cadena encriptada
            return Convert.ToBase64String(objCrypto.TransformFinalBlock(arrBytBuffer, 0, arrBytBuffer.Length));
        }

        #endregion

        #region Decrypt Functions

        private static string Decrypt(string strMessage, string strKey)
        {
            var arrBytBuffer = Convert.FromBase64String(strMessage);
            var objProvider = GetInstance(strKey);
            var objCrypto = objProvider.CreateDecryptor();

            // Devuelve la cadena desencriptada
            return Encoding.Unicode.GetString(objCrypto.TransformFinalBlock(arrBytBuffer, 0, arrBytBuffer.Length));
        }

        #endregion

        #region Instance Function

        private static TripleDES GetInstance(string strKey)
        {
            var objProvider = new TripleDESCryptoServiceProvider();

            // Inicializa el proveedor
            objProvider.Key = Encoding.Unicode.GetBytes(strKey);
            objProvider.IV = new byte[objProvider.BlockSize / 8];
            // Devuelve el proveedor
            return objProvider;
        }

        #endregion
    }
}
