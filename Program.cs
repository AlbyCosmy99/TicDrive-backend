using Autofac;
using Autofac.Extensions.DependencyInjection;
using TicDrive.AppConfig;
using TicDrive.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
.ConfigureContainer<ContainerBuilder>(autofacBuilder =>
{
    autofacBuilder.RegisterModule(new AutofacModule());
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var connection = string.Empty;
if(builder.Environment.IsDevelopment())
{
    builder.Configuration.AddEnvironmentVariables().AddJsonFile("appsettings.Development.json");
    connection = builder.Configuration.GetConnectionString("TICDRIVE_SQL_CONNECTIONSTRING");
} else
{
    connection = Environment.GetEnvironmentVariable("TICDRIVE_SQL_CONNECTIONSTRING");
}

builder.Services.AddDbContext<TicDriveDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TICDRIVE_SQL_CONNECTIONSTRING")));


builder.Services.AddAutoMapper(typeof(AutomapperConfig));

var app = builder.Build();

app.UseCors("AllowAll");

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
