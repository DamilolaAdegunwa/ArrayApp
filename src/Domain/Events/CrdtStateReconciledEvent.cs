namespace ArrayApp.Domain.Events;

public class CrdtStateReconciledEvent : BaseEvent
{
    public CrdtStateReconciledEvent(int ideaId, string clientId, int operationsAppliedCount, long serverSequence)
    {
        IdeaId = ideaId;
        ClientId = clientId;
        OperationsAppliedCount = operationsAppliedCount;
        ServerSequence = serverSequence;
    }

    public int IdeaId { get; }
    public string ClientId { get; }
    public int OperationsAppliedCount { get; }
    public long ServerSequence { get; }
}
