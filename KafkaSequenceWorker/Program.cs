using KafkaModel;
using KafkaSequenceWorker;

var builder = Host.CreateApplicationBuilder(args);

IHostEnvironment hostEnvironment = builder.Environment;

string configPath = ConfigUtil.GetCommonConfigFilePath(hostEnvironment);

builder.Configuration.AddJsonFile(configPath, optional: false, reloadOnChange: true);

var centerConfig = new CenterConfig();
builder.Configuration.Bind(centerConfig);

ConfigUtil.InitConfig(centerConfig);


builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
