namespace Bluetooth_Tutorial
{
    partial class SecureYourDataForm
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
            if (disposing && bluetoothServerThread!=null && bluetoothServerThread.IsAlive)
            {
                    System.Environment.Exit(0);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SecureYourDataForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.tbxDetails = new System.Windows.Forms.TextBox();
            this.lblMainMsg = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnRegNewUser = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lblDeviceName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.notifyicon = new System.Windows.Forms.NotifyIcon(this.components);
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.tbxDetails);
            this.panel1.Controls.Add(this.lblMainMsg);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(277, 248);
            this.panel1.TabIndex = 0;
            // 
            // tbxDetails
            // 
            this.tbxDetails.Location = new System.Drawing.Point(20, 63);
            this.tbxDetails.Multiline = true;
            this.tbxDetails.Name = "tbxDetails";
            this.tbxDetails.Size = new System.Drawing.Size(238, 167);
            this.tbxDetails.TabIndex = 1;
            // 
            // lblMainMsg
            // 
            this.lblMainMsg.AutoSize = true;
            this.lblMainMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMainMsg.Location = new System.Drawing.Point(51, 26);
            this.lblMainMsg.Name = "lblMainMsg";
            this.lblMainMsg.Size = new System.Drawing.Size(161, 17);
            this.lblMainMsg.TabIndex = 0;
            this.lblMainMsg.Text = "Waiting for a connection";
            this.lblMainMsg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.btnRegNewUser);
            this.panel2.Location = new System.Drawing.Point(298, 12);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(145, 81);
            this.panel2.TabIndex = 1;
            // 
            // btnRegNewUser
            // 
            this.btnRegNewUser.Location = new System.Drawing.Point(19, 26);
            this.btnRegNewUser.Name = "btnRegNewUser";
            this.btnRegNewUser.Size = new System.Drawing.Size(112, 23);
            this.btnRegNewUser.TabIndex = 0;
            this.btnRegNewUser.Text = "Register New User";
            this.btnRegNewUser.UseVisualStyleBackColor = true;
            this.btnRegNewUser.Click += new System.EventHandler(this.btnRegNewUser_Click);
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.lblDeviceName);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Location = new System.Drawing.Point(298, 100);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(145, 160);
            this.panel3.TabIndex = 2;
            // 
            // lblDeviceName
            // 
            this.lblDeviceName.AutoSize = true;
            this.lblDeviceName.Location = new System.Drawing.Point(5, 72);
            this.lblDeviceName.Name = "lblDeviceName";
            this.lblDeviceName.Size = new System.Drawing.Size(110, 13);
            this.lblDeviceName.TabIndex = 1;
            this.lblDeviceName.Text = "No device connected";
            this.lblDeviceName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(5, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Paired Device Name";
            // 
            // notifyicon
            // 
            this.notifyicon.BalloonTipText = "SIRS-SECAPP";
            this.notifyicon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyicon.Icon")));
            this.notifyicon.Text = "SIRS-SECAPP";
            this.notifyicon.Visible = true;
            this.notifyicon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notify_onclick);
            // 
            // SecureYourDataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(455, 272);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "SecureYourDataForm";
            this.Text = "SecureYourDataForm";
            this.Resize += new System.EventHandler(this.Form_Resize);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox tbxDetails;
        private System.Windows.Forms.Label lblMainMsg;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnRegNewUser;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label lblDeviceName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NotifyIcon notifyicon;
    }
}