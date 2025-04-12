﻿using Confluent.Kafka;
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

        IConsumer<string, string> _consumer;

        private int MaxThread = 1; // số luồng tối đa

        private object _lockTask = new object();

        private List<Task> _tasks = new List<Task>();

        private int _ThreadCount = 0; // số luồng đang chạy
        #endregion


        #region Constructor
        public KafkaNonSequenceConsumer(KafkaSubcribleConfig config)
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
                    HandleMessage(config, cr);
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

        private void HandleMessage(KafkaSubcribleConfig config, ConsumeResult<string, string> cr)
        {

            if (_tasks.Count < MaxThread)
            {
                string taskName = $"Task {_ThreadCount + 1}";

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

        private void TaskHandleMessage(KafkaSubcribleConfig config, ConsumeResult<string, string> cr)
        {
            LogQueueUtil.ConsoleLog(config, cr);
            Task.Delay(5000).Wait(); // giả lập thời gian xử lý message
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
        }

        #endregion

        #region Methods


        #endregion
    }
}