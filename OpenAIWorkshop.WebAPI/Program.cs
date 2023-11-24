using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddTransient<IKernel>((serviceProvider) =>
{
    return new KernelBuilder()
    .WithLoggerFactory(serviceProvider.GetRequiredService<ILoggerFactory>())
    .WithAzureTextCompletionService(
                builder.Configuration["AzureTextCompletionService:ModelId"]!,
                builder.Configuration["AzureTextCompletionService:Endpoint"]!,
                builder.Configuration["AzureTextCompletionService:Key"]!)
    .WithAzureOpenAIChatCompletionService(
            builder.Configuration["AzureChatCompletionService:ModelId"]!,
            builder.Configuration["AzureChatCompletionService:Endpoint"]!,
            builder.Configuration["AzureChatCompletionService:Key"]!)
    .Build();
});


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
