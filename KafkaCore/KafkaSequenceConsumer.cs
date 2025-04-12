using Confluent.Kafka;
using KafkaModel;
using System.Collections.Concurrent;

namespace KafkaCore
{
    /// <summary>
    /// xử lý queue kafka tuần tự theo routingkey
    /// </summary>
    public class KafkaSequenceConsumer
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
        /// số luồng đang chạy
        /// </summary>
        private int _ThreadCount = 0;

        /// <summary>
        /// số lần lặp tối đa tránh stack overflow
        /// </summary>
        private int _maxLoopBreak = 1000;

        /// <summary>
        /// config của consumer
        /// </summary>
        private KafkaSubcribleConfig _config;


        /// <summary>
        /// danh sách các message cần xử lý theo key sequence
        /// </summary>
        private ConcurrentDictionary<string, ConcurrentQueue<ConsumeResult<string, string>>> _dicQueueBySequence { get; set; } = new ConcurrentDictionary<string, ConcurrentQueue<ConsumeResult<string, string>>>();

        /// <summary>
        /// danh sách các task tương ứng theo key sequence
        /// </summary>
        private ConcurrentDictionary<string, Task> _dicTaskManager { get; set; } = new ConcurrentDictionary<string, Task>();

        #endregion


        #region Constructor

        /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        /// <param name="config"></param>
        public KafkaSequenceConsumer(KafkaSubcribleConfig config)
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
                    HandleMessage(cr);
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
        private void HandleMessage(ConsumeResult<string, string> cr)
        {
            string key = GetKeyForDictionary(cr);
            AddSequenceMessage(key, cr);
            if(_dicTaskManager.Count == MaxThread)
            {
                // chờ 1 task complete rồi mới thực hiện tiếp
                int idx = Task.WaitAny(_dicTaskManager.Values.ToArray());
            }
        }

        /// <summary>
        /// thêm message vào hàng đợi xử lý của task theo key là routingkey
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cr"></param>
        /// <returns></returns>
        private bool AddSequenceMessage(string key, ConsumeResult<string, string> cr)
        {
            lock (_lockTask)
            {
                if (_dicQueueBySequence.ContainsKey(key))
                {
                    _dicQueueBySequence[key].Enqueue(cr);
                }
                else
                {
                    ConcurrentQueue<ConsumeResult<string, string>> queue = new ConcurrentQueue<ConsumeResult<string, string>>();
                    queue.Enqueue(cr);
                    _dicQueueBySequence.TryAdd(key, queue);
                    AddTask(key);
                }
            }
            return true;
        }

        /// <summary>
        /// xóa task ra khỏi dictionary quản lý
        /// </summary>
        private bool RemoveSequenceMessage(string key)
        {
            lock (_lockTask)
            {
                if (_dicQueueBySequence.ContainsKey(key))
                {
                    _dicQueueBySequence.TryRemove(key, out _);
                    _dicTaskManager.TryRemove(key, out _);
                    _ThreadCount--;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// thêm 1 task xử lý đặc thù cho 1 routing key
        /// </summary>
        /// <param name="key"></param>
        private void AddTask(string key)
        {
            if (_dicTaskManager.ContainsKey(key))
            {
                Console.WriteLine($"Vẫn còn task đang chạy theo key {key}");
            }
            else
            {
                _ThreadCount++;
                string taskName = $"Task {_ThreadCount + 1}";

                Task processTask = new Task(() =>
                {
                    try
                    {
                        Thread.CurrentThread.Name = taskName;
                        TaskHandleMessage(key, taskName);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });

                processTask.ContinueWith(t =>
                {
                    try
                    {
                        // chạy xong là dọn luôn task khỏi dictionary
                        RemoveSequenceMessage(key);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                });

                _dicTaskManager.TryAdd(key, processTask);

                processTask.Start();
            }
        }

        /// <summary>
        /// lấy ra routingkey được cấu hình từ message
        /// </summary>
        /// <param name="cr">mesasge nhận được từ kafka</param>
        /// <returns></returns>
        private string GetKeyForDictionary(ConsumeResult<string, string> cr)
        {
            // lấy key từ message
            return cr.Message.Key.ToString() ?? string.Empty;
        }

        /// <summary>
        /// xử lý nghiệp vụ trong 1 task
        /// </summary>
        /// <param name="key"></param>
        /// <param name="taskName"></param>
        private void TaskHandleMessage(string key, string taskName)
        {
            if (!_dicQueueBySequence.ContainsKey(key))
            {
                Console.WriteLine($"Không tìm thấy message theo key {key}");
                return;
            }
            else
            {
                int i = 0;
                ConcurrentQueue<ConsumeResult<string, string>>? queueData = _dicQueueBySequence[key];

                // chạy vòng while để xử lý tuần tự từng message trong queue
                while (queueData.Count > 0)
                {
                    i++;
                    ConsumeResult<string, string>? cr;
                    queueData.TryDequeue(out cr);

                    // xử lý nghiệp vụ trong queue
                    LogQueueUtil.ConsoleLog(_config, cr);

                    // giả lập thời gian xử lý message
                    int delay = ConfigUtil.CenterConfig.TaskDelay * 1000;
                    Task.Delay(delay).Wait(); // giả lập thời gian xử lý message
                    if (i > _maxLoopBreak)
                    {
                        Console.WriteLine($"Vượt quá số lần lặp tối đa {_maxLoopBreak}");
                        break;
                    }
                }
            }
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
