using KafkaCore;
using KafkaModel;

namespace KafkaNonSequenceWorker
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
            _ = Task.Run(() =>
            {
                KafkaSubcribleConfig kafkaConfig = ConfigUtil.CenterConfig.KafkaSubcribleConfig;
                KafkaConsumer kafkaConsumer = new KafkaConsumer(kafkaConfig);
            });
        }
    }
}
