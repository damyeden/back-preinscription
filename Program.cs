using Microsoft.EntityFrameworkCore;
using backPreinscription.Models;
using backPreinscription.Data;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    x.JsonSerializerOptions.WriteIndented = true;
});

var dataSourceBuilder = new NpgsqlDataSourceBuilder(
    builder.Configuration.GetConnectionString("DefaultConnection")
);
dataSourceBuilder.MapEnum<ModeInscriptionEnum>("type_mode_preinscription");

var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<PreinscriptionDbContext>(
    options => options.UseNpgsql(dataSource)
);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

// app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

