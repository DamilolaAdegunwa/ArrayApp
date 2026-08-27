using ArrayApp.Application.Ideas.Commands;

namespace ArrayApp.Application.UnitTests.Ideas.Commands;

[TestFixture]
public class GenerateProvenanceCertificateCommandTests
{
    [Test]
    public void GenerateProvenanceCertificateCommand_Initialization_IssuerDidSet()
    {
        var command = new GenerateProvenanceCertificateCommand(50, "did:arrayapp:corporate:governance");
        command.IdeaId.Should().Be(50);
        command.IssuerDid.Should().Be("did:arrayapp:corporate:governance");
    }

    [Test]
    public void W3CVerifiableCertificateDto_SetsDefaultContextAndType()
    {
        var cert = new W3CVerifiableCertificateDto
        {
            Id = "urn:uuid:12345",
            Issuer = "did:arrayapp:org"
        };

        cert.Context.Should().Contain("https://www.w3.org/2018/credentials/v1");
        cert.Type.Should().Contain("VerifiableCredential");
        cert.Id.Should().Be("urn:uuid:12345");
    }
}
