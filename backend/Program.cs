using backend;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsSettings", builder =>
    {
        builder.WithOrigins("http://localhost:3000")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection(nameof(MongoDBSettings)));

builder.Services.AddSingleton(_ =>
{
    string connectionString = Environment.GetEnvironmentVariable("MongoDBConnection") ?? throw new Exception("Connection string must be provided");
    string dbName = Environment.GetEnvironmentVariable("DatabaseName") ?? throw new Exception("Database name must be provided");

    return new MongoDBContext(connectionString, dbName);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("CorsSettings");
app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();