namespace ArrayApp.Domain.Events;

public class ProvenanceCertificateIssuedEvent : BaseEvent
{
    public ProvenanceCertificateIssuedEvent(int ideaId, string certificateId, string issuerDid, string subjectDid, string rootHash)
    {
        IdeaId = ideaId;
        CertificateId = certificateId;
        IssuerDid = issuerDid;
        SubjectDid = subjectDid;
        RootHash = rootHash;
    }

    public int IdeaId { get; }
    public string CertificateId { get; }
    public string IssuerDid { get; }
    public string SubjectDid { get; }
    public string RootHash { get; }
}
