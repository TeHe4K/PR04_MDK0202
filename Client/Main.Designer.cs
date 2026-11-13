namespace Client
{
    partial class Main
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
            this.groupConnect = new System.Windows.Forms.GroupBox();
            this.lblIp = new System.Windows.Forms.Label();
            this.txtIp = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.lblLogin = new System.Windows.Forms.Label();
            this.txtLogin = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.panelNav = new System.Windows.Forms.Panel();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblPath = new System.Windows.Forms.Label();
            this.listFiles = new System.Windows.Forms.ListView();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panelButtons = new System.Windows.Forms.Panel();
            this.btnDownload = new System.Windows.Forms.Button();
            this.btnUpload = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupConnect.SuspendLayout();
            this.panelNav.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            //
            // groupConnect
            //
            this.groupConnect.Controls.Add(this.lblIp);
            this.groupConnect.Controls.Add(this.txtIp);
            this.groupConnect.Controls.Add(this.lblPort);
            this.groupConnect.Controls.Add(this.txtPort);
            this.groupConnect.Controls.Add(this.lblLogin);
            this.groupConnect.Controls.Add(this.txtLogin);
            this.groupConnect.Controls.Add(this.lblPassword);
            this.groupConnect.Controls.Add(this.txtPassword);
            this.groupConnect.Controls.Add(this.btnConnect);
            this.groupConnect.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupConnect.Location = new System.Drawing.Point(0, 0);
            this.groupConnect.Name = "groupConnect";
            this.groupConnect.Size = new System.Drawing.Size(704, 56);
            this.groupConnect.TabIndex = 0;
            this.groupConnect.TabStop = false;
            this.groupConnect.Text = "Подключение к серверу";
            //
            // lblIp
            //
            this.lblIp.AutoSize = true;
            this.lblIp.Location = new System.Drawing.Point(10, 25);
            this.lblIp.Name = "lblIp";
            this.lblIp.Size = new System.Drawing.Size(20, 13);
            this.lblIp.Text = "IP:";
            //
            // txtIp
            //
            this.txtIp.Location = new System.Drawing.Point(34, 22);
            this.txtIp.Name = "txtIp";
            this.txtIp.Size = new System.Drawing.Size(100, 20);
            this.txtIp.TabIndex = 1;
            this.txtIp.Text = "127.0.0.1";
            //
            // lblPort
            //
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(144, 25);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(35, 13);
            this.lblPort.Text = "Порт:";
            //
            // txtPort
            //
            this.txtPort.Location = new System.Drawing.Point(182, 22);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(55, 20);
            this.txtPort.TabIndex = 2;
            this.txtPort.Text = "5000";
            //
            // lblLogin
            //
            this.lblLogin.AutoSize = true;
            this.lblLogin.Location = new System.Drawing.Point(247, 25);
            this.lblLogin.Name = "lblLogin";
            this.lblLogin.Size = new System.Drawing.Size(41, 13);
            this.lblLogin.Text = "Логин:";
            //
            // txtLogin
            //
            this.txtLogin.Location = new System.Drawing.Point(292, 22);
            this.txtLogin.Name = "txtLogin";
            this.txtLogin.Size = new System.Drawing.Size(100, 20);
            this.txtLogin.TabIndex = 3;
            //
            // lblPassword
            //
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(402, 25);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(48, 13);
            this.lblPassword.Text = "Пароль:";
            //
            // txtPassword
            //
            this.txtPassword.Location = new System.Drawing.Point(454, 22);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(100, 20);
            this.txtPassword.TabIndex = 4;
            this.txtPassword.UseSystemPasswordChar = true;
            //
            // btnConnect
            //
            this.btnConnect.Location = new System.Drawing.Point(566, 20);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(126, 24);
            this.btnConnect.TabIndex = 5;
            this.btnConnect.Text = "Подключиться";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            //
            // panelNav
            //
            this.panelNav.Controls.Add(this.btnUp);
            this.panelNav.Controls.Add(this.btnRefresh);
            this.panelNav.Controls.Add(this.lblPath);
            this.panelNav.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelNav.Location = new System.Drawing.Point(0, 56);
            this.panelNav.Name = "panelNav";
            this.panelNav.Size = new System.Drawing.Size(704, 34);
            this.panelNav.TabIndex = 1;
            //
            // btnUp
            //
            this.btnUp.Location = new System.Drawing.Point(6, 5);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(75, 24);
            this.btnUp.TabIndex = 0;
            this.btnUp.Text = "Вверх";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            //
            // btnRefresh
            //
            this.btnRefresh.Location = new System.Drawing.Point(87, 5);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 24);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "Обновить";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // lblPath
            //
            this.lblPath.AutoSize = true;
            this.lblPath.Location = new System.Drawing.Point(172, 10);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(40, 13);
            this.lblPath.Text = "Путь: -";
            //
            // listFiles
            //
            this.listFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colType});
            this.listFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listFiles.FullRowSelect = true;
            this.listFiles.HideSelection = false;
            this.listFiles.Location = new System.Drawing.Point(0, 90);
            this.listFiles.MultiSelect = false;
            this.listFiles.Name = "listFiles";
            this.listFiles.Size = new System.Drawing.Size(704, 318);
            this.listFiles.TabIndex = 2;
            this.listFiles.UseCompatibleStateImageBehavior = false;
            this.listFiles.View = System.Windows.Forms.View.Details;
            this.listFiles.DoubleClick += new System.EventHandler(this.listFiles_DoubleClick);
            //
            // colName
            //
            this.colName.Text = "Имя";
            this.colName.Width = 480;
            //
            // colType
            //
            this.colType.Text = "Тип";
            this.colType.Width = 180;
            //
            // panelButtons
            //
            this.panelButtons.Controls.Add(this.btnDownload);
            this.panelButtons.Controls.Add(this.btnUpload);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Location = new System.Drawing.Point(0, 408);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(704, 40);
            this.panelButtons.TabIndex = 3;
            //
            // btnDownload
            //
            this.btnDownload.Location = new System.Drawing.Point(6, 8);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(130, 26);
            this.btnDownload.TabIndex = 0;
            this.btnDownload.Text = "Скачать файл";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            //
            // btnUpload
            //
            this.btnUpload.Location = new System.Drawing.Point(142, 8);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(130, 26);
            this.btnUpload.TabIndex = 1;
            this.btnUpload.Text = "Загрузить файл";
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            //
            // statusStrip
            //
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 448);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(704, 22);
            this.statusStrip.TabIndex = 4;
            //
            // lblStatus
            //
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(98, 17);
            this.lblStatus.Text = "Не подключено";
            //
            // Main
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 470);
            this.Controls.Add(this.listFiles);
            this.Controls.Add(this.panelNav);
            this.Controls.Add(this.groupConnect);
            this.Controls.Add(this.panelButtons);
            this.Controls.Add(this.statusStrip);
            this.MinimumSize = new System.Drawing.Size(720, 360);
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FTP-клиент";
            this.Load += new System.EventHandler(this.Main_Load);
            this.groupConnect.ResumeLayout(false);
            this.groupConnect.PerformLayout();
            this.panelNav.ResumeLayout(false);
            this.panelNav.PerformLayout();
            this.panelButtons.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupConnect;
        private System.Windows.Forms.Label lblIp;
        private System.Windows.Forms.TextBox txtIp;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label lblLogin;
        private System.Windows.Forms.TextBox txtLogin;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Panel panelNav;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.ListView listFiles;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colType;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
    }
}
