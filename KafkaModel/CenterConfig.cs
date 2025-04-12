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
        public KafkaConfig KafkaPublishConfig { get; set; } = new KafkaConfig();
        public KafkaSubcribleConfig KafkaSubcribleConfig { get; set; } = new KafkaSubcribleConfig();

        public bool UsingSequence { get; set; } =  false; // sử dụng sequence hay không

    }

    /// <summary>
    /// cấu hình kafka
    /// </summary>
    public class KafkaConfig
    {
        public string BootstrapServers { get; set; }
        public string Topic { get; set; }
        public string GroupId { get; set; }
    }

    /// <summary>
    /// cấu hình kafka
    /// </summary>
    public class KafkaSubcribleConfig
    {
        public string BootstrapServers { get; set; }
        public List<string> Topic { get; set; }
        public string GroupId { get; set; }

        public string MachineName { get; set; } // tên máy

        public int MaxThread { get; set; }
    }
}
