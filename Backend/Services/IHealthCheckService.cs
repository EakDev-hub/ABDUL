using AbdulBackend.Models;

namespace AbdulBackend.Services;

public interface IHealthCheckService
{
    Task<List<HealthCheck>> GetHealthChecksAsync();
    Task<HealthCheck?> UpdateHealthCheckAsync(UpdateHealthCheckDto dto);
}