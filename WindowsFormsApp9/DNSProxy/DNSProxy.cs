using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DNSProxy {

    public enum QueryType {
        A = 1,
        NS = 2,
        CNAME = 5,
        SOA = 6,
        WKS = 11,
        PTR = 12,
        HINFO = 13,
        MX = 15,
        AAAA = 28,
        AXFR = 252,
        ANY = 255
    }

    public class Query {
        public string QueryName { get; set; }
        public QueryType QueryType { get; set; }
        public Int16 QueryClass { get; set; }
        public Int16 Point { get; set; }

        public Query() {
        }

        public Query(Func<int, byte[]> read) {

            byte length = read(1)[0];
            //连续的两位11，表示真，二进制11的等于3
            bool Pointer = (length.GetBits(0, 2) == 3);
            if (!Pointer) {
                //如果不是指针
                StringBuilder name = new StringBuilder();
                while (length != 0) {
                    for (int i = 0; i < length; i++) {
                        name.Append((char)read(1)[0]);
                    }
                    length = read(1)[0];
                    if (length != 0)
                        name.Append(".");
                }
                QueryName = name.ToString();
            } else {
                //如果是指针，则其后面还有1字节数据
                read(-1);
                Point = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(read(2), 0));
            }

            QueryType = (QueryType)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(read(2), 0));
            QueryClass = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(read(2), 0));
        }

        protected List<byte> addName() {
            List<byte> list = new List<byte>();

            if (Point != 0) {
                list.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Point)));
            } else {
                string[] a = QueryName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < a.Length; i++) {
                    list.Add((byte)a[i].Length);
                    for (int j = 0; j < a[i].Length; j++)
                        list.Add((byte)a[i][j]);
                }
                list.Add(0);
            }
            return list;
        }

        public virtual byte[] ToBytes() {
            List<byte> list = addName();

            list.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((Int16)QueryType)));
            list.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(QueryClass)));

            return list.ToArray();

        }
    }

    public class ResouceRecord : Query {
        public Int32 TTL { get; set; }
        public byte[] Datas { get; set; }


        public ResouceRecord() : base() { }

        public ResouceRecord(Func<int, byte[]> read) : base(read) {
            TTL = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(read(4), 0));
            int length2 = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(read(2), 0));
            Datas = read(length2);
        }


        public override byte[] ToBytes() {
            List<byte> list = addName();
            list.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((Int16)QueryType)));
            list.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(QueryClass)));
            list.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(TTL)));
            list.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((Int16)Datas.Length)));
            list.AddRange(Datas);

            return list.ToArray();
        }
    }

    public class DnsPack {
        public Int16 Sign { get; set; } //报文识别标识，响应报文应该与请求报文一致
        public int QR { get; set; }     //0表示查询报文 1表示响应报文
        public int Opcode { get; set; } //0表示标准查询,1表示反向查询,2表示服务器状态请求
        public int AA { get; set; }  //授权回答
        public int TC { get; set; } //表示可截断的
        public int RD { get; set; } //表示期望递归 
        public int RA { get; set; } //表示可用递归
        public int Rcode { get; set; } //0表示没有错误,3表示名字错误
        public bool Success { get; private set; }//表示是否创建成功
        public int Tag { get; set; }//用于多任务识别

        public List<Query> QueryRecords { get; set; }  //问题记录
        public List<ResouceRecord> ResouceRecords { get; set; }  //非授权资源记录
        public List<ResouceRecord> GrantResouces { get; set; }  //授权资源记录
        public List<ResouceRecord> AdditResouces { get; set; }  //附加资源记录

        public byte[] ToBytes() {
            List<byte> list = new List<byte>();
            byte[] bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Sign));
            list.AddRange(bytes);
            byte b = new byte();
            b = b.SetBits(QR, 0, 1)
                .SetBits(Opcode, 1, 4)
                .SetBits(AA, 5, 1)
                .SetBits(TC, 6, 1);

            b = b.SetBits(RD, 7, 1);
            list.Add(b);
            b = new byte();
            b = b.SetBits(RA, 0, 1)
                .SetBits(0, 1, 3)
                .SetBits(Rcode, 4, 4);
            list.Add(b);

            list.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((Int16)QueryRecords.Count)));
            list.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((Int16)ResouceRecords.Count)));
            list.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((Int16)GrantResouces.Count)));
            list.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((Int16)AdditResouces.Count)));

            foreach (Query q in QueryRecords) {
                list.AddRange(q.ToBytes());
            }

            foreach (ResouceRecord r in ResouceRecords) {
                list.AddRange(r.ToBytes());
            }

            foreach (ResouceRecord r in GrantResouces) {
                list.AddRange(r.ToBytes());
            }

            foreach (ResouceRecord r in AdditResouces) {
                list.AddRange(r.ToBytes());
            }
            return list.ToArray();

        }

        private int index;
        private byte[] package;
        private byte ReadByte() {
            return package[index++];
        }
        private byte[] ReadBytes(int count = 1) {
            if (count < 0) {
                index += count;
                return new byte[0];
            }
            byte[] bytes = new byte[count];
            for (int i = 0; i < count; i++)
                bytes[i] = ReadByte();
            return bytes;
        }


        public DnsPack(byte[] buffer)
            : this(buffer, buffer.Length) {
        }

        public DnsPack(byte[] buffer, int length) {
            try {
                package = new byte[length];
                for (int i = 0; i < length; i++)
                    package[i] = buffer[i];

                Sign = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(ReadBytes(2), 0));


                byte b1 = ReadByte();
                byte b2 = ReadByte();

                QR = b1.GetBits(0, 1);
                Opcode = b1.GetBits(1, 4);
                AA = b1.GetBits(5, 1);
                TC = b1.GetBits(6, 1);
                RD = b1.GetBits(7, 1);

                RA = b2.GetBits(0, 1);
                Rcode = b2.GetBits(4, 4);

                int Querys = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(ReadBytes(2), 0));
                int Resouce = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(ReadBytes(2), 0));
                int Grants = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(ReadBytes(2), 0));
                int Addits = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(ReadBytes(2), 0));

                QueryRecords = new List<Query>();
                for (int i = 0; i < Querys; i++) {
                    QueryRecords.Add(new Query(ReadBytes));
                }

                ResouceRecords = new List<ResouceRecord>();
                for (int i = 0; i < Resouce; i++) {
                    ResouceRecords.Add(new ResouceRecord(ReadBytes));
                }

                GrantResouces = new List<ResouceRecord>();
                for (int i = 0; i < Grants; i++) {
                    GrantResouces.Add(new ResouceRecord(ReadBytes));
                }

                AdditResouces = new List<ResouceRecord>();
                for (int i = 0; i < Addits; i++) {
                    AdditResouces.Add(new ResouceRecord(ReadBytes));
                }
                Success = true;
            } catch {
                Success = false;
            }


        }

        public byte[] getNameByts(string name){
            List<byte> list = new List<byte>();

            string[] domains = name.Split('.');

            foreach (string item in domains) {

                list.Add((byte)item.Length);
                foreach (char char_ in item) {
                    list.Add((byte)char_);
                }

            }

            list.Add(0);
            return list.ToArray();
        }

    }

    public static class Extension {
        public static int GetBits(this byte b, int start, int length) {
            int temp = b >> (8 - start - length);
            int mask = 0;
            for (int i = 0; i < length; i++) {
                mask = (mask << 1) + 1;
            }

            return temp & mask;

        }

        public static byte SetBits(this byte b, int data, int start, int length) {
            byte temp = b;

            int mask = 0xFF;
            for (int i = 0; i < length; i++) {
                mask = mask - (0x01 << (7 - (start + i)));
            }
            temp = (byte)(temp & mask);
            mask = ((byte)data).GetBits(8 - length, length);
            mask = mask << (8 - start - length);
            return (byte)(temp | mask);
        }

        public static string ToBinaryString(this byte b) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 8; i++) {
                if (i % 4 == 0)
                    sb.Append(" ");
                int bit = b.GetBits(i, 1);
                sb.Append(bit);
            }
            return sb.ToString();
        }

        public static string BinaryString(this int b) {
            StringBuilder sb = new StringBuilder();
            for (int i = 15; i >= 0; i--)
                sb.Append((b >> i) & 0x01);
            return sb.ToString();
        }

        public static string ToHexString(this IEnumerable<byte> bytes) {
            StringBuilder sb = new StringBuilder();
            foreach (int b in bytes) {
                sb.Append(string.Format("{0:x2}({1}) ", b, (char)b));
            }
            return sb.ToString();
        }
    }
}