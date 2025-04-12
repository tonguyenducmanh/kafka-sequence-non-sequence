using KafkaCore;
using KafkaModel;

namespace KafkaSequenceWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        
        private readonly string MachineName = nameof(KafkaSequenceWorker);

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            _ = Task.Run(() =>
            {
                KafkaSubcribleConfig kafkaConfig = ConfigUtil.CenterConfig.KafkaSubcribleConfig;
                kafkaConfig.MachineName = MachineName;
                KafkaSequenceConsumer kafkaConsumer = new KafkaSequenceConsumer(kafkaConfig);
            });
        }
    }
}
