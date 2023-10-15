using Microsoft.EntityFrameworkCore;
using System.Reflection;
using ThermalLog;
using ThermalLog.Models;
using NLog.Web;
using NLog;
using ThermalLog.Migrations;
using Microsoft.Extensions.Hosting;
using Microsoft.Data.SqlClient;
using System.Data.Common;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Logging.ClearProviders();
builder.Host.UseNLog();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if( string.IsNullOrEmpty(connectionString)) {
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
} else
{
    logger.Info("Database connection string = {ConnectionString}", connectionString);
    builder.Services.AddDbContext<ThermalDbContext>(options =>
        options.UseSqlServer(connectionString)
    );

}


builder.Services.Configure<RouteOptions>((route) =>
{
    route.LowercaseUrls = true;
});
// APIのエンドポイントのプリフィックスを設定
builder.Services.AddControllers(o => {
    o.UseRoutePrefix("api");
});

builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
var config = app.Services.GetRequiredService<IConfiguration>();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ThermalDbContext>();
    try
    {
        context.Database.Migrate();
    }
    catch (DbException e)
    {
        logger.Warn("DB設定中に例外が発生しました。", e);
    }
    var loader = new DbInitialLoader(context);
    var dir = config.GetValue<string>("InitialDataDirectory");
    if(context.Thermals.Count()<1 && Directory.Exists(dir))
    {
        logger.Info($"Load InitialData directory={dir}");
        loader.Load(dir);
    }
}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseDefaultFiles();
app.UseStaticFiles();

//app.UseAuthorization();
app.MapControllers();

app.Run();
