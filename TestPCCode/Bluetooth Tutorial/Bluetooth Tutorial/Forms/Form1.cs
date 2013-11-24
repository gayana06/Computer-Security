using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using InTheHand;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Ports;
using InTheHand.Net.Sockets;
using System.IO;
using System.Security.Cryptography;



namespace Bluetooth_Tutorial
{
    public partial class Form1 : Form
    {

        private String sessionKey;
        private String passwordHash = "5baa61e4c9b93f3f0682250b6cf8331b7ee68fd8";

        private const String MSG_USERNAME="UN";
        private const String MSG_SEP_COLON=":";
        private const String MSG_AUTHENTICATED="ATD";
        private const String MSG_CHALLENGE="CH";
        private const String MSG_CHALLENGE_REPLY="CHR";
        private const String MSG_SESSION_KEY_GET="GSK";
        private const String MSG_SESSION_KEY_REPLY="RSK";

        private int index = 0;
        private string[] expectedSequence = new string[] { MSG_USERNAME, MSG_SESSION_KEY_REPLY, MSG_CHALLENGE_REPLY };
        private long challengeValue;
        Random rand;
        List<string> items;
        public Form1()
        {
            items = new List<string>();
            InitializeComponent();
            rand = new Random();
            Util.CreatePathDirectory();
        }

        private void bGo_Click(object sender, EventArgs e)
        {
            if (serverStarted)
            {
                updateUI("Server already started silly sausage!");
                return;
            }
            if (rbClient.Checked)
            {
                startScan();
            }
            else
            {
                connectAsServer();
            }
        }


        private void startScan()
        {
            listBox1.DataSource = null;
            listBox1.Items.Clear();
            items.Clear();
            Thread bluetoothScanThread = new Thread(new ThreadStart(scan));
            bluetoothScanThread.Start();
        }
        BluetoothDeviceInfo[] devices;
        private void scan()
        {

            updateUI("Starting Scan..");
            BluetoothClient client = new BluetoothClient();
            devices = client.DiscoverDevicesInRange();
            updateUI("Scan complete");
            updateUI(devices.Length.ToString()+" devices discovered");
            foreach (BluetoothDeviceInfo d in devices)
            {
                items.Add(d.DeviceName);
            }

            updateDeviceList();
        }

        private void connectAsServer()
        {
            Thread bluetoothServerThread = new Thread(new ThreadStart(ServerConnectThread));
            bluetoothServerThread.Start();
        }

        private void connectAsClient()
        {
            throw new NotImplementedException();
        }

        Guid mUUID = new Guid("8ce255c0-200a-11e0-ac64-0800200c9a66");
        bool serverStarted = false;
        public void ServerConnectThread()
        {
            serverStarted = true;
            updateUI("Server started, waiting for clients");
            BluetoothListener blueListener = new BluetoothListener(mUUID);
            blueListener.Start();
            BluetoothClient conn = blueListener.AcceptBluetoothClient();
            updateUI("Client has connected");
            index = 0;
            Stream mStream = conn.GetStream();
            while (true)
            {
                try
                {
                    //handle server connection
                    byte[] received = new byte[1024];
                   
                    mStream.Read(received, 0, received.Length);
                    updateUI("Received: " + Encoding.ASCII.GetString(received));

                    string reply = ProcessInput(Encoding.ASCII.GetString(Decode(received)));
                    if (reply != null)
                    {
                        byte[] sent = Encoding.ASCII.GetBytes(reply);
                        mStream.Write(sent, 0, sent.Length);
                    }
                    else
                    {
                        break;
                    }
                }
                catch (IOException exception)
                {
                    updateUI("Client has disconnected!!!!");
                    break;
                }
            }

        }

        public byte[] Decode(byte[] packet)
        {
            var i = packet.Length - 1;
            while (packet[i] == 0)
            {
                --i;
            }
            var temp = new byte[i + 1];
            Array.Copy(packet, temp, i + 1);
           // MessageBox.Show(temp.Length.ToString());
            return temp;
        }

        public string ProcessInput(String message)
        {
            string reply = null;
            try
            {
                string decryptedMessage;

                if (index == 0)
                {
                    decryptedMessage = Crypto.Decrypt(message, passwordHash);
                    updateUI("Decrypted Message: " + decryptedMessage);
                    if (decryptedMessage.StartsWith(expectedSequence[index] + MSG_SEP_COLON))
                    {
                        GenerateSessionKey();
                        reply = MSG_SESSION_KEY_GET + MSG_SEP_COLON + sessionKey + MSG_SEP_COLON + GetChallengeValue();
                        reply = Crypto.Encrypt(reply, passwordHash);
                        IncrementIndex();
                    }
                    else
                    {
                        //disconnect
                        //Authentication fail message
                    }
                }
                else if (index == 1)
                {
                    decryptedMessage = Crypto.Decrypt(message, sessionKey);
                    updateUI("Decrypted Message: " + decryptedMessage);
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
                    decryptedMessage = Crypto.Decrypt(message, sessionKey);
                    updateUI("Decrypted Message: " + decryptedMessage);
                    if (decryptedMessage.StartsWith(expectedSequence[index] + MSG_SEP_COLON))
                    {
                        bool isValidReply = CheckResponse(decryptedMessage.Split(MSG_SEP_COLON.ToCharArray())[1]);
                        if (isValidReply)
                        {
                            Thread.Sleep(5000);
                            reply = MSG_CHALLENGE + MSG_SEP_COLON + GetChallengeValue();
                            reply = Crypto.Encrypt(reply, sessionKey);
                        }
                        else
                        {
                            //disconnect
                            //decrypt folder
                        }
                    }
                    else
                    {
                        //disconnect
                        //decrypt folder
                    }
                }
            }
            catch (Exception ex)
            {
                updateUI("Wrong password");
            }
            return reply;

        }

        public bool CheckResponse(string response)
        {
            long tmp = long.Parse(response);
            if (tmp == (challengeValue + 1))
                return true;
            else
                return false;
        }

        public void IncrementIndex()
        {
            if (index < 3)
                index++;

        }

        private void GenerateSessionKey()
        {
            string guid = Guid.NewGuid().ToString();
            this.sessionKey=CalculateSHA1(guid,Encoding.ASCII);
        }

        public  string CalculateSHA1(string text, Encoding enc)
        {
            byte[] buffer = enc.GetBytes(text);
            SHA1CryptoServiceProvider cryptoTransformSHA1 = new SHA1CryptoServiceProvider();
            return BitConverter.ToString(cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "");
        }

        public long GetChallengeValue()
        {
            challengeValue = rand.Next(1000000, Int32.MaxValue);
            return challengeValue;
        }

        private void updateUI(string message)
        {
            Func<int> del = delegate()
            {
                tbOutput.AppendText(message + System.Environment.NewLine);
                return 0;
            };
            Invoke(del);
        }

        private void updateDeviceList()
        {
            Func<int> del = delegate()
            {
                listBox1.DataSource = items;
                return 0;
            };
            Invoke(del);
        }


        BluetoothDeviceInfo deviceInfo;
        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            deviceInfo = devices.ElementAt(listBox1.SelectedIndex);
            updateUI(deviceInfo.DeviceName + " was selected, attempting connect");

            if (pairDevice())
            {
                updateUI("device paired..");
                updateUI("starting connect thread");
                Thread bluetoothClientThread = new Thread(new ThreadStart(ClientConnectThread));
                bluetoothClientThread.Start();

            }
            else
            {
                updateUI("Pair failed");
            }

        }

        private void ClientConnectThread()
        {
            BluetoothClient client = new BluetoothClient();
            updateUI("attempting connect");
            client.BeginConnect(deviceInfo.DeviceAddress, mUUID, this.BluetoothClientConnectCallback, client);

        }

        void BluetoothClientConnectCallback(IAsyncResult result)
        {
            BluetoothClient client = (BluetoothClient)result.AsyncState;
            client.EndConnect(result);

            Stream stream = client.GetStream();
            stream.ReadTimeout = 1000;
            byte[] buf=new byte[1024];
            while (true)
            {
                while (!ready) ;
                stream.Write(message, 0, message.Length);
                stream.Read(buf, 0, buf.Length);
                string val = System.Text.Encoding.UTF8.GetString(buf);
                updateUI(val);
                ready = false;
            }
            

        }

        string myPin = "1234";
        private bool pairDevice()
        {
            if (!deviceInfo.Authenticated)
            {
                if (!BluetoothSecurity.PairRequest(deviceInfo.DeviceAddress, myPin))
                {
                    return false;
                }
            }
            return true;
        }


        bool ready = false;
        byte[] message;
        private void tbText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                message = Encoding.ASCII.GetBytes(tbText.Text);
                ready = true;
                tbText.Clear();
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            Register reg = new Register();
            reg.Show();
        }

        
    }
}
