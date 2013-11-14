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


namespace Bluetooth_Tutorial
{
    public partial class SecureYourDataForm : Form
    {
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

        public SecureYourDataForm()
        {
            InitializeComponent();
            Util.CreatePathDirectory();
            StartServer();

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
            Thread bluetoothServerThread = new Thread(new ThreadStart(ServerConnectThread));
            bluetoothServerThread.Start();
        }

        /// <summary>
        /// Update detail text box
        /// </summary>
        /// <param name="message"></param>
        public void UpdateDetails(string message)
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
        public void UpdateMainMessage(string message)
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
        public void UpdateDeviceMessage(string message)
        {
            Func<int> del = delegate()
            {
                lblDeviceName.Text = message;
                return 0;
            };
            Invoke(del);
        }
        
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

        public void RestartServer()
        {
            Func<int> del = delegate()
            {
                StartServer();
                return 0;
            };
            Invoke(del);
        }

        /// <summary>
        /// Server listing method
        /// </summary>
        public void ServerConnectThread()
        {
            BluetoothListener blueListener = new BluetoothListener(ApplicationID); ;
            BluetoothClient connection = null;
            Stream mStream = null;
            System.Threading.Timer ticker=null;
            while (true)
            {
                if (connection == null || !connection.Connected)
                {
                    lblMainMsg.Text = "Waiting for a connection";
                    isReplyTimeOut = false;
                    blueListener.Start();
                    connection = blueListener.AcceptBluetoothClient();
                    UpdateMainMessage("Client has connected");
                    UpdateDetails("Client has connected");
                    UpdateDeviceMessage(connection.RemoteMachineName + " : " + connection.RemoteEndPoint.ToString());
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
                            throw new Exception();
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
                            throw new Exception();
                        }
                    }
                    catch (Exception exception)
                    {
                        //Log this situation
                        UpdateDetails("Client has disconnected!!!!");
                        ResetDisplay();
                        connection.Close();
                        blueListener.Stop();
                        UpdateDetails("Server restarted!!!!");
                    }
                }
            }

        }

        public void TimeOutReply(object state)
        {
            isReplyTimeOut = true;
            MessageBox.Show("Hit here");
            //encrypt directory
        }

        public string ProcessInput(String message)
        {
            string reply = null;
            try
            {
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
                    }
                    else
                    {
                        //disconnect
                        //Authentication fail message
                        UpdateDetails("Authentication failed : password not correct");
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
                        }
                        else
                        {
                            //disconnect
                            //Authentication fail message
                            UpdateDetails("Authentication failed : Session key establishment failed");
                            ResetDisplay();
                        }
                    }
                    else
                    {
                        //disconnect
                        //Authentication fail message
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
                            Thread.Sleep(challengePeriod);
                            reply = MSG_CHALLENGE + MSG_SEP_COLON + GetChallengeValue();
                            reply = Crypto.Encrypt(reply, sessionKey);
                        }
                        else
                        {
                            //disconnect
                            //decrypt folder
                            UpdateDetails("Challenge response invalid");
                            ResetDisplay();
                        }
                    }
                    else
                    {
                        //disconnect
                        //decrypt folder
                        UpdateDetails("Session changed...disconnect");
                        ResetDisplay();
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateDetails("Wrong password");
            }
            return reply;

        }

        /// <summary>
        /// Check the challenge is answered correctly
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public bool CheckResponse(string response)
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
        public void IncrementIndex()
        {
            if (index < 2)
                index++;

        }

        /// <summary>
        /// Generate a challenge and keep it in the memory
        /// </summary>
        /// <returns></returns>
        public long GetChallengeValue()
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
        public string InitialMessageProcess_Deprecated(string message)
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
                        //log error
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
        public string InitialMessageProcess(string message)
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
        public string LatterMessageProcessing(string message)
        {
            string decryptedMsg = string.Empty;
            try
            {
                decryptedMsg = Crypto.Decrypt(message, sessionKey);
            }
            catch (Exception ex)
            {
                //log error
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





    }
}
