using KafkaModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

IHostEnvironment hostEnvironment = builder.Environment;

string configPath = ConfigUtil.GetCommonConfigFilePath(hostEnvironment);

builder.Configuration.AddJsonFile(configPath, optional: false, reloadOnChange: true);

var centerConfig = new CenterConfig();
builder.Configuration.Bind(centerConfig);

ConfigUtil.InitConfig(centerConfig);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
