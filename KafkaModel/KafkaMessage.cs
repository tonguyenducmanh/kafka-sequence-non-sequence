using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafkaModel
{
    /// <summary>
    /// cấu hình queue
    /// </summary>
    public class KafkaMessage
    {
        /// <summary>
        /// nội dung queue
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// key của queue ( dùng để routing message vào worker )
        /// </summary>
        public string Sequency { get; set; }
    }
}
