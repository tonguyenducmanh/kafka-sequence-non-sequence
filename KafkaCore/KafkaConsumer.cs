using Confluent.Kafka;
using KafkaModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Confluent.Kafka.ConfigPropertyNames;

namespace KafkaCore
{
    /// <summary>
    /// KafkaConsumer là lớp dùng để tiêu thụ message từ kafka
    /// </summary>
    public class KafkaConsumer
    {
        #region Declare
        
        IConsumer<Ignore, string> _consumer;


        #endregion


        #region Constructor
        public KafkaConsumer(KafkaSubcribleConfig config) {
            ConsumerConfig consumerConfig = new ConsumerConfig
            {
                BootstrapServers = config.BootstrapServers,
                GroupId = config.GroupId
            };
            _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
            _consumer.Subscribe(config.Topic);
            CancellationTokenSource cts = new CancellationTokenSource();

            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    var cr = _consumer.Consume(cts.Token);
                    Console.WriteLine($"Nhận message: {cr.Message.Value} từ topic {cr.Topic}, partition {cr.Partition}, offset {cr.Offset}");
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Consumer dừng lại.");
            }
            finally
            {
                _consumer.Close();
            }
        }

        #endregion

        #region Methods


        #endregion
    }
}
