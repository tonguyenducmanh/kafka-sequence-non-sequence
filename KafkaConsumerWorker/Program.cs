using KafkaConsumerWorker;
using KafkaModel;

var builder = Host.CreateApplicationBuilder(args);

ConfigUtil.InitGlobalConfig(builder);

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
