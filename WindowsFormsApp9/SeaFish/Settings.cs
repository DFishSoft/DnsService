using Microsoft.Win32;
using System;
using System.Drawing;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SeaFish {

    internal static class Settings {

        //配置保存路径
        private static string SavePath = @"Software\{0}\{1}";

        //构造函数
        static Settings() {
            AssemblyInfo assembly = GetExecuting();
            SavePath = string.Format(SavePath, assembly.Company, assembly.AppName);
        }

        //程序集信息
        private class AssemblyInfo {
            public string AppName;
            public string Description;
            public string Copyright;
            public string Company;
        }

        //默认的配置
        public class Default {
            private string Configur = "";
            public string AuthCode { get; set; } = "";
            public bool SavePoint { get; set; } = false;
            public Point WindowsPoin { get; set; } = new Point(0, 0);
            //--------------------------------------------------------
            public IPAddress Server1 { get; set; } = IPAddress.Parse(@"114.114.114.114");
            public IPAddress Server2 { get; set; } = IPAddress.Parse(@"114.114.115.115");
            public IPAddress Server3 { get; set; } = IPAddress.Parse(@"8.8.4.4");
            public IPAddress Server4 { get; set; } = IPAddress.Parse(@"8.8.8.8");
            public string ServerName { get; set; } = "dns.doffish.com";
            public int Relay { get; set; } = 0;


            public void Save() {
                Settings.Save(Configur);
            }

            public void Init() {
                Settings.Init(Configur);
            }

            public void Reload() {
                Settings.Reload(Configur);
            }

            public void Load(string Configur) {
                this.Configur = Configur;
                Settings.Reload(Configur);
            }
        }

        //用户的配置
        public static Default Custom { get; private set; } = new Default();

        //保存配置
        private static void Save(string Configur) {
            //反射静态类的字段
            PropertyInfo[] Fields = typeof(Default).GetProperties();
            string saveString = string.Empty;
            foreach (PropertyInfo Field in Fields) {
                object SetValue = Field.GetValue(Custom, null);
                string FieldType = Field.PropertyType.ToString();
                switch (FieldType) {
                    case "System.Drawing.Color":
                        saveString = ColorTranslator.ToHtml((Color)SetValue);
                        break;
                    default:
                        saveString = SetValue.ToString();
                        break;
                }

                WriteKey(Field.Name, saveString, Configur);

            }
        }

        ///读取配置
        private static void Reload(string Configur) {
            //反射静态类的字段
            PropertyInfo[] Fields = typeof(Default).GetProperties();
            foreach (PropertyInfo Field in Fields) {
                string RegValue;
                bool Success = ReadKey(Field.Name, out RegValue, Configur);
                if (Success) {
                    string FieldType = Field.PropertyType.ToString();
                    switch (FieldType) {
                        case "System.String":
                            Field.SetValue(Custom, RegValue, null);
                            break;
                        case "System.Drawing.Color":
                            Color ColorVal;
                            if (strToColor(RegValue, out ColorVal))
                                Field.SetValue(Custom, ColorVal, null);
                            break;
                        case "System.Int32":
                            Int32 IntVal;
                            if (int.TryParse(RegValue, out IntVal))
                                Field.SetValue(Custom, IntVal, null);
                            break;
                        case "System.Boolean":
                            Boolean BooleanVal;
                            if (Boolean.TryParse(RegValue, out BooleanVal))
                                Field.SetValue(Custom, BooleanVal, null);
                            break;
                        case "System.Double":
                            Double DoubleVal;
                            if (double.TryParse(RegValue, out DoubleVal))
                                Field.SetValue(Custom, DoubleVal, null);
                            break;
                        case "System.Drawing.Point":
                            Match match = Regex.Match(RegValue, @"^{X=([0-9]{1,5}),Y=([0-9]{1,5})}$");
                            if (match.Groups.Count == 3) {
                                Point PointVal = new Point(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
                                Field.SetValue(Custom, PointVal, null);
                            }
                            break;
                        case "System.Net.IPAddress":
                            IPAddress iPAddress;
                            if (IPAddress.TryParse(RegValue,out iPAddress))
                                Field.SetValue(Custom, iPAddress, null);
                            break;
                    }
                }
            }
            //转为颜色
            bool strToColor(string RegValue, out Color ColorVal) {
                try {
                    ColorVal = ColorTranslator.FromHtml(RegValue);
                    return true;
                } catch (Exception) {
                    ColorVal = Color.Black;
                    return false;
                }

            }
        }

        //还原默认设置
        private static void Init(string Configur) {
            //加载最新的配置，并记录下激活码
            Reload(Configur);
            string AuthCode = Custom.AuthCode;
            Custom = new Default();
            //还原激活码，保持激活状态
            Custom.AuthCode = AuthCode;
            Save(Configur);
        }

        //写入注册表
        private static bool WriteKey(string Keystr, string Value,string Configur) {
            try {
                string RegexPath = SavePath + @"\" + Configur;
                RegexPath = (Configur == string.Empty) ? SavePath : RegexPath;
                RegistryKey RegKey = Registry.CurrentUser;
                RegistryKey Software = RegKey.CreateSubKey(RegexPath);
                Software.SetValue(Keystr, Value);
                RegKey.Close();
                return true;
            } catch (Exception) {
                return false;
            }
        }

        //读取注册表
        private static bool ReadKey(string Keystr, out string Value,string Configur) {
            try {
                string RegexPath =  SavePath + @"\" + Configur;
                RegexPath = (Configur == string.Empty) ? SavePath : RegexPath;
                RegistryKey RegKey = Registry.CurrentUser;
                RegistryKey Software = RegKey.OpenSubKey(RegexPath, true);
                Value = Software.GetValue(Keystr).ToString();
                Software.Close();
                return true;
            } catch (Exception) {
                Value = string.Empty;
                return false;
            }

        }

        //获取程序集信息
        private static AssemblyInfo GetExecuting() {
            AssemblyInfo assembly = new AssemblyInfo();
            Assembly asm = Assembly.GetExecutingAssembly();
            assembly.AppName = Assembly.GetExecutingAssembly().GetName().Name;
            assembly.Description = ((AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(asm, typeof(AssemblyDescriptionAttribute))).Description;
            assembly.Copyright = ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(asm, typeof(AssemblyCopyrightAttribute))).Copyright;
            assembly.Company = ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(asm, typeof(AssemblyCompanyAttribute))).Company;
            return assembly;
        }
    }
}