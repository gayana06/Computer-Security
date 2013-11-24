using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Bluetooth_Tutorial
{
	public class SirsCryptClass
	{
		public const int MAX_BLOCK_LENGTH = 16;
        public const int MAX_KEY_LENGTH   = 16;
		
		
        private KeySize ekeySize= KeySize.Bits128;
        private BlockSize eblockSize= BlockSize.Bits128;
		
		
	
		//encrypts a directory per file
		public Boolean EncryptDirectory(string sourceDirectory,string cPassword)
        {
			var Files = Directory.EnumerateFiles(sourceDirectory, "*.*", SearchOption.AllDirectories);
            foreach (string currentFile in Files)
            {
                if (!currentFile.EndsWith(".aes"))
                {
                    if (false == Encrypt(Path.GetFullPath(currentFile), cPassword))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
		
		//decrypts a directory per file
        public Boolean DecryptDirectory(string sourceDirectory, string cPassword)
        {
            var Files = Directory.EnumerateFiles(sourceDirectory, "*.aes", SearchOption.AllDirectories);

            foreach (string currentFile in Files)
            {
                if (false == Decrypt(Path.GetFullPath(currentFile), cPassword))
                {
                    return false;
                }
            }

            return true;
        }

        /// Encrypts a file using Rijndael algorithm.
        ///</summary>
        ///<param name="inputFile"></param>
        ///<param name="outputFile"></param>
        private bool EncryptFile(string inputFile, string password)
        {
            bool isEncrypted = false;
            try
            {
                string outputFile = inputFile + ".aes";
               // string password = @"myKey123"; // Your Key Here
                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                string cryptFile = outputFile;
                FileStream fsCrypt = new FileStream(cryptFile, FileMode.Create);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateEncryptor(key, key),
                    CryptoStreamMode.Write);

                FileStream fsIn = new FileStream(inputFile, FileMode.Open);

                int data;
                while ((data = fsIn.ReadByte()) != -1)
                    cs.WriteByte((byte)data);


                fsIn.Close();
                cs.Close();
                fsCrypt.Close();
                File.Delete(inputFile);
                isEncrypted = true;
            }
            catch
            {
               // MessageBox.Show("Encryption failed!", "Error");
            }
            return isEncrypted;
        }

        ///
        /// Decrypts a file using Rijndael algorithm.
        ///</summary>
        ///<param name="inputFile"></param>
        ///<param name="outputFile"></param>
        private bool DecryptFile(string inputFile,string password)
        {
            bool isDecrypted = false;
            try
            {
                int index = inputFile.IndexOf(".aes");
                string outputFile = inputFile.Remove(index);
                {
                    //string password = @"myKey123"; // Your Key Here

                    UnicodeEncoding UE = new UnicodeEncoding();
                    byte[] key = UE.GetBytes(password);

                    FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);

                    RijndaelManaged RMCrypto = new RijndaelManaged();
                    
                    CryptoStream cs = new CryptoStream(fsCrypt,
                        RMCrypto.CreateDecryptor(key, key),
                        CryptoStreamMode.Read);
                    
                    FileStream fsOut = new FileStream(outputFile, FileMode.Create);

                    int data;
                    while ((data = cs.ReadByte()) != -1)
                        fsOut.WriteByte((byte)data);

                    fsOut.Close();
                    cs.Close();
                    fsCrypt.Close();
                    File.Delete(inputFile);
                    isDecrypted = true;
                   

                }
            }
            catch
            {
                // MessageBox.Show("Encryption failed!", "Error");
            }
            return isDecrypted;
        }





	
		//encrypts a file, then replace the original with a .aes encrypted file
		private Boolean Encrypt(string cOpenFile,string cPassword)
        {
			//destination file
			string cSaveFile= cOpenFile+".aes";
            //check param
            if (("" == cOpenFile) ||
                ("" == cPassword))
            {
                return false;
            }

            if (false == File.Exists(cOpenFile))
            {
                return false;
            }

            while (true == File.Exists(cSaveFile))
            {
                cSaveFile = cSaveFile+"1";
            }

            byte[] plainText = new byte[MAX_BLOCK_LENGTH];
            byte[] cipherText = new byte[MAX_BLOCK_LENGTH];
            byte[] bzkey = new byte[MAX_KEY_LENGTH];

            //get password
            bzkey = Encoding.Unicode.GetBytes(cPassword);

            //get bytes from file
            FileStream fileStream = new FileStream(cOpenFile, FileMode.Open);
            fileStream.Seek(0, SeekOrigin.Begin);

            //get the file stream for save 
            FileStream saveStream = new FileStream(cSaveFile, FileMode.Append);
            
            //set length of the file
            long lFileLength = fileStream.Length;
            //set position of the file
            long lPostion = fileStream.Position;

            //Read byte and Encrypt
            while (lPostion < lFileLength)
            {
                //Initialize  the buffer
                Initialize(plainText, MAX_BLOCK_LENGTH);

                long lHasRead = fileStream.Read(plainText, 0, MAX_BLOCK_LENGTH);
                if (0 >= lHasRead)
                {
                    break;
                }
                //set current cursor position
                lPostion = fileStream.Position;

                //Encrypt
                Aes aes = new Aes(ekeySize, bzkey, eblockSize);

                //Initialize  the buffer
                Initialize(cipherText, MAX_BLOCK_LENGTH);

                aes.Cipher(plainText, cipherText);
                saveStream.Write(cipherText, 0, MAX_BLOCK_LENGTH);
            }

            saveStream.Close();
            fileStream.Close();
			File.Delete(cOpenFile);
            return true;
        }

		//decrypts a file, then replace the original.aes encrypted file with a plaintext file
        private Boolean Decrypt(string cOpenFile, string cPassword)
        {
			int index = cOpenFile.IndexOf(".aes");
			string cSaveFile=cOpenFile.Remove(index);
            //check param
            if (("" == cOpenFile) ||
                ("" == cPassword))
            {
                return false;
            }

            if (0 > cOpenFile.LastIndexOf(".aes"))
            {
                return false;
            }

            if (false == File.Exists(cOpenFile))
            {
                return false;
            }

            while (true == File.Exists(cSaveFile))
            {
                cSaveFile = cSaveFile + "1";
            }

            byte[] plainText = new byte[MAX_BLOCK_LENGTH];
            byte[] cipherText = new byte[MAX_BLOCK_LENGTH];
            byte[] bzkey = new byte[MAX_KEY_LENGTH];

            //get password
            bzkey = Encoding.Unicode.GetBytes(cPassword);

            //get bytes from file
            FileStream fileStream = new FileStream(cOpenFile, FileMode.Open);
            fileStream.Seek(0, SeekOrigin.Begin);

            //get the file stream for save 
            FileStream saveStream = new FileStream(cSaveFile, FileMode.Append);

            //set length of the file
            long lFileLength = fileStream.Length;
            //set position of the file
            long lPostion = fileStream.Position;

            //Read byte and Decrypt
            while (lPostion < lFileLength)
            {
                //Initialize  the buffer
                Initialize(plainText, MAX_BLOCK_LENGTH);

                long lHasRead = fileStream.Read(plainText, 0, MAX_BLOCK_LENGTH);
                if (0 >= lHasRead)
                {
                    break;
                }
                //set current cursor position
                lPostion = fileStream.Position;

                //Encrypt
                Aes aes = new Aes(ekeySize, bzkey, eblockSize);

                //Initialize  the buffer
                Initialize(cipherText, MAX_BLOCK_LENGTH);
                //Decrypt
                aes.InvCipher(plainText, cipherText);
                saveStream.Write(cipherText, 0, MAX_BLOCK_LENGTH);
            }

            saveStream.Close();
            fileStream.Close();
			File.Delete(cOpenFile);
            return true;
        }

        private void Initialize(byte[] pByte,int iLength)
        {
            int iIndex = 0;
            for (iIndex = 0; iIndex < iLength; iIndex++)
            {
                pByte[iIndex] = 0;
            }
        }
	}
}