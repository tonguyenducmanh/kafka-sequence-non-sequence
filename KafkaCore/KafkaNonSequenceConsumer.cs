using Confluent.Kafka;
using KafkaModel;

namespace KafkaCore
{
    /// <summary>
    /// KafkaConsumer là lớp dùng để tiêu thụ message từ kafka
    /// lớp này xử lý không tuần tự
    /// </summary>
    public class KafkaNonSequenceConsumer
    {
        #region Declare
        
        IConsumer<Ignore, string> _consumer;


        #endregion


        #region Constructor
        public KafkaNonSequenceConsumer(KafkaSubcribleConfig config) {
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
                    Console.WriteLine($"{config.MachineName} Nhận message: {cr.Message.Value} từ topic {cr.Topic}, partition {cr.Partition}, offset {cr.Offset}");
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
