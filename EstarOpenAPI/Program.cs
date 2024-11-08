using EstarOpenAPI.Application.Interfaces;
using EstarOpenAPI.Infrastructure.Identity.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IHomePageService, HomePageService>();
builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.ListenAnyIP(44386, listenOptions =>
    {
        listenOptions.UseHttps();  // HTTPS
    });
    options.ListenAnyIP(5189); // HTTP
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
