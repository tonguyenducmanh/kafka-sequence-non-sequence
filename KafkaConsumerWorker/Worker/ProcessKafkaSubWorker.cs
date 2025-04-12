using KafkaCore;
using KafkaModel;

namespace KafkaConsumerWorker
{
    /// <summary>
    /// process xử lý các message từ kafka
    /// </summary>
    public class ProcessKafkaSubWorker : BackgroundService
    {
        private readonly ILogger<ProcessKafkaSubWorker> _logger;
        
        public ProcessKafkaSubWorker(ILogger<ProcessKafkaSubWorker> logger)
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
