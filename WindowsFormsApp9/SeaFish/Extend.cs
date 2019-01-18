using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace SeaFish {
    static class Extend {

        /// <summary>
        /// 判断一个IP地址是否在指定网络中
        /// </summary>
        /// <param name="IPAddres">IP地址：例如：192.168.1.1</param>
        /// <param name="Network">网络地址，例如：192.168.1.0/24</param>
        /// <returns></returns>
        public static bool IP_IN_Nework(string IPAddres, string Network) {
            try {
                string[] NetworkAry = Network.Split('/');
                if (NetworkAry.Length != 2) { return false; }
                string NetAddress = NetworkAry[0];
                int MaskLenth = 0;
                int.TryParse(NetworkAry[1], out MaskLenth);
                uint MaskAdd = 0xFFFFFFFF << (32 - MaskLenth);
                uint NetAdd = IP2Long(NetAddress);
                uint IPAdd = IP2Long(IPAddres);
                if (NetAdd != 0 && IPAdd != 0) {
                    uint NetWork_1 = NetAdd & MaskAdd;
                    uint NetWork_2 = IPAdd & MaskAdd;
                    if (NetWork_1 == NetWork_2) {
                        return true;
                    }
                }
            } catch {}
            return false;

            uint IP2Long(string IPString) {
                uint IPNumber = 0;
                try {
                    //字符串转为IP地址
                    IPAddress IP = IPAddress.Parse(IPString);
                    byte[] IPAry = IP.GetAddressBytes();
                    IPNumber += (uint)IPAry[0] << 24;
                    IPNumber += (uint)IPAry[1] << 16;
                    IPNumber += (uint)IPAry[2] << 8;
                    IPNumber += (uint)IPAry[3];
                } catch { throw; }
                return IPNumber;
            }
        }

        /// <summary>
        /// 读取文本文件
        /// </summary>
        /// <param name="FilePath">文件名称</param>
        /// <returns></returns>
        public static string ReadFile(string FilePath) {
            int Start = Environment.TickCount;
            try {
                FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                byte[] Data = new byte[fs.Length];
                br.Read(Data, 0, (int)fs.Length);

                string StringMessage;
                if (Data[0] >= 0xEF) {
                    if (Data[0] == 0xEF && Data[1] == 0xBB) {
                        StringMessage = System.Text.Encoding.UTF8.GetString(Data);
                    } else if (Data[0] == 0xFE && Data[1] == 0xFF) {
                        StringMessage = System.Text.Encoding.BigEndianUnicode.GetString(Data);
                    } else if (Data[0] == 0xFF && Data[1] == 0xFE) {
                        StringMessage = System.Text.Encoding.Unicode.GetString(Data);
                    } else {
                        StringMessage = System.Text.Encoding.Default.GetString(Data);
                    }
                } else {
                    StringMessage = System.Text.Encoding.Default.GetString(Data);
                }

                int Ended = Environment.TickCount;
                int Taking = Ended - Start;
                fs.Dispose();
                br.Dispose();
                return StringMessage;
            } catch {
                int Ended = Environment.TickCount;
                int Taking = Ended - Start;
                return "";
            }
        }
    }
}
