using Confluent.Kafka;
using KafkaModel;

namespace KafkaCore
{
    /// <summary>
    /// xử lý queue kafka không tuần tự dù queue trả về có theo routingkey
    /// </summary>
    public class KafkaNonSequenceConsumer
    {
        #region Declare

        /// <summary>
        /// consumer sẽ dùng để đọc queue, tránh khởi tạo nhiều lần
        /// </summary>
        IConsumer<string, string> _consumer;

        /// <summary>
        /// số thread tối đa được phép chạy
        /// </summary>
        private int MaxThread = 1;

        /// <summary>
        /// đối tượng dùng để lock khi add task
        /// </summary>
        private object _lockTask = new object();

        /// <summary>
        /// danh sách các task đang chạy
        /// </summary>
        private List<Task> _tasks = new List<Task>();

        /// <summary>
        /// số luồng đang chạy
        /// </summary>
        private int _ThreadCount = 0;

        /// <summary>
        /// config của consumer
        /// </summary>
        private KafkaSubcribleConfig _config;
        #endregion


        #region Constructor

        /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        /// <param name="config"></param>
        public KafkaNonSequenceConsumer(KafkaSubcribleConfig config)
        {
            InitConsumer(config);
        }
        #endregion

        #region Methods
        /// <summary>
        /// xử lý các message trong queue nhận được từ kafka
        /// </summary>
        public void ProcessDequeueKafka()
        {
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
                    HandleMessage(_config, cr);
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


        /// <summary>
        /// xử lý 1 message nhận được từ queue kafka
        /// </summary>
        /// <param name="cr"></param>
        private void HandleMessage(KafkaSubcribleConfig config, ConsumeResult<string, string> cr)
        {

            if (_tasks.Count < MaxThread)
            {
                string taskName = $"Task {_ThreadCount + 1}";

                // mỗi message nhận được sẽ tạo ra 1 task để xử lý
                Task processTask = new Task(() =>
                {
                    try
                    {
                        Thread.CurrentThread.Name = taskName;
                        TaskHandleMessage(config, cr);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });

                lock (_lockTask)
                {
                    _tasks.Add(processTask);
                    _ThreadCount++;
                    if (_ThreadCount == MaxThread)
                    {
                        _ThreadCount = 0;
                    }
                }

                processTask.Start();

                // nếu số luồng đang chạy bằng số luồng tối đa thì chờ cho 1 luồng hoàn thành
                if (_tasks.Count == MaxThread)
                {
                    // task chạy xong thì xóa khỏi danh sách task đang chạy
                    int idx = Task.WaitAny(_tasks.ToArray());
                    lock (_lockTask)
                    {
                        _tasks.RemoveAt(idx);
                    }
                }
            }
        }

        /// <summary>
        /// xử lý nghiệp vụ trong 1 task
        /// </summary>
        /// <param name="key"></param>
        /// <param name="taskName"></param>

        private void TaskHandleMessage(KafkaSubcribleConfig config, ConsumeResult<string, string> cr)
        {
            LogQueueUtil.ConsoleLog(config, cr);
            int delay = ConfigUtil.CenterConfig.TaskDelay * 1000;
            Task.Delay(delay).Wait(); // giả lập thời gian xử lý message
        }

        /// <summary>
        /// khởi tạo consumer
        /// </summary>
        /// <param name="config"></param>
        private void InitConsumer(KafkaSubcribleConfig config)
        {
            MaxThread = config.MaxThread;
            ConsumerConfig consumerConfig = new ConsumerConfig
            {
                BootstrapServers = config.BootstrapServers,
                GroupId = config.GroupId
            };
            _consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
            _consumer.Subscribe(config.Topic);
            _config = config;
        }

        #endregion

        #region Methods


        #endregion
    }
}