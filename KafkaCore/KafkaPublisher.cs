using Confluent.Kafka;
using KafkaModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafkaCore
{
    public class KafkaPublisher
    {
        public IProducer<string, string> _producer;

        /// <summary>
        /// Gửi message lên kafka
        /// </summary>
        /// <param name="message"></param>
        /// <param name="sequency"></param>
        /// <returns></returns>
        public async Task PublishAsync(KafkaConfig config, string message, string sequency)
        {
            if (_producer == null)
            {
                ProducerConfig keyValuePairs = new ProducerConfig
                {
                    BootstrapServers = config.BootstrapServers
                };
                _producer = KafkaQueueFactory.GetProducer(keyValuePairs);
            }
            // Gửi message
            var result = await _producer.ProduceAsync(config.Topic, new Message<string, string> { Key = sequency, Value = message, Headers = new Headers() });
        }
    }
}
