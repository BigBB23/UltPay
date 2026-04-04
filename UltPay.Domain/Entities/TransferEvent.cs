
namespace UltPay.Domain.Entities;

public class TransferEvent
{
    public Guid Id { get; set; }
    public Guid TransferId { get; set; }
    public string OldStatus { get; set; } = string.Empty;
    public string NewStatus { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string ProviderStatus { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
