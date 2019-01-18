using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace DNSProxy {
    
    /// <summary>
    /// 深海鱼DNS服务器
    /// </summary>
    class Server {
        #region 变量和结构体
        public bool isRunning { get; private set; }
        public double RelaySum;
        public double CacheSum;
        private Socket server;
        private Task tListener;
        private CancellationTokenSource cListener;
        private SeaFish.DataBase dataBase = new SeaFish.DataBase();
        struct WorkingData {
            public EndPoint client;
            public byte[] dnsPack;

            public WorkingData(byte[] dnsPack, EndPoint client) {
                this.dnsPack = dnsPack;
                this.client = client;
            }
        }
        static readonly string basePaht = string.Format(@"{0}\{1}", Application.StartupPath, "network.txt");
        #endregion

        /// <summary>
        /// 开始DNS服务
        /// </summary>
        public int Start() {
            try {
                if (isRunning) return 0;
                server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                server.SendBufferSize = 512;
                server.ReceiveBufferSize = 512;
                server.Bind(new IPEndPoint(IPAddress.Loopback, 53));
                cListener = new CancellationTokenSource();
                CancellationToken cts = cListener.Token;
                tListener = new Task(() => { Listener(cts); }, cts);
                tListener.Start();
                isRunning = true;
                return 0;
            } catch (Exception err) {

                return GetErrCode(err);
            }
        }

        /// <summary>
        /// 停止DNS服务
        /// </summary>
        public int Stop() {
            try {
                cListener.Cancel();
                server.Close();
                isRunning = false;
                return 0;
            } catch (Exception err) {

                return GetErrCode(err);
            }
        }

        /// <summary>
        /// 域名解析过程
        /// </summary>
        /// <param name="data"></param>
        private void Working(WorkingData data) {
            DnsPack dnsPack = new DnsPack(data.dnsPack) ;
            EndPoint client = data.client;


            //本机PTR指针
            if (dnsPack.QR == 0 && dnsPack.Opcode == 0 && dnsPack.QueryRecords.Count >= 1) {
                if (dnsPack.QueryRecords[0].QueryType == QueryType.PTR) {
                    string hostName = Dns.GetHostName();
                    IPAddress[] localhost = Dns.GetHostAddresses(hostName);
                    foreach (IPAddress IP in localhost) {
                        if (IP.AddressFamily == AddressFamily.InterNetwork) {
                            byte[] ip = IP.GetAddressBytes();
                            string ptrstr = string.Format("{3}.{2}.{1}.{0}.{4}", ip[0], ip[1], ip[2], ip[3], @"in-addr.arpa");

                            if (dnsPack.QueryRecords[0].QueryName == ptrstr ||
                                dnsPack.QueryRecords[0].QueryName == @"1.0.0.127.in-addr.arpa") {

                                dnsPack.QR = 1; dnsPack.RA = 1; dnsPack.RD = 1;
                                dnsPack.ResouceRecords = new List<ResouceRecord>{
                                    new ResouceRecord{
                                        Point=IPAddress.HostToNetworkOrder(BitConverter.ToInt16(new byte[]{192,12}, 0)),
                                        Datas=dnsPack.getNameByts(SeaFish.Settings.Custom.ServerName),
                                        TTL =100,
                                        QueryClass=1,
                                        QueryType=QueryType.PTR,
                                    }
                                };
                                Response(dnsPack.ToBytes(), client);
                                return;
                            }
                        }
                    }
                } else if (dnsPack.QueryRecords[0].QueryType == QueryType.A) {
                    if (dnsPack.QueryRecords[0].QueryName == SeaFish.Settings.Custom.ServerName) {

                        dnsPack.QR = 1; dnsPack.RA = 1; dnsPack.RD = 1;
                        dnsPack.ResouceRecords = new List<ResouceRecord>{
                                    new ResouceRecord{
                                        Point=IPAddress.HostToNetworkOrder(BitConverter.ToInt16(new byte[]{192,12}, 0)),
                                        Datas=new byte[]{127,0,0,1 },
                                        TTL =100,
                                        QueryClass=1,
                                        QueryType=QueryType.A,
                                    }
                                };
                        Response(dnsPack.ToBytes(), client);
                        return;
                    }
                }
            }

            //如果存在缓存数据
            if (dataBase.ReadCache(dnsPack, out byte[] cache)) {
                DnsPack dnsCache = new DnsPack(cache) {
                    //这里需要修改识别标识
                    Sign = dnsPack.Sign,
                };
                Response(dnsCache.ToBytes(), client);

            } else {
                //无缓存则直接转发
                RelayDNS(data.dnsPack, client);
            }
        }

        /// <summary>
        /// 转发DNS包
        /// </summary>
        /// <param name="buff">包内容字节数组</param>
        /// <param name="read">包大小</param>
        private void RelayDNS(byte[] buff, EndPoint client) {
            List<Task> taskList = new List<Task>();
            bool isResponse = false;
            DnsPack[] dnsPacks;
            IPEndPoint[] server = new IPEndPoint[4];
            server[0] = new IPEndPoint(SeaFish.Settings.Custom.Server1, 53);//公共DNS服务器IP
            server[1] = new IPEndPoint(SeaFish.Settings.Custom.Server2, 53);//公共DNS服务器IP
            server[2] = new IPEndPoint(SeaFish.Settings.Custom.Server3, 53);//公共DNS服务器IP
            server[3] = new IPEndPoint(SeaFish.Settings.Custom.Server4, 53);//公共DNS服务器IP

            int Relay = SeaFish.Settings.Custom.Relay;
            switch (Relay) {
                case 0://国内优先
                    //首先用境内DNS解析，如果解析到境外IP，则使用境外DNS再次解析
                    ResponsenNonDomain(buff, client);
                    break;
                case 1://国际优先 
                    //首先用境外DNS解析，如果解析到境内IP，则使用境内DNS再次解析
                    ResponsenNonDomain(buff, client);
                    break;
                case 2://仅国内
                    RelayDns(false);
                    break;
                case 3://仅国际
                    RelayDns(true);
                    break;
                default:
                    ResponsenNonDomain(buff, client);
                    return;

            }

            void RelayDns(bool inter){
                //同时转发给两台服务器
                int index = (inter) ? 2 : 0;
                int Count = 0;
                dnsPacks = new DnsPack[2];
                for (int i = 0; i <= 1; i++) {

                    TaskFactory taskfactory = new TaskFactory();
                    taskList.Add(taskfactory.StartNew(new Action<object>(t => {
                        int index_ = int.Parse(t.ToString());
                        dnsPacks[index_] = new DnsPack(SendUDP(server[index + index_], buff));
                        dnsPacks[index_].Tag = index_;
                    }), i).ContinueWith(m => {
                        Count++;
                        if (!isResponse) {
                            foreach (DnsPack dns in dnsPacks) {
                                if (!dns.Success) continue;
                                if (dns.Rcode != 0) continue;
                                if (dns.ResouceRecords.Count >= 1 ||
                                    dns.GrantResouces.Count >= 1 ||
                                    dns.AdditResouces.Count >= 1) {

                                    foreach(ResouceRecord dnsr in dns.ResouceRecords){
                                        if (dnsr.QueryClass == 1 && dnsr.QueryType == QueryType.A) {
                                            Debug.WriteLine("数据来源：{0}\t中国地址：{1}\t解析值：{3}\t问题：{2}", 
                                                server[index + dns.Tag], IPinChina(dnsr.Datas), dns.QueryRecords[0].QueryName, new IPAddress(dnsr.Datas).ToString());
                                        }
                                    }

                                    Response(dns, client);
                                    isResponse = true;
                                    return;
                                }
                            }
                        }
                        if (Count >= 2) {
                            //所有服务器响应，但没有结果
                            ResponsenNonDomain(buff, client);
                        }
                    }));

                }
            }

            bool IPinChina(byte[] IPadd) {
                try {
                    String IPAddres = new IPAddress(IPadd).ToString();

                    string nettexts = SeaFish.Extend.ReadFile(basePaht).Replace("\r\n", "\n");
                    string[] networks = nettexts.Split((char)10);

                    foreach (string net in networks) {
                        if (SeaFish.Extend.IP_IN_Nework(IPAddres, net)) {
                            return true;
                        }
                    }
                } catch{}
                return false;
            }
        }

        /// <summary>
        /// 响应一个没有记录
        /// </summary>
        /// <param name="dnsPack">原始数据</param>
        /// <param name="client">客户端节点</param>
        private void ResponsenNonDomain(DnsPack dnsPack, EndPoint client) {
            //标记为响应
            dnsPack.QR = 1;
            //标记位未找到
            dnsPack.Rcode = 3;
            //响应客户端
            server.SendTo(dnsPack.ToBytes(), client);
        }

        /// <summary>
        /// 响应一个没有记录
        /// </summary>
        /// <param name="dnsPack">原始数据</param>
        /// <param name="client">客户端节点</param>
        private void ResponsenNonDomain(byte[] dnsPack, EndPoint client) {
            DnsPack dnsPack_ = new DnsPack(dnsPack);
            //标记为响应
            dnsPack_.QR = 1;
            //标记位未找到
            dnsPack_.Rcode = 3;
            //响应客户端
            server.SendTo(dnsPack_.ToBytes(), client);
        }

        /// <summary>
        /// 响应DNS请求
        /// </summary>
        /// <param name="DnsPack">响应数据</param>
        /// <param name="client">客户端节点</param>
        private void Response(DnsPack dnsPack, EndPoint client) {
            //响应并记录缓存
            server.SendTo(dnsPack.ToBytes(), client);

            //如果有解析到记录，则缓存数据

            //实际应该针对每条资源的TLL缓存，这里直接缓存了整个数据包
            if (dnsPack.Rcode == 0) {
                if (dnsPack.ResouceRecords.Count > 0 ||
                dnsPack.GrantResouces.Count > 0 ||
                dnsPack.AdditResouces.Count > 0)
                    dataBase.WriteCache(dnsPack);
            }
            //触发事件，修改统计数值
            RelaySum++;
            SumChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// 响应DNS请求
        /// </summary>
        /// <param name="dnsPack"></param>
        /// <param name="client"></param>
        private void Response(byte[] dnsPack, EndPoint client) {
            //这个方法用于把缓存的二进制发送个客户端
            //所以不再执行缓存
            server.SendTo(dnsPack, client);

            //触发事件，修改统计数值
            CacheSum++;
            SumChanged(this, EventArgs.Empty);
        }
        
        #region 其他方法
        /// <summary>
        /// 发送UDP数据包
        /// </summary>
        /// <param name="server">转发到指定网络节点</param>
        /// <param name="buff">包内容字节数组</param>
        /// <param name="read">包大小</param>
        /// <returns></returns>
        private byte[] SendUDP(IPEndPoint server, byte[] buff) {
            try {
                UdpClient proxy = new UdpClient();
                proxy.Client.ReceiveTimeout = 1000;

                proxy.Connect(server);
                proxy.Send(buff, buff.Length);

                byte[] bytes = proxy.Receive(ref server);

                return bytes;
            } catch {
                return new byte[0];
            }
        }

        /// <summary>
        /// 接收UDP数据包
        /// </summary>
        /// <param name="token">取消任务标记</param>
        private void Listener(CancellationToken token) {
            while (true) {
                try {
                    byte[] buff = new byte[512];
                    EndPoint client = new IPEndPoint(IPAddress.Loopback, 0);
                    int read = server.ReceiveFrom(buff, ref client);

                    TaskFactory task = new TaskFactory();
                    WorkingData data = new WorkingData(buff.Take(read).ToArray(), client);
                    task.StartNew(new Action<object>(t => {
                        Working(data);
                    }), data);

                } catch { continue;}

                token.ThrowIfCancellationRequested();

            }
        }
       
        /// <summary>
        /// 解读json
        /// </summary>
        /// <param name="JSON">json字符串</param>
        /// <param name="Key">key</param>
        /// <returns></returns>
        private string readJson(string JSON, string Key) {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                Dictionary<string, object> json = (Dictionary<string, object>)serializer.DeserializeObject(JSON);
                object value;
                if (json.TryGetValue(Key, out value)) return value.ToString();
                return string.Empty;
            }

        /// <summary>
        /// 获取一个异常的ErrorCode
        /// </summary>
        /// <param name="err">发生的异常</param>
        /// <returns>ErrorCode</returns>
        public int GetErrCode(Exception err) {
            Win32Exception w32ex = err as Win32Exception;
            if (w32ex == null)
                w32ex = err.InnerException as Win32Exception;
            if (w32ex != null)
                return w32ex.ErrorCode;
            return 0;
        }

        /// <summary>
        /// 获取一个异常的描述信息
        /// </summary>
        /// <param name="err">异常代码</param>
        /// <returns></returns>
        public string GetErrMsg(int err) {
            Win32Exception winex = new Win32Exception(err);
            return winex.Message;
        }

        /// <summary>
        /// 清空缓存数据库
        /// </summary>
        /// <returns></returns>
        public bool ClearCache() {
            return dataBase.ClearCache();
        }

        /// <summary>
        /// 定义事件，通知UI修改计数
        /// </summary>
        public event EventHandler<EventArgs> SumChanged;
        #endregion
    }
}
