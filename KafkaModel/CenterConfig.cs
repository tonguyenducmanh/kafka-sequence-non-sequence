using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafkaModel
{
    /// <summary>
    /// cấu hình chung
    /// </summary>
    public class CenterConfig
    {
        /// <summary>
        /// cấu hình publish queue
        /// </summary>
        public KafkaConfig KafkaPublishConfig { get; set; } = new KafkaConfig();

        /// <summary>
        /// cấu hình nhận queue
        /// </summary>
        public KafkaSubcribleConfig KafkaSubcribleConfig { get; set; } = new KafkaSubcribleConfig();

        /// <summary>
        /// sử dụng sequence hay không
        /// </summary>
        public bool UsingSequence { get; set; } =  false;

    }

    /// <summary>
    /// cấu hình kafka
    /// </summary>
    public class KafkaConfig
    {
        /// <summary>
        /// server kafka
        /// </summary>
        public string BootstrapServers { get; set; }

        /// <summary>
        /// topic kafka
        /// </summary>
        public string Topic { get; set; }
    }

    /// <summary>
    /// cấu hình kafka
    /// </summary>
    public class KafkaSubcribleConfig
    {
        /// <summary>
        /// server kafka
        /// </summary>
        public string BootstrapServers { get; set; }

        /// <summary>
        /// topic kafka
        /// </summary>
        public List<string> Topic { get; set; }

        /// <summary>
        /// group id kafka
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// tên máy để console log
        /// </summary>
        public string MachineName { get; set; } // tên máy

        /// <summary>
        /// số luồng tối đa được phép chạy
        /// </summary>
        public int MaxThread { get; set; }
    }
}
