namespace AtlasTCPSvcApp
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.logger = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.edAddIP = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.edListenPort = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.edListenIP = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnDelACL = new System.Windows.Forms.Button();
            this.btnAddACL = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.listACL = new System.Windows.Forms.ListBox();
            this.tmrChecker = new System.Windows.Forms.Timer(this.components);
            this.tmrTranWakeup = new System.Windows.Forms.Timer(this.components);
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(501, 321);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnClearLog);
            this.tabPage1.Controls.Add(this.logger);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(493, 295);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Log";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnClearLog
            // 
            this.btnClearLog.Location = new System.Drawing.Point(369, 258);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(118, 27);
            this.btnClearLog.TabIndex = 1;
            this.btnClearLog.Text = "Clear";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // logger
            // 
            this.logger.Location = new System.Drawing.Point(6, 7);
            this.logger.Multiline = true;
            this.logger.Name = "logger";
            this.logger.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.logger.Size = new System.Drawing.Size(481, 245);
            this.logger.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.edAddIP);
            this.tabPage2.Controls.Add(this.btnSave);
            this.tabPage2.Controls.Add(this.edListenPort);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.edListenIP);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.btnDelACL);
            this.tabPage2.Controls.Add(this.btnAddACL);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.listACL);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(493, 295);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Settings";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // edAddIP
            // 
            this.edAddIP.Location = new System.Drawing.Point(8, 212);
            this.edAddIP.Name = "edAddIP";
            this.edAddIP.Size = new System.Drawing.Size(143, 20);
            this.edAddIP.TabIndex = 9;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(388, 228);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(99, 33);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // edListenPort
            // 
            this.edListenPort.Location = new System.Drawing.Point(256, 73);
            this.edListenPort.Name = "edListenPort";
            this.edListenPort.Size = new System.Drawing.Size(229, 20);
            this.edListenPort.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(254, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Listen port";
            // 
            // edListenIP
            // 
            this.edListenIP.Location = new System.Drawing.Point(256, 20);
            this.edListenIP.Name = "edListenIP";
            this.edListenIP.Size = new System.Drawing.Size(229, 20);
            this.edListenIP.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(254, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Listen IP";
            // 
            // btnDelACL
            // 
            this.btnDelACL.Location = new System.Drawing.Point(8, 239);
            this.btnDelACL.Name = "btnDelACL";
            this.btnDelACL.Size = new System.Drawing.Size(106, 21);
            this.btnDelACL.TabIndex = 3;
            this.btnDelACL.Text = "Delete IP";
            this.btnDelACL.UseVisualStyleBackColor = true;
            this.btnDelACL.Click += new System.EventHandler(this.btnDelACL_Click);
            // 
            // btnAddACL
            // 
            this.btnAddACL.Location = new System.Drawing.Point(157, 212);
            this.btnAddACL.Name = "btnAddACL";
            this.btnAddACL.Size = new System.Drawing.Size(76, 21);
            this.btnAddACL.TabIndex = 2;
            this.btnAddACL.Text = "Add IP...";
            this.btnAddACL.UseVisualStyleBackColor = true;
            this.btnAddACL.Click += new System.EventHandler(this.btnAddACL_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "ACL";
            // 
            // listACL
            // 
            this.listACL.FormattingEnabled = true;
            this.listACL.Location = new System.Drawing.Point(8, 20);
            this.listACL.Name = "listACL";
            this.listACL.Size = new System.Drawing.Size(225, 186);
            this.listACL.TabIndex = 0;
            // 
            // tmrChecker
            // 
            this.tmrChecker.Tick += new System.EventHandler(this.tmrChecker_Tick);
            // 
            // tmrTranWakeup
            // 
            this.tmrTranWakeup.Interval = 60000;
            this.tmrTranWakeup.Tick += new System.EventHandler(this.tmrTranWakeup_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 321);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Atlas TCP service";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.SizeChanged += new System.EventHandler(this.MainForm_SizeChanged);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.TextBox logger;
        private System.Windows.Forms.Button btnDelACL;
        private System.Windows.Forms.Button btnAddACL;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listACL;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox edListenPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox edListenIP;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox edAddIP;
        private System.Windows.Forms.Timer tmrChecker;
        private System.Windows.Forms.Timer tmrTranWakeup;
    }
}

