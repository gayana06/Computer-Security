using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Bluetooth_Tutorial
{
    public partial class Register : Form
    {
        private const string MSG_ENTER_VALID_UNAME = "Enter a valid username with at least 6 characters";
        private const string MSG_ENTER_VALID_PASS = "Enter a valid password with no space characters";
        private const string MSG_ENTER_VALID_FOLDER = "Select a folder";
        private const string MSG_PASSWORD_DESCRIPTION = "Password should have at least 8 characters.\nAt least one lowercase, one upper case, \none digit and one special character.";
        private const string MSG_NEWUSER_ADDED = "New user added to the system successfully.";
        private const string MSG_USERNAME_ALREADY_EXISTS = "The username exists, please give a new username";
        private const string REGX_PASSWORD = "^((?=(.*\\d))(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z\\d])).{8,20}$";

        public Register()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Register a new user to the system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRegister_Click(object sender, EventArgs e)
        {
            string username=tbxUname.Text.Trim();
            string password=tbxPass.Text.Trim();
            string folderPath = tbxPath.Text.Trim();
            if (ValidateInput(username, password,folderPath))
            {
                User newuser = new User();
                newuser.Username = username;
                newuser.PasswordHash = Util.GetSHA1Hash(password);
                newuser.FolderPath = folderPath;
                if (Util.IsValidNewUser(newuser))
                {
                    Util.AddNewUser(newuser);
                    ShowMsg(MSG_NEWUSER_ADDED);
                    this.Dispose();
                }
                else
                {
                    ShowMsg(MSG_USERNAME_ALREADY_EXISTS);
                }


                
            }
        }

        /// <summary>
        /// Validate input data for the registration of user
        /// </summary>
        /// <param name="uname"></param>
        /// <param name="password"></param>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        private bool ValidateInput(string uname, string password, string folderPath)
        {
            bool isValid = false;
            if (uname == string.Empty || uname.Length<6)
            {
                ShowMsg(MSG_ENTER_VALID_UNAME);
            }
            else if (password == string.Empty || password.Contains(" "))
            {
                ShowMsg(MSG_ENTER_VALID_PASS);
            }
            else if (folderPath == string.Empty)
            {
                ShowMsg(MSG_ENTER_VALID_FOLDER);
            }
            else 
            {
                if (!Regex.IsMatch(password, REGX_PASSWORD))
                {
                    ShowMsg(MSG_PASSWORD_DESCRIPTION);
                }
                else
                {
                    isValid = true;
                }
            }
            return isValid;
        }

        private void ShowMsg(string msg)
        {
            MessageBox.Show(msg);
        }

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            DialogResult result = dialogFolderBrowse.ShowDialog();
            if (result == DialogResult.OK)
            {
                tbxPath.Text = dialogFolderBrowse.SelectedPath;                
            }
        }
    }
}
