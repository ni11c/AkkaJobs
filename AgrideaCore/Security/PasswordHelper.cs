using System.Text;
using Agridea.Diagnostics.Logging;

using System;
using System.IO;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Web.Mvc;
using System.Web.Security;

namespace Agridea.Security
{
    public static class PasswordHelper
    {
        #region Constants
        private const string HashProvider = "SHA256Managed";
        // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
        // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
        private const string InitVector = "pemgail9uzpgzl88";
        // This constant is used to determine the keysize of the encryption algorithm
        private const int Keysize = 256;
        #endregion

        #region Services
        /// <summary>
        /// http://tekeye.biz/2015/encrypt-decrypt-c-sharp-string
        /// </summary>
        public static string Encrypt(string nonCryptedData, string password = "turlututu")
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(InitVector);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(nonCryptedData);
            PasswordDeriveBytes passwordDeriveBytes = new PasswordDeriveBytes(password, null);
            byte[] keyBytes = passwordDeriveBytes.GetBytes(Keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(cipherTextBytes);
        }
        public static string Decrypt(string cryptedData, string password = "turlututu")
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(InitVector);
            byte[] cipherTextBytes = Convert.FromBase64String(cryptedData);
            PasswordDeriveBytes passwordDeriveBytes = new PasswordDeriveBytes(password, null);
            byte[] keyBytes = passwordDeriveBytes.GetBytes(Keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }
        public static string GeneratePassword()
        {
            string password = null;
            do
                password = Membership.GeneratePassword(6, 0);
            while (password.Intersects("!@#$%^&*()_-+=[{]};:<>|./?O0IL"));
            return password;
        }
        public static string Hash(string password)
        {
            var crypto = CryptographyHelper.CreateHashWithSalt(Encoding.Unicode.GetBytes(password), null);
            return Convert.ToBase64String(crypto);

        }
        public static bool CompareHash(string password, string hashed)
        {
            if (password == null || hashed == null) return false;
            return CryptographyHelper.CompareHash(password, hashed);
        }
        public static bool TryRecoverPasswordFor(string from, string subject, IAgrideaService service, User user, ModelStateDictionary modelState)
        {
            try
            {
                string newPassword = PasswordHelper.GeneratePassword();
                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Send(
                    from,
                    user.Email,
                    subject,
                    string.Format("Votre mot de passe a été réinitialisé. Merci de vous connecter avec les informations de connexion ci-dessous, puis de modifier votre mot de passe:\n\n" +
                                  "Nom d'utilisateur: {0}\n" +
                                  "Mot de passe: {1}", user.UserName, newPassword));

                service.ChangePassword(user.UserName, newPassword);
                return true;
            }
            catch (SmtpException e)
            {
                Log.Error(e);
                modelState.AddModelError("", "Un problème est survenu lors de l'envoi du mot de passe.");
                return false;
            }
        }
        #endregion


    }
}
