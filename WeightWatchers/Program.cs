using Microsoft.EntityFrameworkCore;
using WeightWatchers.Data.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("allowAll",
                          policy =>
                          {
                              policy.AllowAnyHeader().AllowAnyMethod();
                          });
});

var conectionString = builder.Configuration.GetConnectionString("WeightWatchers");
builder.Services.AddDbContext<WeightWatchersContext>(options => options.UseSqlServer(conectionString));


var app = builder.Build();

// Configure the HTTP request pipeline.
//if(app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

#pragma warning disable S1118
public partial class Program { }
#pragma warning restore S1118