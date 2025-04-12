using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafkaModel
{
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
        /// khởi tạo config
        /// </summary>
        /// <param name="centerConfig"></param>
        public static void InitConfig(CenterConfig centerConfig)
        {
            _centerConfig = centerConfig;
        }

        /// <summary>
        /// tìm ra đường dẫn config chung
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public static string GetCommonConfigFilePath(IHostEnvironment env)
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
