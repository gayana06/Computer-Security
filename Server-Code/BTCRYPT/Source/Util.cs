#region Directive Section

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

#endregion

namespace Bluetooth_Tutorial
{
    class Util
    {

        #region Global-Definition

        private const string XML_PATH = "userdata";
        private const string XML_FILE_NAME="users.xml";
        private const char SEP_SLASH = '/';
        private const string XML_TAG_USERS = "USERS";
        private const string XML_TAG_USER = "USER";
        private const string XML_TAG_NAME = "NAME";
        private const string XML_TAG_PHASH = "PHASH";
        private const string XML_TAG_PATH = "PATH";
        private const string MSG_USERNAME_EXISTS = "Usename already exists. Enter a new username";
        private const string MSG_SELECT_NEW_FOLDER = "Select a new folder.";
        private static Random rand = new Random();
        private static ILogger logger;

        #endregion
        
        #region Public Section

        /// <summary>
        /// Log instance initiation
        /// </summary>
        /// <returns></returns>
        public static ILogger Logger()
        {
            if (logger == null)
                logger = Log4NetLogger.GetInstance();
            return logger;
        }

        /// <summary>
        /// Create a directory if not exist in the XML file path
        /// </summary>
        public static void CreatePathDirectory()
        {
            if (!Directory.Exists(XML_PATH))
            {
                Directory.CreateDirectory(XML_PATH);
            }
        }

        /// <summary>
        /// Add a new user to the system
        /// </summary>
        /// <param name="userObj"></param>
        /// <returns></returns>
        public static bool AddNewUser(User userObj)
        {
            bool isAdded = false;
            string filePath = XML_PATH + SEP_SLASH + XML_FILE_NAME;
            XmlDocument doc = ReadXml(filePath);
            XmlNode users = null;
            if (doc == null)
            {
                doc = new XmlDocument();
                XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                doc.AppendChild(docNode);
                users = doc.CreateElement(XML_TAG_USERS);
                doc.AppendChild(users);

            }
            else
            {
                users = doc.SelectSingleNode(XML_TAG_USERS);
            }

            XmlNode user = doc.CreateElement(XML_TAG_USER);
            users.AppendChild(user);
            XmlNode nameNode = doc.CreateElement(XML_TAG_NAME);
            nameNode.AppendChild(doc.CreateTextNode(userObj.Username));
            user.AppendChild(nameNode);
            nameNode = doc.CreateElement(XML_TAG_PHASH);
            nameNode.AppendChild(doc.CreateTextNode(userObj.PasswordHash));
            user.AppendChild(nameNode);
            nameNode = doc.CreateElement(XML_TAG_PATH);
            nameNode.AppendChild(doc.CreateTextNode(userObj.FolderPath));
            user.AppendChild(nameNode);
            doc.Save(filePath);

            return isAdded;
        }

        /// <summary>
        /// Load system users
        /// </summary>
        /// <returns></returns>
        public static List<User> LoadUserList()
        {
            List<User> userList = new List<User>();
            XmlDocument doc = ReadXml(XML_PATH + SEP_SLASH + XML_FILE_NAME);
            if (doc != null)
            {               
                User user = null;
                XmlNodeList nodes = doc.DocumentElement.SelectNodes(XML_TAG_USER);
                foreach (XmlNode node in nodes)
                {
                    user = new User();                    
                    foreach (XmlNode child in node)
                    {
                        if (child.Name == XML_TAG_NAME)
                            user.Username = child.InnerText;
                        else if (child.Name == XML_TAG_PHASH)
                            user.PasswordHash = child.InnerText;
                        else if (child.Name == XML_TAG_PATH)
                            user.FolderPath = child.InnerText;
                    }
                    userList.Add(user);
                }
            }
            return userList.Count>0?userList:null;
        }

        /// <summary>
        /// Read a XML file from a given path
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static XmlDocument ReadXml(string filePath)
        {
            XmlDocument doc=null;
            if (File.Exists(filePath))
            {
                doc = new XmlDocument();
                doc.Load(filePath);
            }
            return doc;
        }

        /// <summary>
        /// Check the new user is acceptable.
        /// Note: This system username, password and folder path are unique 
        /// </summary>
        /// <param name="newUser"></param>
        /// <returns></returns>
        public static bool IsValidNewUser(User newUser, ref string message)
        {
            bool isValid = true;
            List<User> existingUsers = LoadUserList();
            if (existingUsers != null)
            {
                foreach (User user in existingUsers)
                {
                    if (user.Username == newUser.Username)
                    {
                        isValid = false;
                        message = MSG_USERNAME_EXISTS;
                        break;
                    }
                    else if (newUser.FolderPath.Contains(user.FolderPath))
                    {
                        isValid = false;
                        message = MSG_SELECT_NEW_FOLDER;
                        break;
                    }
                }
            }
            return isValid;
        }

        /// <summary>
        /// Get SHA-1 Hash of a text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GetSHA1Hash_obsalete(string text)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            SHA1CryptoServiceProvider cryptoTransformSHA1 = new SHA1CryptoServiceProvider();
            return BitConverter.ToString(cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "").ToLower();
        }

        /// <summary>
        /// Get SHA-256 Hash of a text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GetSHA256Hash(string text)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            SHA256CryptoServiceProvider cryptoTransformSHA256 = new SHA256CryptoServiceProvider();
            return BitConverter.ToString(cryptoTransformSHA256.ComputeHash(buffer)).Replace("-", "").ToLower();
        }


        /// <summary>
        /// Generate a new session key
        /// </summary>
        public static string  GenerateSessionKey()
        {
            string guid = Guid.NewGuid().ToString();
            return GetSHA256Hash(guid);
        }

        /// <summary>
        /// Get a random value as a challenge
        /// </summary>
        /// <returns></returns>
        public static int GetChallengeValue()
        {
            return rand.Next(10000, Int32.MaxValue);
        }

        /// <summary>
        /// Filter the buffer received removing 0 values
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public static byte[] BufferFilter(byte[] packet)
        {
            var i = packet.Length - 1;
            while (packet[i] == 0)
            {
                --i;
            }
            var temp = new byte[i + 1];
            Array.Copy(packet, temp, i + 1);
            return temp;
        }

        /// <summary>
        /// Delete the file if exists
        /// </summary>
        /// <param name="file"></param>
        public static void DeleteFile(string file)
        {
            try
            {
                if (File.Exists(file))
                    File.Delete(file);
            }
            catch (Exception ex)
            {
                Logger().LogError("File deletion error", ex.Message, ex.Source);
            }
        }

        /// <summary>
        /// Print the header and footer of log files
        /// </summary>
        /// <param name="isHeader"></param>
        public static void PrintLogHeaderFooter(bool isHeader)
        {
            if (isHeader)
            {
                Logger().LogInfo("---------------------------------------------------------------------------------",string.Empty,string.Empty);
                Logger().LogInfo("--------------------------", "--START OF SESSION--", "--------------------------");
                Logger().LogInfo("---------------------------------------------------------------------------------", string.Empty, string.Empty);
            }
            else
            {
                Logger().LogInfo("---------------------------------------------------------------------------------", string.Empty, string.Empty);
                Logger().LogInfo("--------------------------", "---END OF SESSION---", "--------------------------");
                Logger().LogInfo("---------------------------------------------------------------------------------", string.Empty, string.Empty);
                Logger().LogInfo(string.Empty, string.Empty, string.Empty);
                Logger().LogInfo(string.Empty, string.Empty, string.Empty);
            }

        }
        #endregion
    }
}
