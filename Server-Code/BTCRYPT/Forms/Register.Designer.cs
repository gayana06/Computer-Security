namespace Bluetooth_Tutorial
{
    partial class Register
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Register));
            this.panel1 = new System.Windows.Forms.Panel();
            this.tbxRePassword = new System.Windows.Forms.TextBox();
            this.lblRePassword = new System.Windows.Forms.Label();
            this.btnSelectFolder = new System.Windows.Forms.Button();
            this.tbxPath = new System.Windows.Forms.TextBox();
            this.lblPath = new System.Windows.Forms.Label();
            this.btnRegister = new System.Windows.Forms.Button();
            this.tbxPass = new System.Windows.Forms.TextBox();
            this.tbxUname = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblUsername = new System.Windows.Forms.Label();
            this.dialogFolderBrowse = new System.Windows.Forms.FolderBrowserDialog();
            this.errorUsername = new System.Windows.Forms.ErrorProvider(this.components);
            this.correctUname = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorPassword = new System.Windows.Forms.ErrorProvider(this.components);
            this.correctPassword = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorRePass = new System.Windows.Forms.ErrorProvider(this.components);
            this.correctRePass = new System.Windows.Forms.ErrorProvider(this.components);
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorUsername)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.correctUname)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorPassword)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.correctPassword)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorRePass)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.correctRePass)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.tbxRePassword);
            this.panel1.Controls.Add(this.lblRePassword);
            this.panel1.Controls.Add(this.btnSelectFolder);
            this.panel1.Controls.Add(this.tbxPath);
            this.panel1.Controls.Add(this.lblPath);
            this.panel1.Controls.Add(this.btnRegister);
            this.panel1.Controls.Add(this.tbxPass);
            this.panel1.Controls.Add(this.tbxUname);
            this.panel1.Controls.Add(this.lblPassword);
            this.panel1.Controls.Add(this.lblUsername);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(314, 198);
            this.panel1.TabIndex = 0;
            // 
            // tbxRePassword
            // 
            this.tbxRePassword.Location = new System.Drawing.Point(98, 92);
            this.tbxRePassword.MaxLength = 20;
            this.tbxRePassword.Name = "tbxRePassword";
            this.tbxRePassword.Size = new System.Drawing.Size(159, 20);
            this.tbxRePassword.TabIndex = 9;
            this.tbxRePassword.UseSystemPasswordChar = true;
            this.tbxRePassword.TextChanged += new System.EventHandler(this.check_textmatch);
            // 
            // lblRePassword
            // 
            this.lblRePassword.AutoSize = true;
            this.lblRePassword.Location = new System.Drawing.Point(15, 94);
            this.lblRePassword.Name = "lblRePassword";
            this.lblRePassword.Size = new System.Drawing.Size(83, 13);
            this.lblRePassword.TabIndex = 8;
            this.lblRePassword.Text = "Password Again";
            // 
            // btnSelectFolder
            // 
            this.btnSelectFolder.Location = new System.Drawing.Point(263, 130);
            this.btnSelectFolder.Name = "btnSelectFolder";
            this.btnSelectFolder.Size = new System.Drawing.Size(25, 23);
            this.btnSelectFolder.TabIndex = 7;
            this.btnSelectFolder.Text = "...";
            this.btnSelectFolder.UseVisualStyleBackColor = true;
            this.btnSelectFolder.Click += new System.EventHandler(this.btnSelectFolder_Click);
            // 
            // tbxPath
            // 
            this.tbxPath.Location = new System.Drawing.Point(98, 132);
            this.tbxPath.Name = "tbxPath";
            this.tbxPath.ReadOnly = true;
            this.tbxPath.Size = new System.Drawing.Size(159, 20);
            this.tbxPath.TabIndex = 6;
            // 
            // lblPath
            // 
            this.lblPath.AutoSize = true;
            this.lblPath.Location = new System.Drawing.Point(13, 132);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(61, 13);
            this.lblPath.TabIndex = 5;
            this.lblPath.Text = "Folder Path";
            // 
            // btnRegister
            // 
            this.btnRegister.Location = new System.Drawing.Point(16, 162);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(75, 23);
            this.btnRegister.TabIndex = 4;
            this.btnRegister.Text = "Register";
            this.btnRegister.UseVisualStyleBackColor = true;
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
            // 
            // tbxPass
            // 
            this.tbxPass.Location = new System.Drawing.Point(98, 56);
            this.tbxPass.MaxLength = 20;
            this.tbxPass.Name = "tbxPass";
            this.tbxPass.Size = new System.Drawing.Size(159, 20);
            this.tbxPass.TabIndex = 3;
            this.tbxPass.UseSystemPasswordChar = true;
            this.tbxPass.TextChanged += new System.EventHandler(this.password_validation);
            // 
            // tbxUname
            // 
            this.tbxUname.Location = new System.Drawing.Point(98, 19);
            this.tbxUname.MaxLength = 10;
            this.tbxUname.Name = "tbxUname";
            this.tbxUname.Size = new System.Drawing.Size(159, 20);
            this.tbxUname.TabIndex = 2;
            this.tbxUname.TextChanged += new System.EventHandler(this.uname_validation);
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(15, 56);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(53, 13);
            this.lblPassword.TabIndex = 1;
            this.lblPassword.Text = "Password";
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(13, 19);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(55, 13);
            this.lblUsername.TabIndex = 0;
            this.lblUsername.Text = "Username";
            // 
            // errorUsername
            // 
            this.errorUsername.ContainerControl = this;
            this.errorUsername.Icon = ((System.Drawing.Icon)(resources.GetObject("errorUsername.Icon")));
            // 
            // correctUname
            // 
            this.correctUname.ContainerControl = this;
            this.correctUname.Icon = ((System.Drawing.Icon)(resources.GetObject("correctUname.Icon")));
            // 
            // errorPassword
            // 
            this.errorPassword.ContainerControl = this;
            this.errorPassword.Icon = ((System.Drawing.Icon)(resources.GetObject("errorPassword.Icon")));
            // 
            // correctPassword
            // 
            this.correctPassword.ContainerControl = this;
            this.correctPassword.Icon = ((System.Drawing.Icon)(resources.GetObject("correctPassword.Icon")));
            // 
            // errorRePass
            // 
            this.errorRePass.ContainerControl = this;
            this.errorRePass.Icon = ((System.Drawing.Icon)(resources.GetObject("errorRePass.Icon")));
            // 
            // correctRePass
            // 
            this.correctRePass.ContainerControl = this;
            this.correctRePass.Icon = ((System.Drawing.Icon)(resources.GetObject("correctRePass.Icon")));
            // 
            // Register
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(338, 222);
            this.Controls.Add(this.panel1);
            this.Name = "Register";
            this.Text = "Register";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorUsername)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.correctUname)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorPassword)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.correctPassword)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorRePass)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.correctRePass)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox tbxPass;
        private System.Windows.Forms.TextBox tbxUname;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.TextBox tbxPath;
        private System.Windows.Forms.FolderBrowserDialog dialogFolderBrowse;
        private System.Windows.Forms.Button btnSelectFolder;
        private System.Windows.Forms.TextBox tbxRePassword;
        private System.Windows.Forms.Label lblRePassword;
        private System.Windows.Forms.ErrorProvider errorUsername;
        private System.Windows.Forms.ErrorProvider correctUname;
        private System.Windows.Forms.ErrorProvider errorPassword;
        private System.Windows.Forms.ErrorProvider correctPassword;
        private System.Windows.Forms.ErrorProvider errorRePass;
        private System.Windows.Forms.ErrorProvider correctRePass;
    }
}