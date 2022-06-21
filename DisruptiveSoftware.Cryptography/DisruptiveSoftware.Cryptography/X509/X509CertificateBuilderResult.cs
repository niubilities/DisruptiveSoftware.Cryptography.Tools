namespace DisruptiveSoftware.Cryptography.X509
{
  using System.Security;
  using DisruptiveSoftware.Cryptography.BouncyCastle.Extensions;
  using Org.BouncyCastle.Crypto;
  using Org.BouncyCastle.X509;

  public class X509CertificateBuilderResult
  {
    public X509CertificateBuilderResult(X509Certificate certificate, AsymmetricKeyParameter privateKey)
    {
      Certificate = certificate;
      PrivateKey = privateKey;
    }

    public X509Certificate Certificate { get; }

    protected AsymmetricKeyParameter PrivateKey { get; }

    public byte[] ExportCertificate(SecureString? password, string alias = "Certificate")
    {
      return Certificate.ExportCertificate(password, PrivateKey, alias);
    }

    public byte[] ExportToPfx(SecureString? password)
    {
      return Certificate.ExportToPfx(password);
    }
  }
}