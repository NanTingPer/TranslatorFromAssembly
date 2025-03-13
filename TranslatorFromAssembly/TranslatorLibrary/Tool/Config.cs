using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorLibrary.Tools
{
    public static class Config
    {
        /// <summary>
        /// 配置文件名
        /// </summary>
        public static string ConfigName { get; } = "Cof.conf";
        private static Dictionary<string, string> AConfig { get; } = [];
        private static string CreateConf(string confName)
        {
            string confPath = Path.Combine(Environment.CurrentDirectory, confName);
            if (!File.Exists(confPath)) {
                File.Create(confPath).Dispose();
            }
            return confPath;
        }

        private static void LoadConf()
        {
            CreateConf(ConfigName);
            //AConfig.Clear();
            string allText = File.ReadAllText(CreateConf(ConfigName));
            string[] allconf = allText.Split("\r\n");
            for (int i = 0; i < allconf.Length; i++) {
                string c = allconf[i];
                if (string.IsNullOrEmpty(c))
                    continue;

                string[] oneConf = c.Split("=");
                if (!AConfig.ContainsKey(oneConf[0].Trim()))
                    AConfig.Add(oneConf[0].Trim(), oneConf[1].Trim());
            }
        }

        /// <summary>
        /// 获取配置值
        /// </summary>
        /// <param name="confName"> 配置名 </param>
        public static string? GetConf(string confName)
        {
            LoadConf();
            if (AConfig.TryGetValue(confName, out string? confsizer)) {
                return confsizer ??= null;
            }
            return null;
        }

        /// <summary>
        /// 设置配置值
        /// </summary>
        /// <param name="confName"> 项目名 </param>
        /// <param name="confValue"> 项目值 </param>
        public static void SetConf(string confName, string confValue)
        {
            LoadConf();
            string? conf = GetConf(confName);
            if (conf is null || conf == string.Empty) {
                AConfig.Add(confName, confValue);
                WriteFile();
            } else {
                AConfig.Remove(confName);
                AConfig.Add(confName, confValue);
                WriteFile();
            }
        }

        private static void WriteFile()
        {
            LoadConf();
            string filePath = CreateConf(ConfigName);
            try {
                File.Delete(filePath);
            } catch {
                throw new Exception("配置文件写失败");
            }

            StreamWriter file = File.AppendText(filePath);
            foreach (var kv in AConfig) {
                string 配置项 = kv.Key;
                string 配置值 = kv.Value;
                file.WriteLine(配置项+"="+配置值);
            }
            file.Dispose();
        }

        /// <summary>
        /// 字符串转字节数组
        /// </summary>
        public static byte[] StringToBytes(string value)
        {
            List<byte> bytes = [];
            foreach (var cr in value) {
                bytes.Add(Convert.ToByte(cr));
            }
            return bytes.ToArray();
        }
    }
}
