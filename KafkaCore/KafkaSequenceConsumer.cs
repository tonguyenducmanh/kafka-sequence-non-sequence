using Confluent.Kafka;
using KafkaModel;
using System.Collections.Concurrent;
using System.Diagnostics.Tracing;

namespace KafkaCore
{
    /// <summary>
    /// KafkaConsumer là lớp dùng để tiêu thụ message từ kafka
    /// lớp này xử lý tuần tự
    /// </summary>
    public class KafkaSequenceConsumer
    {
        #region Declare

        IConsumer<string, string> _consumer;

        private int MaxThread = 1; // số luồng tối đa

        private object _lockTask = new object();

        private List<Task> _tasks = new List<Task>();

        private int _ThreadCount = 0; // số luồng đang chạy

        // số lần lặp tối đa tránh stack overflow
        private int _maxLoopBreak = 1000;

        private KafkaSubcribleConfig _config;
        /// <summary>
        /// danh sách các message cần xử lý theo key sequence
        /// </summary>
        private ConcurrentDictionary<string, ConcurrentQueue<ConsumeResult<string, string>>> _dicMessage { get; set; } = new ConcurrentDictionary<string, ConcurrentQueue<ConsumeResult<string, string>>>();

        /// <summary>
        /// danh sách các task tương ứng theo key sequence
        /// </summary>
        private ConcurrentDictionary<string, Task> _dicTask { get; set; } = new ConcurrentDictionary<string, Task>();

        #endregion


        #region Constructor
        public KafkaSequenceConsumer(KafkaSubcribleConfig config)
        {
            InitConsumer(config);
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

        private void HandleMessage(ConsumeResult<string, string> cr)
        {
            string key = GetKeyForDictionary(cr);
            AddSequenceMessage(key, cr);
            if(_dicTask.Count == MaxThread)
            {
                // chờ 1 task complete rồi mới thực hiện tiếp
                int idx = Task.WaitAny(_dicTask.Values.ToArray());
            }
        }

        private bool AddSequenceMessage(string key, ConsumeResult<string, string> cr)
        {
            lock (_lockTask)
            {
                if (_dicMessage.ContainsKey(key))
                {
                    _dicMessage[key].Enqueue(cr);
                }
                else
                {
                    ConcurrentQueue<ConsumeResult<string, string>> queue = new ConcurrentQueue<ConsumeResult<string, string>>();
                    queue.Enqueue(cr);
                    _dicMessage.TryAdd(key, queue);
                    AddTask(key);
                }
            }
            return true;
        }


        private bool RemoveSequenceMessage(string key)
        {
            lock (_lockTask)
            {
                if (_dicMessage.ContainsKey(key))
                {
                    _dicMessage.TryRemove(key, out _);
                    _dicTask.TryRemove(key, out _);
                    _ThreadCount--;
                    return true;
                }
            }
            return false;
        }

        private void AddTask(string key)
        {
            if (_dicTask.ContainsKey(key))
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

                _dicTask.TryAdd(key, processTask);

                processTask.Start();
            }
        }

        private string GetKeyForDictionary(ConsumeResult<string, string> cr)
        {
            // lấy key từ message
            return cr.Message.Key.ToString() ?? string.Empty;
        }

        private void TaskHandleMessage(string key, string taskName)
        {
            if (!_dicMessage.ContainsKey(key))
            {
                Console.WriteLine($"Không tìm thấy message theo key {key}");
                return;
            }
            else
            {
                int i = 0;
                ConcurrentQueue<ConsumeResult<string, string>>? queueData = _dicMessage[key];
                while (queueData.Count > 0)
                {
                    i++;
                    ConsumeResult<string, string>? cr;
                    queueData.TryDequeue(out cr);
                    LogQueueUtil.ConsoleLog(_config, cr);
                    Task.Delay(5000).Wait(); // giả lập thời gian xử lý message
                    if (i > _maxLoopBreak)
                    {
                        Console.WriteLine($"Vượt quá số lần lặp tối đa {_maxLoopBreak}");
                        break;
                    }
                }
            }
        }

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
