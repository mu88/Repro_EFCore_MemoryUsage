using Microsoft.Extensions.DependencyInjection;

namespace Core;

public class BulkProcessor
{
    private readonly IServiceProvider _serviceProvider;

    public BulkProcessor(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public bool ProcessingEnabled { get; set; } = true;

    public async Task ProcessAsync(CancellationToken ct)
    {
        if (!ProcessingEnabled) return;

        using IServiceScope serviceScope = _serviceProvider.CreateScope();
        var processor = serviceScope.ServiceProvider.GetRequiredService<Processor>();

        List<Task> processingTasks = new();
        for (var i = 0; i < 10; i++) processingTasks.Add(processor.ProcessAsync(ct));

        await Task.WhenAll(processingTasks);
    }
}