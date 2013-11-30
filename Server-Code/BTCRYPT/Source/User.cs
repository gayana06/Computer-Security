using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bluetooth_Tutorial
{
    class User
    {
        private string username;

        public string Username
        {
            get { return username; }
            set { username = value; }
        }
        private string passwordHash;

        public string PasswordHash
        {
            get { return passwordHash; }
            set { passwordHash = value; }
        }
        private string folderPath;

        public string FolderPath
        {
            get { return folderPath; }
            set { folderPath = value; }
        }


    }
}
