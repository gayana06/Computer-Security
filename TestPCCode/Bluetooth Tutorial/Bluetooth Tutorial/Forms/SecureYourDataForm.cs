#region Directive Section

using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

#endregion

namespace Bluetooth_Tutorial
{
    public partial class SecureYourDataForm : Form
    {
        #region Global-Definition

        Guid ApplicationID = new Guid("8ce255c0-200a-11e0-ac64-0800200c9a66");
        private String sessionKey;
        private User activeUser;
        private const String MSG_USERNAME = "UN";
        private const String MSG_SEP_COLON = ":";
        private const char MSG_SEP_COLON_CHAR = ':';
        private const String MSG_AUTHENTICATED = "ATD";
        private const String MSG_CHALLENGE = "CH";
        private const String MSG_CHALLENGE_REPLY = "CHR";
        private const String MSG_SESSION_KEY_GET = "GSK";
        private const String MSG_SESSION_KEY_REPLY = "RSK";
        private int index = 0;
        private string[] expectedSequence = new string[] { MSG_USERNAME, MSG_SESSION_KEY_REPLY, MSG_CHALLENGE_REPLY };
        private long challengeValue;
        private int challengePeriod = 5000; //in milliseconds
        private int challengeReplyTimeOut = 5000;//in milliseconds
        private bool isReplyTimeOut = false;
        private const string MSG_WAITING_FOR_CLIENT = "Waiting for a connection";
        private bool isFolderDecrypted = false;
        Thread bluetoothServerThread;
        private const string MSG_CLIENT_CONNCTED = "Client Connected";
        private const string MSG_CLIENT_DISCONNCTED = "Client Disonnected";
        private const string MSG_SERVER_RESTERTED = "Server Restarted";
        private const string MSG_CHALLENGE_REPLY_TIMEOUT = "Challenge reply timeout";
        private const string MSG_WRONG_VAL_FROM_MOBILE = "Wrong message from mobile";

        #endregion

        #region Public Section

        public SecureYourDataForm()
        {
            InitializeComponent();
            Util.CreatePathDirectory();
            StartServer();
        }

        #endregion

        #region Private Section

        /// <summary>
        /// Update detail text box
        /// </summary>
        /// <param name="message"></param>
        private void UpdateDetails(string message)
        {
            Func<int> del = delegate()
            {
                tbxDetails.AppendText(message + System.Environment.NewLine);
                return 0;
            };
            Invoke(del);
        }

        /// <summary>
        /// Update main detail label
        /// </summary>
        /// <param name="message"></param>
        private void UpdateMainMessage(string message)
        {
            Func<int> del = delegate()
            {
                lblMainMsg.Text = message;
                return 0;
            };
            Invoke(del);
        }

        /// <summary>
        /// Update paired device name
        /// </summary>
        /// <param name="message"></param>
        private void UpdateDeviceMessage(string message)
        {
            Func<int> del = delegate()
            {
                lblDeviceName.Text = message;
                return 0;
            };
            Invoke(del);
        }

        /// <summary>
        /// Server listing method
        /// </summary>
        private void ServerConnectThread()
        {
            BluetoothListener blueListener = new BluetoothListener(ApplicationID); ;
            BluetoothClient connection = null;
            Stream mStream = null;
            System.Threading.Timer ticker = null;
            while (true)
            {
                if (connection == null || !connection.Connected)
                {                    
                    isFolderDecrypted = false;
                    lblMainMsg.Text = MSG_WAITING_FOR_CLIENT;
                    isReplyTimeOut = false;
                    blueListener.Start();
                    connection = blueListener.AcceptBluetoothClient();
                    Util.PrintLogHeaderFooter(true);
                    UpdateMainMessage(MSG_CLIENT_CONNCTED);
                    UpdateDetails(MSG_CLIENT_CONNCTED);
                    UpdateDeviceMessage(connection.RemoteMachineName + " : " + connection.RemoteEndPoint.ToString());
                    Util.Logger().LogInfo(MSG_CLIENT_CONNCTED, "Machine Name: " + connection.RemoteMachineName + ", Endpoint: " + connection.RemoteEndPoint.ToString(),string.Empty);
                    index = 0;
                    mStream = connection.GetStream();
                }
                else
                {
                    try
                    {
                        //handle server connection
                        byte[] received = new byte[1024];
                        mStream.Read(received, 0, received.Length);
                        if (isReplyTimeOut)
                        {
                            ticker.Dispose();
                            throw new Exception(MSG_CHALLENGE_REPLY_TIMEOUT);
                        }
                        else
                        {
                            if (ticker != null)
                                ticker.Dispose();
                        }
                        UpdateDetails("Received: " + Encoding.ASCII.GetString(received));
                        string reply = ProcessInput(Encoding.ASCII.GetString(Util.BufferFilter(received)));
                        if (reply != null)
                        {
                            byte[] sent = Encoding.ASCII.GetBytes(reply);
                            mStream.Write(sent, 0, sent.Length);
                            ticker = new System.Threading.Timer(TimeOutReply, null, challengeReplyTimeOut, challengeReplyTimeOut);
                        }
                        else
                        {
                            throw new Exception(MSG_WRONG_VAL_FROM_MOBILE);
                        }
                    }
                    catch (Exception ex)
                    {
                        //Log this situation
                        Util.Logger().LogError(ex.Message, MSG_SERVER_RESTERTED, ex.Source);
                        if(isFolderDecrypted)
                            FolderEncryptionThreadStart();
                        UpdateDetails(MSG_CLIENT_DISCONNCTED);
                        ResetDisplay();
                        connection.Close();
                        blueListener.Stop();
                        UpdateDetails(MSG_SERVER_RESTERTED);
                        Util.PrintLogHeaderFooter(false);
                    }
                }
            }

        }

        /// <summary>
        /// Start registraion window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRegNewUser_Click(object sender, EventArgs e)
        {
            Register register = new Register();
            register.Show();
        }

        /// <summary>
        /// start server in a new thread
        /// </summary>
        private void StartServer()
        {
            bluetoothServerThread = new Thread(new ThreadStart(ServerConnectThread));
            bluetoothServerThread.Start();
        }

        /// <summary>
        /// Reset the frond end display
        /// </summary>
        private void ResetDisplay()
        {
            Func<int> del = delegate()
            {
                UpdateMainMessage("Waiting for a connection");
                UpdateDeviceMessage("No device connected");
                return 0;
            };
            Invoke(del);

        }

        /// <summary>
        /// Timeout action TODO: 
        /// </summary>
        /// <param name="state"></param>
        private void TimeOutReply(object state)
        {
            isReplyTimeOut = true;
        }

        /// <summary>
        /// Process the incoming messages
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private string ProcessInput(String message)
        {
            string reply = null;

            string decryptedMessage;

            if (index == 0)
            {
                decryptedMessage = InitialMessageProcess(message);
                UpdateDetails("Decrypted Message: " + decryptedMessage);
                if (decryptedMessage.StartsWith(expectedSequence[index] + MSG_SEP_COLON))
                {
                    GenerateSessionKey();
                    reply = MSG_SESSION_KEY_GET + MSG_SEP_COLON + sessionKey + MSG_SEP_COLON + GetChallengeValue();
                    reply = Crypto.Encrypt(reply, activeUser.PasswordHash);
                    IncrementIndex();
                    Util.Logger().LogInfo("Session key generated.",string.Empty,string.Empty);
                }
                else
                {
                    //disconnect
                    //Authentication fail message
                    Util.Logger().LogInfo("Authentication failed","Incorrect Password", string.Empty);
                    UpdateDetails("Authentication failed : Incorrect Password");
                    ResetDisplay();
                }
            }
            else if (index == 1)
            {
                decryptedMessage = LatterMessageProcessing(message);
                UpdateDetails("Decrypted Message: " + decryptedMessage);
                if (decryptedMessage.StartsWith(expectedSequence[index] + MSG_SEP_COLON))
                {
                    bool isValidReply = CheckResponse(decryptedMessage.Split(MSG_SEP_COLON.ToCharArray())[1]);
                    if (isValidReply)
                    {
                        reply = MSG_AUTHENTICATED + MSG_SEP_COLON + GetChallengeValue();
                        reply = Crypto.Encrypt(reply, sessionKey);
                        IncrementIndex();
                        Util.Logger().LogInfo("Authentication successful.", string.Empty, string.Empty);
                    }
                    else
                    {
                        //disconnect
                        //Authentication fail message
                        Util.Logger().LogInfo("Authentication failed", "Session key establishment failed", string.Empty);
                        UpdateDetails("Authentication failed : Session key establishment failed");
                        ResetDisplay();
                    }
                }
            }
            else if (index == 2)
            {
                decryptedMessage = LatterMessageProcessing(message);
                UpdateDetails("Decrypted Message: " + decryptedMessage);
                if (decryptedMessage.StartsWith(expectedSequence[index] + MSG_SEP_COLON))
                {
                    bool isValidReply = CheckResponse(decryptedMessage.Split(MSG_SEP_COLON.ToCharArray())[1]);
                    if (isValidReply)
                    {
                        if(!isFolderDecrypted)
                            FolderDecryptionThreadStart();
                        Thread.Sleep(challengePeriod);
                        reply = MSG_CHALLENGE + MSG_SEP_COLON + GetChallengeValue();
                        reply = Crypto.Encrypt(reply, sessionKey);                        
                    }
                    else
                    {
                        //disconnect
                        //decrypt folder
                        Util.Logger().LogInfo("Challenge reply invalid",string.Empty,string.Empty);
                        UpdateDetails("Challenge response invalid");
                        ResetDisplay();
                    }
                }
            }

            return reply;

        }

        /// <summary>
        /// Check the challenge is answered correctly
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private bool CheckResponse(string response)
        {
            long tmp = long.Parse(response);
            if (tmp == (challengeValue + 1))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Syncronization index increment
        /// </summary>
        private void IncrementIndex()
        {
            if (index < 2)
                index++;

        }

        /// <summary>
        /// Generate a challenge and keep it in the memory
        /// </summary>
        /// <returns></returns>
        private long GetChallengeValue()
        {
            challengeValue = Util.GetChallengeValue();
            return challengeValue;
        }


        /// <summary>
        /// Initial authentication cross checking with registered users
        /// Message decryption with password hash 
        /// Deprecated method
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private string InitialMessageProcess_Deprecated(string message)
        {
            List<User> userList = Util.LoadUserList();
            string decryptedMsg=string.Empty;
            if (userList != null)
            {
                foreach (User user in userList)
                {
                    try
                    {
                        decryptedMsg = Crypto.Decrypt(message, user.PasswordHash);
                        string[] tmp = decryptedMsg.Split(MSG_SEP_COLON_CHAR);
                        if (tmp[0] == MSG_USERNAME && tmp[1] == user.Username)
                        {
                            activeUser = user;
                            break;
                        }
                        else
                        {
                            decryptedMsg = string.Empty;
                        }
                    }
                    catch (Exception ex)
                    {
                        Util.Logger().LogError("Messsage decryption failed.", ex.Message, ex.Source);
                    }
                }
            }
            return decryptedMsg;
        }


        /// <summary>
        /// Initial authentication cross checking with registered users
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private string InitialMessageProcess(string message)
        {
            List<User> userList = Util.LoadUserList();
            bool isUserAvailable = false;
            if (userList != null)
            {
                foreach (User user in userList)
                {                    
                    string[] tmp = message.Split(MSG_SEP_COLON_CHAR);
                    if (tmp[0] == MSG_USERNAME && tmp[1] == user.Username)
                    {
                        activeUser = user;
                        isUserAvailable = true;
                        break;
                    }
                }
            }
            return isUserAvailable?message:string.Empty;
        }


        /// <summary>
        /// Messages decryption with session key
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private string LatterMessageProcessing(string message)
        {
            string decryptedMsg = string.Empty;
            try
            {
                decryptedMsg = Crypto.Decrypt(message, sessionKey);
            }
            catch (Exception ex)
            {
                Util.Logger().LogError("Messsage decryption failed.",ex.Message,ex.Source);
            }
            return decryptedMsg;
        }

        /// <summary>
        /// Generate a session key 
        /// </summary>
        private void GenerateSessionKey()
        {
            this.sessionKey = Util.GenerateSessionKey();
        }

        /// <summary>
        /// Start the folder encryption thread
        /// </summary>
        private void FolderEncryptionThreadStart()
        {
            Thread folderEncThread = new Thread(new ThreadStart(EncryptFolder));
            folderEncThread.Start();
            Util.Logger().LogInfo("Folder encryption started", string.Empty, string.Empty);
            folderEncThread.Join();
            Util.Logger().LogInfo("Folder encryption finished", string.Empty, string.Empty);
        }

        /// <summary>
        /// Start the folder decryption thread
        /// </summary>
        private void FolderDecryptionThreadStart()
        {
            Thread folderDecThread = new Thread(new ThreadStart(DecryptFolder));
            folderDecThread.Start();
            Util.Logger().LogInfo("Folder decryption started",string.Empty,string.Empty);
            folderDecThread.Join();
            Util.Logger().LogInfo("Folder decryption finished", string.Empty, string.Empty);
        }

        /// <summary>
        /// Folder encryption function
        /// </summary>
        private void EncryptFolder()
        {
            /* 
            SirsCryptClass crypt = new SirsCryptClass();
            bool done = crypt.EncryptDirectory(activeUser.FolderPath, activeUser.PasswordHash);
            if (done)
                isFolderDecrypted = false;
            */

            List<string> errorFileList = Crypto.EncryptDirectory(activeUser.FolderPath, activeUser.PasswordHash);
            if (errorFileList!=null && errorFileList.Count > 0)
            {
                Util.Logger().LogError("Files encrypted with errors", "Below files are not encrypted", string.Empty);
                foreach (string file in errorFileList)
                {
                    Util.Logger().LogError("Error-file : " + file, string.Empty, string.Empty);
                }
            }
            else
            {
                Util.Logger().LogInfo("Files encrypted successfully",string.Empty,string.Empty);
            }
            isFolderDecrypted = false;

        }

        /// <summary>
        /// Folder decryption function
        /// </summary>
        private void DecryptFolder()
        {
       /*     
            SirsCryptClass crypt = new SirsCryptClass();
            bool done = crypt.DecryptDirectory(activeUser.FolderPath, activeUser.PasswordHash);
            if (done)
                isFolderDecrypted = true;
       */
            List<string> errorFileList = Crypto.DecryptDirectory(activeUser.FolderPath, activeUser.PasswordHash);
            if (errorFileList != null && errorFileList.Count > 0)
            {
                Util.Logger().LogError("Files decrypted with errors", "Below files are not decrypted", string.Empty);
                foreach (string file in errorFileList)
                {
                    Util.Logger().LogError("Error-file : " + file, string.Empty, string.Empty);
                }
            }
            else
            {
                Util.Logger().LogInfo("Files decrypted successfully", string.Empty, string.Empty);
            }
            isFolderDecrypted = true;
        }


        /// <summary>
        /// Send the application to the notification tray
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyicon.Visible = true;
                notifyicon.ShowBalloonTip(500);
                this.Hide();
            }
        }

        /// <summary>
        /// Get the application wondow back from notification tray
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notify_onclick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            notifyicon.Visible = false;
        }

        #endregion

    }
}
