using Confluent.Kafka;
using KafkaModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafkaCore
{
    public class LogQueueUtil
    {
        public static void ConsoleLog(KafkaSubcribleConfig config, ConsumeResult<string, string> cr)
        {
            //Console.WriteLine($"{config.MachineName} {Thread.CurrentThread.Name} Nhận message: {cr.Message.Value} từ topic {cr.Topic}, partition {cr.Partition}, offset {cr.Offset}");
            Console.WriteLine($"{config.MachineName} {Thread.CurrentThread.Name} Nhận message: {cr.Message.Value} từ topic {cr.Topic}, sequence theo key {cr.Message.Key}");
        }
    }
}
