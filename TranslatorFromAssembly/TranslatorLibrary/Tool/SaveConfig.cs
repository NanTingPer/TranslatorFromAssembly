namespace TranslatorLibrary.Tools
{
    public class SaveConfig
    {
        /// <summary>
        /// 获取key对应的配置值
        /// </summary>
        /// <param name="key">目标配置</param>
        /// <returns></returns>
        public static string GetConfig(string key, string defaultV)
        {
            string configPath = GetAppFilePath.GetPathAndCreate(key);
            string configValue = File.ReadAllText(configPath);
            if (configValue is null || string.IsNullOrWhiteSpace(configValue)) {
                return defaultV;
            }
            return configValue;
        }

        /// <summary>
        /// 写入对应Key的配置值
        /// </summary>
        /// <param name="key">目标key </param>
        /// <param name="value">要写入的值</param>
        public static void SetConfig(string key, string value)
        {
            string configKey = GetAppFilePath.GetPathAndCreate(key);
            File.Delete(configKey);
            File.WriteAllText(configKey, value);
        }
    }
}
