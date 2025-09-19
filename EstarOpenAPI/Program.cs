using Application.Interfaces;
using Infrastructure.Identity.Services;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IHomePageService, HomePageService>();
builder.Services.AddScoped<IPayService, PayService>();
builder.Services.AddScoped<IAccountService, Infrastructure.Identity.Services.AccountService>();
builder.Services.AddScoped<IServicesService, ServicesService>();
builder.Services.AddScoped<ITaskHandlerSservice, TaskHandlerSservice>();
builder.Services.AddScoped<ITasksService, TasksService>();
builder.Services.AddScoped<IGoogleFileService, GoogleFileService>();
builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.ListenAnyIP(44386, listenOptions =>
    {
        listenOptions.UseHttps();  // HTTPS
    });
    options.ListenAnyIP(5189); // HTTP
});



StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];  // Set your Secret API Key here

builder.Services.AddControllers();


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
