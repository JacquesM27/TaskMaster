namespace TaskMaster.Abstractions.Events;

public sealed record EventMetadata(
    Guid EventId,
    string EventType,
    DateTime CreatedAt,
    string Source,
    int Version = 1);