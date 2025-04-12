using KafkaCore;
using KafkaModel;

namespace KafkaConsumerWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            _ = Task.Run(() =>
            {
                // dựa vào config, tùy chọn xem muốn hiển thị tuần tự hay không tuần tự
                if(ConfigUtil.CenterConfig.UsingSequence)
                {
                    KafkaSubcribleConfig kafkaConfig = ConfigUtil.CenterConfig.KafkaSubcribleConfig;
                    kafkaConfig.MachineName = "Worker chạy tuần tự";
                    KafkaSequenceConsumer kafkaConsumer = new KafkaSequenceConsumer(kafkaConfig);
                    kafkaConsumer.ProcessDequeueKafka();
                }
                else
                {
                    KafkaSubcribleConfig kafkaConfig = ConfigUtil.CenterConfig.KafkaSubcribleConfig;
                    kafkaConfig.MachineName = "Worker chạy không tuần tự";
                    KafkaNonSequenceConsumer kafkaConsumer = new KafkaNonSequenceConsumer(kafkaConfig);
                    kafkaConsumer.ProcessDequeueKafka();
                }
            });
        }
    }
}
