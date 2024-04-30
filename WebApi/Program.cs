using Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<CustomBackgroundService>();
builder.Services.AddSingleton<BulkProcessor>();
builder.Services.AddScoped<Processor>();
builder.Services.ConfigureDatabase(
    builder.Configuration.GetConnectionString("AppDatabase") ?? throw new ArgumentNullException(),
    builder.Configuration.GetValue<bool>("DisablePooledDbContextFactory"));

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/migrate", async (MyDbContext context) =>
    {
        await context.Database.MigrateAsync();

        return Results.Empty;
    })
    .WithOpenApi();

app.MapPost("/prepare", async (MyDbContext context, [FromQuery(Name = "count")] int count) =>
    {
        for (var i = 0; i < count; i++)
        {
            var interactionPoint = new InteractionPoint { State = ProcessState.Received, Content = $$"""{"id": "{{{i}}}"}""" };
            context.InteractionPoints.Add(interactionPoint);
        }

        await context.SaveChangesAsync();

        return Results.Empty;
    })
    .WithOpenApi();

app.MapPost("/enable", (BulkProcessor processor) =>
    {
        processor.ProcessingEnabled = true;

        return Results.Empty;
    })
    .WithOpenApi();

app.MapPost("/disable", (BulkProcessor processor) =>
    {
        processor.ProcessingEnabled = false;

        return Results.Empty;
    })
    .WithOpenApi();

app.Run();