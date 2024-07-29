using Microsoft.OpenApi.Models;
using DsStorage.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Storahe API", Version = "v1" });
});
builder.Services.AddSwaggerDocument();


builder.Services.AddOptions<StorageOptions>()
    .Bind(builder.Configuration.GetSection(StorageOptions.SECTION))
    .ValidateDataAnnotations();

builder.Services.AddSingleton(x => {
    var options = builder.Configuration.GetSection(StorageOptions.SECTION).Get<StorageOptions>() ?? throw new("No storage options");
    return Supabase.StatelessClient.Storage(options.Url, options.Key);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(builder =>
    builder.WithOrigins("http://localhost:1420", "http://localhost:5215", "http://localhost:5216", "http://localhost:5217", "http://localhost:5218")
            .AllowAnyHeader()
            .AllowAnyMethod()
);
app.MapControllers();
app.Run();