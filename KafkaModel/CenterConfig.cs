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
        public KafkaConfig KafkaConfig { get; set; } = new KafkaConfig();
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
}
