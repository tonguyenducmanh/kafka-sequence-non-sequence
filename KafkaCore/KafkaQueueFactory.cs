using Confluent.Kafka;

namespace KafkaCore
{
    public class KafkaQueueFactory
    {
        /// <summary>
        /// quản lý connection tới kafka, mỗi máy chủ kafka sẽ có 
        /// 1 connection để tối ưu
        /// </summary>
        internal static Dictionary<string, IProducer<string, string>> _producers = new Dictionary<string, IProducer<string, string>>();

        /// <summary>
        /// đối tượng lock để khởi tạo đối tượng
        /// </summary>
        static object _lockGet = new object();

        /// <summary>
        /// Lấy producer từ dictionary, nếu không có thì tạo mới
        /// </summary>
        /// <param name="config">cấu hình</param>
        public static IProducer<string, string> GetProducer(ProducerConfig config)
        {
            string key = config.BootstrapServers.Trim().ToLower();
            if (_producers.ContainsKey(key))
            {
                return _producers[key];
            }
            else
            {
                lock (_lockGet)
                {
                    if (_producers.ContainsKey(key))
                    {
                        return _producers[key];
                    }
                    // Nếu không có, tạo mới và thêm vào dictionary
                    IProducer<string, string>? producer = new ProducerBuilder<string, string>(config).Build();
                    _producers.Add(key, producer);
                    return producer;
                }
            }
        }


    }
}
