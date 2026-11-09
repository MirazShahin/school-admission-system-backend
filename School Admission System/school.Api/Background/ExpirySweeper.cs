using School.Application.Abstractions;

namespace School.Api.Background;

public sealed class ExpirySweeper(IServiceScopeFactory scopes, ILogger<ExpirySweeper> log)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                // Repositories are scoped, so each sweep gets its own scope.
                using var scope = scopes.CreateScope();
                var holds = scope.ServiceProvider.GetRequiredService<IHoldRepository>();
                var clock = scope.ServiceProvider.GetRequiredService<IClock>();

                await holds.ExpireAllStaleAsync(clock.UtcNow, stoppingToken);
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                log.LogError(ex, "Expiry sweep failed");
            }
        }
    }
}