using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Bluetooth_Tutorial
{
    public class Crypto
    {

        private const string ENCRYPTED_FILE_EXTENTION = ".aes";
        private const string ERROR_FILE_ENCRYPTION = "File encryption error";
        private const string ERROR_FILE_DECRYPTION = "File decryption error";
        private const string PATTERN_ALL_FILES = "*.*";
        private const string PATTERN_AES_FILES = "*.aes";
        private const int AES_KEY_SIZE = 256;
        private const int AES_IV_SIZE = 128;

        #region Public Section

        /// <summary>
        /// Encrypts plaintext using AES 128bit key and a Chain Block Cipher and returns a base64 encoded string
        /// </summary>
        /// <param name="plainText">Plain text to encrypt</param>
        /// <param name="key">Secret key</param>
        /// <returns>Base64 encoded string</returns>
        public static String Encrypt(String plainText, String key)
        {
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(Encrypt(plainBytes, GetRijndaelManaged(key)));
        }

        /// <summary>
        /// Decrypts a base64 encoded string using the given key (AES 128bit key and a Chain Block Cipher)
        /// </summary>
        /// <param name="encryptedText">Base64 Encoded String</param>
        /// <param name="key">Secret Key</param>
        /// <returns>Decrypted String</returns>
        public static String Decrypt(String encryptedText, String key)
        {
            var encryptedBytes = Convert.FromBase64String(encryptedText);
            return Encoding.UTF8.GetString(Decrypt(encryptedBytes, GetRijndaelManaged(key)));
        }

        
        /// <summary>
        /// encrypts a directory per file
        /// </summary>
        /// <param name="sourceDirectory"></param>
        /// <param name="cPassword"></param>
        /// <returns></returns>
        public static List<string> EncryptDirectory(string sourceDirectory, string cPassword)
        {
            List<string> errorFileList = new List<string>();
            var Files = Directory.EnumerateFiles(sourceDirectory, PATTERN_ALL_FILES, SearchOption.AllDirectories);       
            foreach (string currentFile in Files)
            {
                if (!currentFile.EndsWith(ENCRYPTED_FILE_EXTENTION))
                {
                    if (false == EncryptFile(Path.GetFullPath(currentFile), cPassword))
                    {
                        errorFileList.Add(Path.GetFileName(currentFile));
                    }
                }
            }
            return errorFileList;
        }

        
        /// <summary>
        /// decrypts a directory per file
        /// </summary>
        /// <param name="sourceDirectory"></param>
        /// <param name="cPassword"></param>
        /// <returns></returns>
        public static List<string> DecryptDirectory(string sourceDirectory, string cPassword)
        {
            List<string> errorFileList = new List<string>();
            var Files = Directory.EnumerateFiles(sourceDirectory, PATTERN_AES_FILES, SearchOption.AllDirectories);
            foreach (string currentFile in Files)
            {
                if (false == DecryptFile(Path.GetFullPath(currentFile), cPassword))
                {
                    errorFileList.Add(Path.GetFileName(currentFile));
                }
            }

            return errorFileList;
        }


        #endregion

        #region Private Section

        private static RijndaelManaged GetRijndaelManaged(String secretKey)
        {
            var keyBytes = new byte[16];
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            Array.Copy(secretKeyBytes, keyBytes, Math.Min(keyBytes.Length, secretKeyBytes.Length));
            return new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128,
                Key = keyBytes,
                IV = keyBytes
            };
        }

        private static byte[] Encrypt(byte[] plainBytes, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateEncryptor()
                .TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }

        private static byte[] Decrypt(byte[] encryptedData, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateDecryptor()
                .TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        }

        /// Encrypts a file using Rijndael algorithm.
        ///</summary>
        ///<param name="inputFile"></param>
        ///<param name="outputFile"></param>
        private static bool EncryptFile(string inputFile, string password)
        {
            bool isEncrypted = false;
            string outputFile = inputFile + ENCRYPTED_FILE_EXTENTION;
            try
            {                
                byte[] salt = Encoding.ASCII.GetBytes(password.Length.ToString());
                PasswordDeriveBytes secretKey = new PasswordDeriveBytes(password, salt);
                byte[] key = secretKey.GetBytes(AES_KEY_SIZE/8);
                byte[] IV = secretKey.GetBytes(AES_IV_SIZE/8);

                FileStream fsCrypt = new FileStream(outputFile, FileMode.Create);
                RijndaelManaged RMCrypto = new RijndaelManaged();
                RMCrypto.Mode = CipherMode.CBC;
                CryptoStream cs = new CryptoStream(fsCrypt,RMCrypto.CreateEncryptor(key, IV), CryptoStreamMode.Write);
                FileStream fsIn = new FileStream(inputFile, FileMode.Open);
                int data;
                while ((data = fsIn.ReadByte()) != -1)
                {
                    cs.WriteByte((byte)data);
                }
                fsIn.Close();
                cs.Close();
                fsCrypt.Close();
                Util.DeleteFile(inputFile);
                isEncrypted = true;
            }
            catch (Exception ex)
            {
                Util.Logger().LogError(ERROR_FILE_ENCRYPTION, string.Empty, ex.Source);
                Util.DeleteFile(outputFile);
            }
            return isEncrypted;
        }

        ///
        /// Decrypts a file using Rijndael algorithm.
        ///</summary>
        ///<param name="inputFile"></param>
        ///<param name="outputFile"></param>
        private static bool DecryptFile(string inputFile, string password)
        {
            bool isDecrypted = false;
            string outputFile = inputFile.Remove(inputFile.IndexOf(ENCRYPTED_FILE_EXTENTION));
            try
            {
                byte[] salt = Encoding.ASCII.GetBytes(password.Length.ToString());
                PasswordDeriveBytes secretKey = new PasswordDeriveBytes(password, salt);
                byte[] key = secretKey.GetBytes(AES_KEY_SIZE / 8);
                byte[] IV = secretKey.GetBytes(AES_IV_SIZE / 8);

                FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);
                RijndaelManaged RMCrypto = new RijndaelManaged();
                RMCrypto.Mode = CipherMode.CBC;
                CryptoStream cs = new CryptoStream(fsCrypt, RMCrypto.CreateDecryptor(key, IV), CryptoStreamMode.Read);
                FileStream fsOut = new FileStream(outputFile, FileMode.Create);
                int data;
                while ((data = cs.ReadByte()) != -1)
                {
                    fsOut.WriteByte((byte)data);
                }

                fsOut.Close();
                cs.Close();
                fsCrypt.Close();
                Util.DeleteFile(inputFile);
                isDecrypted = true;

            }
            catch (Exception ex)
            {
                Util.Logger().LogError(ERROR_FILE_DECRYPTION, string.Empty, ex.Source);
                Util.DeleteFile(outputFile);
            }
            return isDecrypted;
        }

        #endregion
    }
}