using KafkaConsumerWorker;
using KafkaModel;

var builder = Host.CreateApplicationBuilder(args);

ConfigUtil.InitGlobalConfig(builder);

builder.Services.AddHostedService<ProcessKafkaSubWorker>();

var host = builder.Build();
host.Run();
