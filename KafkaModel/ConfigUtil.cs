using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace KafkaModel
{
    /// <summary>
    /// cấu hình chung lưu global singleton
    /// </summary>
    public class ConfigUtil
    {
        static CenterConfig _centerConfig;

        public static CenterConfig CenterConfig
        {
            get
            {
                return _centerConfig;
            }
        }

        /// <summary>
        /// khởi tạo config toàn cục singleton
        /// </summary>
        /// <param name="builder"></param>
        public static void InitGlobalConfig(IHostApplicationBuilder builder)
        {
            IHostEnvironment hostEnvironment = builder.Environment;

            string configPath = GetCommonConfigFilePath(hostEnvironment);

            builder.Configuration.AddJsonFile(configPath, optional: false, reloadOnChange: true);

            var centerConfig = new CenterConfig();
            builder.Configuration.Bind(centerConfig);

            InitConfig(centerConfig);
        }

        /// <summary>
        /// khởi tạo config
        /// </summary>
        /// <param name="centerConfig"></param>
        private static void InitConfig(CenterConfig centerConfig)
        {
            _centerConfig = centerConfig;
            string message = "Config loaded: " + JsonSerializer.Serialize(_centerConfig);
            Console.WriteLine(message);
        }

        /// <summary>
        /// tìm ra đường dẫn config chung
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        private static string GetCommonConfigFilePath(IHostEnvironment env)
        {
            string folder = env.ContentRootPath,
                filePath;

            Console.WriteLine($"Root path: <{folder}>.");

            string fileName = "appsettings.json";

            do
            {
                filePath = Path.Combine(folder, "Config", env.EnvironmentName, fileName);

                if (!File.Exists(filePath))
                {
                    folder = Directory.GetParent(folder).FullName;
                }

            }
            while (!File.Exists(filePath));

            return filePath;
        }
    }
}
