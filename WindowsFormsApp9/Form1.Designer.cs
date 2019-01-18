namespace SeaFish {
    partial class Form1 {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.serverShow = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.flushDNS = new System.Windows.Forms.ToolStripMenuItem();
            this.serviceStart = new System.Windows.Forms.ToolStripMenuItem();
            this.serviceEnded = new System.Windows.Forms.ToolStripMenuItem();
            this.tbDns1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbDns2 = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbDns3 = new System.Windows.Forms.TextBox();
            this.tbDns4 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.buStart = new System.Windows.Forms.Button();
            this.buCache = new System.Windows.Forms.Button();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.cbRules = new System.Windows.Forms.ComboBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tlStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tlResponse = new System.Windows.Forms.ToolStripStatusLabel();
            this.tlVersion = new System.Windows.Forms.ToolStripStatusLabel();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tbDomain = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.serverShow,
            this.toolStripMenuItem1,
            this.flushDNS,
            this.serviceStart,
            this.serviceEnded});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(181, 120);
            // 
            // serverShow
            // 
            this.serverShow.Name = "serverShow";
            this.serverShow.Size = new System.Drawing.Size(180, 22);
            this.serverShow.Text = "服务器设置...";
            this.serverShow.Click += new System.EventHandler(this.serverShow_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(177, 6);
            // 
            // flushDNS
            // 
            this.flushDNS.Name = "flushDNS";
            this.flushDNS.Size = new System.Drawing.Size(180, 22);
            this.flushDNS.Text = "清除DNS缓存";
            this.flushDNS.Click += new System.EventHandler(this.buCache_Click);
            // 
            // serviceStart
            // 
            this.serviceStart.Name = "serviceStart";
            this.serviceStart.Size = new System.Drawing.Size(180, 22);
            this.serviceStart.Text = "启动服务";
            this.serviceStart.Click += new System.EventHandler(this.buStart_Click);
            // 
            // serviceEnded
            // 
            this.serviceEnded.Name = "serviceEnded";
            this.serviceEnded.Size = new System.Drawing.Size(180, 22);
            this.serviceEnded.Text = "退出";
            this.serviceEnded.Click += new System.EventHandler(this.serviceEnded_Click);
            // 
            // tbDns1
            // 
            this.tbDns1.Location = new System.Drawing.Point(59, 20);
            this.tbDns1.Name = "tbDns1";
            this.tbDns1.Size = new System.Drawing.Size(125, 21);
            this.tbDns1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "主DNS：";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tbDns2);
            this.groupBox1.Controls.Add(this.tbDns1);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(190, 80);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "国内设置";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "备DNS：";
            // 
            // tbDns2
            // 
            this.tbDns2.Location = new System.Drawing.Point(59, 47);
            this.tbDns2.Name = "tbDns2";
            this.tbDns2.Size = new System.Drawing.Size(125, 21);
            this.tbDns2.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.tbDns3);
            this.groupBox2.Controls.Add(this.tbDns4);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(12, 98);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(190, 80);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "国际设置";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "备DNS：";
            // 
            // tbDns3
            // 
            this.tbDns3.Location = new System.Drawing.Point(59, 20);
            this.tbDns3.Name = "tbDns3";
            this.tbDns3.Size = new System.Drawing.Size(125, 21);
            this.tbDns3.TabIndex = 2;
            // 
            // tbDns4
            // 
            this.tbDns4.Location = new System.Drawing.Point(59, 47);
            this.tbDns4.Name = "tbDns4";
            this.tbDns4.Size = new System.Drawing.Size(125, 21);
            this.tbDns4.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "主DNS：";
            // 
            // buStart
            // 
            this.buStart.Location = new System.Drawing.Point(6, 133);
            this.buStart.Name = "buStart";
            this.buStart.Size = new System.Drawing.Size(124, 23);
            this.buStart.TabIndex = 7;
            this.buStart.Text = "启动服务";
            this.buStart.UseVisualStyleBackColor = true;
            this.buStart.Click += new System.EventHandler(this.buStart_Click);
            // 
            // buCache
            // 
            this.buCache.Location = new System.Drawing.Point(6, 104);
            this.buCache.Name = "buCache";
            this.buCache.Size = new System.Drawing.Size(124, 23);
            this.buCache.TabIndex = 6;
            this.buCache.Text = "清除缓存";
            this.buCache.UseVisualStyleBackColor = true;
            this.buCache.Click += new System.EventHandler(this.buCache_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "深海鱼迷你DNS";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // cbRules
            // 
            this.cbRules.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRules.FormattingEnabled = true;
            this.cbRules.Items.AddRange(new object[] {
            "国内优先",
            "国际优先",
            "仅国内",
            "仅国际"});
            this.cbRules.Location = new System.Drawing.Point(6, 35);
            this.cbRules.Name = "cbRules";
            this.cbRules.Size = new System.Drawing.Size(124, 20);
            this.cbRules.TabIndex = 4;
            this.cbRules.SelectedIndexChanged += new System.EventHandler(this.cbRules_SelectedIndexChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tlStatus,
            this.tlResponse,
            this.tlVersion});
            this.statusStrip1.Location = new System.Drawing.Point(0, 187);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(356, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 11;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tlStatus
            // 
            this.tlStatus.AutoSize = false;
            this.tlStatus.Name = "tlStatus";
            this.tlStatus.Size = new System.Drawing.Size(100, 17);
            this.tlStatus.Text = "tlStatus";
            this.tlStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tlResponse
            // 
            this.tlResponse.Name = "tlResponse";
            this.tlResponse.Size = new System.Drawing.Size(141, 17);
            this.tlResponse.Spring = true;
            this.tlResponse.Text = "0 - 0";
            this.tlResponse.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tlVersion
            // 
            this.tlVersion.AutoSize = false;
            this.tlVersion.Name = "tlVersion";
            this.tlVersion.Size = new System.Drawing.Size(100, 17);
            this.tlVersion.Text = "tlVersion";
            this.tlVersion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 12;
            this.label5.Text = "转发规则：";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tbDomain);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.buStart);
            this.groupBox3.Controls.Add(this.buCache);
            this.groupBox3.Controls.Add(this.cbRules);
            this.groupBox3.Location = new System.Drawing.Point(208, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(136, 166);
            this.groupBox3.TabIndex = 13;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "通用设置";
            // 
            // tbDomain
            // 
            this.tbDomain.Location = new System.Drawing.Point(6, 73);
            this.tbDomain.Name = "tbDomain";
            this.tbDomain.Size = new System.Drawing.Size(124, 21);
            this.tbDomain.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 58);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 12);
            this.label6.TabIndex = 13;
            this.label6.Text = "服务器名称：";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(356, 209);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "深海鱼迷你DNS";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem serverShow;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem flushDNS;
        private System.Windows.Forms.ToolStripMenuItem serviceStart;
        private System.Windows.Forms.ToolStripMenuItem serviceEnded;
        private System.Windows.Forms.TextBox tbDns1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbDns2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbDns3;
        private System.Windows.Forms.TextBox tbDns4;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buStart;
        private System.Windows.Forms.Button buCache;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ComboBox cbRules;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tlStatus;
        private System.Windows.Forms.ToolStripStatusLabel tlVersion;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox tbDomain;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ToolStripStatusLabel tlResponse;
    }
}

