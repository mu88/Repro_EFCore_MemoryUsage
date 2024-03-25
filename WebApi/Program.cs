using Core;
using Microsoft.AspNetCore.Mvc;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<CustomBackgroundService>();
builder.Services.AddSingleton<Processor>();
builder.Services.ConfigureDatabase(builder.Configuration.GetConnectionString("AppDatabase") ?? throw new ArgumentNullException());

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

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

app.MapPost("/enable", (Processor processor) =>
    {
        processor.ProcessingEnabled = true;

        return Results.Empty;
    })
    .WithOpenApi();

app.MapPost("/disable", (Processor processor) =>
    {
        processor.ProcessingEnabled = false;

        return Results.Empty;
    })
    .WithOpenApi();

app.Run();