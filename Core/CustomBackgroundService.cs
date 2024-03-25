using Microsoft.Extensions.Hosting;

namespace Core;

public class CustomBackgroundService : BackgroundService
{
    private readonly BulkProcessor _bulkProcessor;

    public CustomBackgroundService(BulkProcessor bulkProcessor) => _bulkProcessor = bulkProcessor;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (true)
        {
            await _bulkProcessor.ProcessAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}