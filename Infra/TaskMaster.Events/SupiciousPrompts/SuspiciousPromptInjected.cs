using TaskMaster.Abstractions.Events;

namespace TaskMaster.Events.SupiciousPrompts;

public sealed record SuspiciousPromptInjected(IEnumerable<string> Reasons) : IEvent;