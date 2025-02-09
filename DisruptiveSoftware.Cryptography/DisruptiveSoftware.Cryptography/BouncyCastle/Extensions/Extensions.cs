﻿using SystemX509Certificates = System.Security.Cryptography.X509Certificates;

namespace DisruptiveSoftware.Cryptography.BouncyCastle.Extensions
{
  using System.Security;
  using DisruptiveSoftware.Cryptography.Extensions;
  using Org.BouncyCastle.Asn1;
  using Org.BouncyCastle.Crypto;
  using Org.BouncyCastle.Crypto.Prng;
  using Org.BouncyCastle.Pkcs;
  using Org.BouncyCastle.Security;
  using Org.BouncyCastle.X509;

  public static class Extensions
  {
    public static byte[] ExportCertificate(this X509Certificate x509Certificate, SecureString? password,
      AsymmetricKeyParameter privateKey, string alias = "Certificate")
    {
      var pkcs12Store = new Pkcs12Store();

      var x509CertificateEntry = new X509CertificateEntry(x509Certificate);
      pkcs12Store.SetCertificateEntry(alias, x509CertificateEntry);

      pkcs12Store.SetKeyEntry(alias, new AsymmetricKeyEntry(privateKey), new[] { x509CertificateEntry });

      using var ms = new MemoryStream();
      pkcs12Store.Save(ms, password.ToCharArray(), new SecureRandom(new CryptoApiRandomGenerator()));

      return ms.ToArray();
    }

    public static byte[] ExportPublicKeyCertificate(this X509Certificate x509Certificate)
    {
      var x509Certificate2 = new SystemX509Certificates.X509Certificate2(x509Certificate.GetEncoded());

      return x509Certificate2.Export(SystemX509Certificates.X509ContentType.Cert);
    }

    public static byte[] ExportToPfx(this X509Certificate x509Certificate, SecureString? password)
    {
      var x509Certificate2 = new SystemX509Certificates.X509Certificate2(x509Certificate.GetEncoded());

      return x509Certificate2.Export(SystemX509Certificates.X509ContentType.Pfx, password);
    }

    public static AsymmetricKeyParameter GetPrivateKeyAsAsymmetricKeyParameter(
      this SystemX509Certificates.X509Certificate2 x509Certificate2)
    {
      return DotNetUtilities
        .GetKeyPair(SystemX509Certificates.RSACertificateExtensions.GetRSAPrivateKey(x509Certificate2)).Private;
    }

    public static AsymmetricKeyParameter GetPublicKeyAsAsymmetricKeyParameter(
      this SystemX509Certificates.X509Certificate2 x509Certificate2)
    {
      return DotNetUtilities
        .GetKeyPair(SystemX509Certificates.RSACertificateExtensions.GetRSAPrivateKey(x509Certificate2)).Public;
    }

    public static bool IsSelfSigned(this X509Certificate x509Certificate)
    {
      return x509Certificate.IssuerDN.Equivalent(x509Certificate.SubjectDN);
    }

    public static byte[] ToDEREncoded(this byte[] data)
    {
      using var asn1InputStream = new Asn1InputStream(data);
      var asn1Encodable = asn1InputStream.ReadObject();

      return asn1Encodable.GetDerEncoded();
    }

    public static X509Certificate ToX509Certificate(this SystemX509Certificates.X509Certificate2 x509Certificate2)
    {
      return new X509CertificateParser().ReadCertificate(x509Certificate2.RawData);
    }

    public static X509Certificate ToX509Certificate(this SystemX509Certificates.X509Certificate x509Certificate)
    {
      return new X509CertificateParser().ReadCertificate(x509Certificate.GetRawCertData());
    }

    public static X509Certificate ToX509Certificate(this byte[] certificate)
    {
      return new X509CertificateParser().ReadCertificate(certificate);
    }
  }
}