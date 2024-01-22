using DentistSchedulerWebApi.Interfaces;
using DentistSchedulerWebApi.Models;
using DentistSchedulerWebApi.Services;

var MyAllowSpecificOrigins = "http://localhost:3000/";


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder.WithOrigins("http://localhost:3000/");
                      });
});


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<DentistSchedulerDatabaseSettings>(
builder.Configuration.GetSection("DentistSchedulerDatabase"));

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IDentistSchedulerService, DentistSchedulerTestService>();
}
else
{
    builder.Services.AddScoped<IDentistSchedulerService, DentistSchedulerService>();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

// TODO: need to fix cors policy in the future.
app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
// app.MapControllers();

app.Run();
