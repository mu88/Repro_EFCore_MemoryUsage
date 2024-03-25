using Microsoft.Extensions.Hosting;

namespace Core;

public class CustomBackgroundService : BackgroundService
{
    private readonly Processor _processor;

    public CustomBackgroundService(Processor processor) => _processor = processor;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (true)
        {
            await _processor.ProcessAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}