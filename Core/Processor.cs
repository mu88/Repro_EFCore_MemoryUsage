using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Core;

public class Processor
{
    private readonly IDbContextFactory<MyDbContext> _factory;
    private readonly ILogger<Processor> _logger;

    public Processor(IDbContextFactory<MyDbContext> factory, ILogger<Processor> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public async Task ProcessAsync(CancellationToken ct)
    {
        MyDbContext context = _factory.CreateDbContext();

        IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync(ct);

            var entities = await context.Set<InteractionPoint>()
                .Where(entity => entity.State == ProcessState.Received)
                .OrderBy(entity => entity.Key)
                .Take(1000)
                .TagWith(DbCommandTags.SkipLockedRows)
                .AsTracking()
                .ToListAsync(ct);
            await HandleEntitiesAsync(entities, ct);

            await context.SaveChangesAsync(ct);
            context.ChangeTracker.Clear();
            await transaction.CommitAsync(ct);
        });
    }

    private async Task HandleEntitiesAsync(List<InteractionPoint> entities, CancellationToken ct)
    {
        foreach (InteractionPoint entity in entities)
        {
            var interactionPointModel = JsonConvert.DeserializeObject<InteractionPointModel>(entity.Content);
            // Process the entity
            entity.State = ProcessState.Processed;
            _logger.LogInformation("Entity {EntityKey} processed", entity.Key);
        }
    }
}