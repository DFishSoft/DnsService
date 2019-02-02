using System;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

namespace SeaFish {
    public partial class Form1 : Form {
        public string[] args;

        public Form1() {
            InitializeComponent();
            this.args = new string[] { };
        }

        public Form1(string[] args) {
            InitializeComponent();
            this.args = args;
        }

        DNSProxy.Server proxy = new DNSProxy.Server();
        delegate void ChangeInvoke();

        #region 事件
        private void Form1_Load(object sender, EventArgs e) {

            LoadSetting();
            tlStatus.Text = "服务未运行...";
            tlStatus.ForeColor = Color.Red;
            proxy.SumChanged += proxy_SumChanged;

            if (Settings.Custom.SavePoint) {
                this.tlVersion.Text = "Fish dns v1.0";
                this.Left = Settings.Custom.WindowsPoin.X;
                this.Top = Settings.Custom.WindowsPoin.Y;
            }
            notifyIcon1.Visible = true;

            foreach (string arg in this.args) {
                if (arg == "-h") {
                    this.WindowState = FormWindowState.Minimized;
                    this.ShowInTaskbar = false;
                }
                if (arg == "-r") buStart_Click(null, null);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = true;

            if (IsChange()) {
                int ret = (int)MessageBox.Show("配置信息已更改，是否保存？","温馨提示",
                    MessageBoxButtons.YesNoCancel,MessageBoxIcon.Question);
                if (ret == (int)DialogResult.Cancel) return;
                if (ret == (int)DialogResult.Yes) SaveSetting();
                if (ret == (int)DialogResult.No) { };
            }
            
            SavePoint();
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            notifyIcon1.Visible = true;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e) {
            ShowForm();
        }

        private void serverShow_Click(object sender, EventArgs e) {
            ShowForm();
        }

        private void buStart_Click(object sender, EventArgs e) {

            if (!proxy.isRunning) {
                int Err = proxy.Start();
                if (Err == 0) {
                    tlStatus.Text = "服务正在运行...";
                    tlStatus.ForeColor = Color.Blue;
                    serviceStart.Text = "停止服务";
                    buStart.Text = "停止服务";
                } else {
                    switch (Err) {
                        case 10048:
                            MessageBox.Show("端口被占用，请检查是否有其他DNS服务在运行", "启动服务失败", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            break;
                        default:
                            MessageBox.Show(proxy.GetErrMsg(Err), "启动服务失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                    }
                } 
            } else {
                if (MessageBox.Show("停止服务可能将无法打开网页，确定要继续么", "确定要停止服务么？",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {
                    int Err = proxy.Stop();
                    if (Err == 0) {
                        tlStatus.Text = "服务已停止！";
                        tlStatus.ForeColor = Color.Red;
                        serviceStart.Text = "启动服务";
                        buStart.Text = "启动服务";
                    } else {
                        MessageBox.Show(proxy.GetErrMsg(Err),"停止服务失败",  MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }

            }
            
        }

        private void proxy_SumChanged(object sender, EventArgs e) {
            ChangeInvoke change = new ChangeInvoke(ChangeNum);
            this.Invoke(change);
        }

        private void serviceEnded_Click(object sender, EventArgs e) {
            if (proxy.isRunning == true) {
                if (MessageBox.Show("检测到服务正在运行中，真的要停止服务并退出程序么？", "服务正在运行",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {

                    if (proxy.Stop() != 0) {
                        MessageBox.Show("停止服务失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                } else {
                    return;
                }
            }
            SavePoint();
            notifyIcon1.Visible = false;
            System.Environment.Exit(0);
        }

        private void buCache_Click(object sender, EventArgs e) {
            if (MessageBox.Show("清空缓存表可能导致打开网页变慢，确定要继续么", "清空缓存表",
            MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {

                if (proxy.ClearCache()) {
                    MessageBox.Show("清空缓存表成功！", "清空缓存表成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                } else {
                    MessageBox.Show("清空缓存表失败！", "清空缓存表失败", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void cbRules_SelectedIndexChanged(object sender, EventArgs e) {

            if (cbRules.SelectedIndex <= 1) {

                MessageBox.Show("我还没想好调度算法，你这样选择我只会解析缓存的数据！", "tips", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }        
        
        #endregion

       
        #region 方法
        private void SaveSetting() {
            try {
                Settings.Custom.Server1 = IPAddress.Parse(tbDns1.Text);
                Settings.Custom.Server2 = IPAddress.Parse(tbDns2.Text);
                Settings.Custom.Server3 = IPAddress.Parse(tbDns3.Text);
                Settings.Custom.Server4 = IPAddress.Parse(tbDns4.Text);
                Settings.Custom.ServerName = tbDomain.Text;
                Settings.Custom.Relay = cbRules.SelectedIndex;
                Settings.Custom.Save();
                LoadSetting();

            } catch (Exception) {

                throw;
            }
        }

        private void LoadSetting() {
            Settings.Custom.Reload();
            tbDns1.Text = Settings.Custom.Server1.ToString();
            tbDns2.Text = Settings.Custom.Server2.ToString();
            tbDns3.Text = Settings.Custom.Server3.ToString();
            tbDns4.Text = Settings.Custom.Server4.ToString();
            cbRules.SelectedIndex = Settings.Custom.Relay;
            tbDomain.Text = Settings.Custom.ServerName;
        }

        private bool IsChange() {
            if (tbDns1.Text != Settings.Custom.Server1.ToString() |
            tbDns2.Text != Settings.Custom.Server2.ToString() |
            tbDns3.Text != Settings.Custom.Server3.ToString() |
            tbDns4.Text != Settings.Custom.Server4.ToString() |
            tbDomain.Text != Settings.Custom.ServerName |
            cbRules.SelectedIndex != Settings.Custom.Relay
            ) {
                return true;
            }
            return false;
        }

        private void ShowForm() {
            if (WindowState == FormWindowState.Minimized) {
                WindowState = FormWindowState.Normal;
                this.Activate();
                this.ShowInTaskbar = true;
            }
        }

        private void SavePoint() {
            //如果程序不在托盘则更新位置
            if (this.WindowState != FormWindowState.Minimized) {
                Settings.Custom.SavePoint = true;
                Settings.Custom.WindowsPoin = new Point(this.Left, this.Top);
                Settings.Custom.Save();
            }
        }

        private void ChangeNum() {
            tlResponse.Text = string.Format("{0} - {1}", proxy.RelaySum.ToString(), proxy.CacheSum.ToString());
        }
        
        #endregion


        }
    }
