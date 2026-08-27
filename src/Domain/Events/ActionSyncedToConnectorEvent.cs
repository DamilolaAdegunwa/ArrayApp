namespace ArrayApp.Domain.Events;

public class ActionSyncedToConnectorEvent : BaseEvent
{
    public ActionSyncedToConnectorEvent(int actionId, int ideaId, ConnectorType connectorType, string externalReferenceKey, string externalUrl)
    {
        ActionId = actionId;
        IdeaId = ideaId;
        ConnectorType = connectorType;
        ExternalReferenceKey = externalReferenceKey;
        ExternalUrl = externalUrl;
    }

    public int ActionId { get; }
    public int IdeaId { get; }
    public ConnectorType ConnectorType { get; }
    public string ExternalReferenceKey { get; }
    public string ExternalUrl { get; }
}
