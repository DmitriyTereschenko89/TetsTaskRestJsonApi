using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using TetsTaskRestJsonApi.Connectors;
using TetsTaskRestJsonApi.Models;
using TetsTaskRestJsonApi.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ProducesAttribute("application/json"));
});

builder.Services.AddTransient<IDbConnection, MySqlConnection>();

builder.Services.AddTransient<IDBPositionConnector, DBPositionConnector>();
builder.Services.AddTransient<IPositionRepository, PositionRepository>();

builder.Services.AddTransient<IDBEmployeeConnector, DBEmployeeConnector>();
builder.Services.AddTransient<IEmployeeRepository, EmployeeRepository>();

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
