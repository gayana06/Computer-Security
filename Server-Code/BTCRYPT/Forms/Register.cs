#region Directive Section

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

#endregion

namespace Bluetooth_Tutorial
{
    public partial class Register : Form
    {
        #region Global-Definition

        private const string MSG_ENTER_VALID_UNAME = "Enter a valid username with at least 6 characters.\nOnly letters and digits.";
        private const string MSG_ENTER_VALID_PASS = "Enter a valid password with no space characters";
        private const string MSG_ENTER_VALID_FOLDER = "Select a folder";
        private const string MSG_PASSWORD_DESCRIPTION = "Password should have at least 8 characters.\nAt least one lowercase, one upper case, \none digit and one special character.";
        private const string MSG_NEWUSER_ADDED = "New user added to the system successfully.";
        private const string MSG_USERNAME_ALREADY_EXISTS = "The username exists, please give a new username";
        private const string MSG_PASSWORD_MISMATCH = "Password and Re-entered passwords do not match";        
        private const string REGX_PASSWORD = "^((?=(.*\\d))(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z\\d])).{8,20}$";
        private const string REGX_USERNAME = "^([a-zA-Z0-9]).{5,10}$";
        private const string TIP_CORRECT = "Correct";
        private const string TIP_MATCH = "Match";
        #endregion

        #region Public Section

        public Register()
        {
            InitializeComponent();
        }

        #endregion

        #region Private Section

        /// <summary>
        /// Register a new user to the system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                string username = tbxUname.Text.Trim();
                string password = tbxPass.Text.Trim();
                string reenteredPassword = tbxRePassword.Text.Trim();
                string folderPath = tbxPath.Text.Trim();
                if (ValidateInput(username, password, reenteredPassword, folderPath))
                {
                    User newuser = new User();
                    newuser.Username = username;
                    newuser.PasswordHash = Util.GetSHA256Hash(password);
                    newuser.FolderPath = folderPath;
                    string message = string.Empty;
                    if (Util.IsValidNewUser(newuser, ref message))
                    {
                        Util.AddNewUser(newuser);
                        ShowMsg(MSG_NEWUSER_ADDED);
                        Util.Logger().LogInfo("New user registered successfully.", string.Empty, string.Empty);
                        this.Dispose();
                    }
                    else
                    {
                        ShowMsg(message);
                    }
                }
            }
            catch (Exception ex)
            {
                Util.Logger().LogError("User registration failed.", ex.Message, ex.Source);
            }
        }

        /// <summary>
        /// Validate input data for the registration of user
        /// </summary>
        /// <param name="uname"></param>
        /// <param name="password"></param>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        private bool ValidateInput(string uname, string password,string againPassword, string folderPath)
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
            else if (password != againPassword)
            {
                ShowMsg(MSG_PASSWORD_MISMATCH);
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

        /// <summary>
        /// Message box display
        /// </summary>
        /// <param name="msg"></param>
        private void ShowMsg(string msg)
        {
            MessageBox.Show(msg);
        }

        /// <summary>
        /// Path selection dialog box 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            DialogResult result = dialogFolderBrowse.ShowDialog();
            if (result == DialogResult.OK)
            {
                tbxPath.Text = dialogFolderBrowse.SelectedPath;                
            }
        }


        /// <summary>
        /// Username frontend validation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uname_validation(object sender, EventArgs e)
        {
            Regex regex = new Regex(REGX_USERNAME);
            if (regex.IsMatch(tbxUname.Text))
            {
                errorUsername.SetError(tbxUname, string.Empty);
                correctUname.SetError(tbxUname, TIP_CORRECT);
            }
            else
            {
                errorUsername.SetError(tbxUname, MSG_ENTER_VALID_UNAME);
                correctUname.SetError(tbxUname, string.Empty);
            }
        }

        /// <summary>
        /// Password text validation at frontend
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void password_validation(object sender, EventArgs e)
        {
            Regex regex = new Regex(REGX_PASSWORD);
            if (regex.IsMatch(tbxPass.Text))
            {
                errorPassword.SetError(tbxPass, string.Empty);
                correctPassword.SetError(tbxPass, TIP_CORRECT);
            }
            else
            {
                errorPassword.SetError(tbxPass, MSG_PASSWORD_DESCRIPTION);
                correctPassword.SetError(tbxPass, string.Empty);
            }
        }

        /// <summary>
        /// Check re entering matching with original
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void check_textmatch(object sender, EventArgs e)
        {
            if (tbxPass.Text == tbxRePassword.Text)
            {
                errorRePass.SetError(tbxRePassword, string.Empty);
                correctRePass.SetError(tbxRePassword, TIP_MATCH);
            }
            else
            {
                errorRePass.SetError(tbxRePassword, MSG_PASSWORD_MISMATCH);
                correctRePass.SetError(tbxRePassword, string.Empty);
            }
        }


        #endregion


    }
}
